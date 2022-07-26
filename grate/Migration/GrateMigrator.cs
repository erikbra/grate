using System;
using System.Collections.Generic;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using grate.Configuration;
using grate.Exceptions;
using Microsoft.Extensions.Logging;

namespace grate.Migration;

public class GrateMigrator : IAsyncDisposable
{
    private readonly ILogger<GrateMigrator> _logger;
    private readonly IDbMigrator _migrator;

    public IDbMigrator DbMigrator => _migrator;

    public GrateMigrator(ILogger<GrateMigrator> logger, IDbMigrator migrator)
    {
        _logger = logger;
        _migrator = migrator;
    }

    public async Task Migrate()
    {
        IDbMigrator dbMigrator = _migrator;
        await dbMigrator.InitializeConnections();

        var silent = dbMigrator.Configuration.Silent;
        var database = dbMigrator.Database;
        var config = dbMigrator.Configuration;
        KnownFolders knownFolders = config.KnownFolders ?? throw new ArgumentException(nameof(config.KnownFolders));


        _logger.LogInformation("Running grate v{Version} against {ServerName} - {DatabaseName}.",
            ApplicationInfo.Version,
            database.ServerName,
            database.DatabaseName
        );

        _logger.LogInformation("Looking in {UpFolder} for scripts to run.", knownFolders.Up?.Path);

        PressEnterWhenReady(silent);

        var runInTransaction = MakeSureWeCanRunInTransaction(config.Transaction, silent, dbMigrator);

        var changeDropFolder = ChangeDropFolder(config, database.ServerName, database.DatabaseName);
        CreateChangeDropFolder(changeDropFolder);

        _logger.LogDebug("The change_drop (output) folder is: {ChangeDropFolder}", changeDropFolder);
        Separator('=');

        _logger.LogInformation("Setup, Backup, Create/Restore/Drop");
        Separator('=');
        
        TransactionScope? scope = null;
        IList<Exception> exceptions = new List<Exception>();
        string? newVersion = default;
        long versionId = default;
        
        if (config.Drop)
        {
            await dbMigrator.DropDatabase();
            _logger.LogInformation("{AppName} has removed database ({DatabaseName}) if it existed.", ApplicationInfo.Name, database.DatabaseName);
        }

        var databaseCreated = false;

        if (config.CreateDatabase)
        {
            databaseCreated = await CreateDatabaseIfItDoesNotExist(dbMigrator);
        }
            
      
        if (!string.IsNullOrEmpty(config.Restore))
        {
            await RestoreDatabaseFromPath(config.Restore, dbMigrator);
        }

        await dbMigrator.OpenConnection();

        // Run these first without a transaction, to make sure the tables are created even on a potential rollback
        await CreateGrateStructure(dbMigrator);

        (versionId, newVersion) = await VersionTheDatabase(dbMigrator);

        Separator('=');
        _logger.LogInformation("Migration Scripts");
        Separator('=');

        // This one should not be necessary, we throw on assignment if null
        System.Diagnostics.Debug.Assert(knownFolders != null, nameof(knownFolders) + " != null");

        await BeforeMigration(knownFolders, changeDropFolder, versionId);

        if (config.AlterDatabase)
        {
            await AlterDatabase(dbMigrator, knownFolders, changeDropFolder, versionId);
        }
        
        await dbMigrator.CloseConnection();

        var ct = ConnectionType.Default;
        var th = TransactionHandling.Default;
            
        try
        {
            // Start the transaction, if configured
            if (runInTransaction)
            {
                scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
            }

            await dbMigrator.OpenConnection();

            if (databaseCreated)
            {
                await LogAndProcess(knownFolders.RunAfterCreateDatabase!, changeDropFolder, versionId, ct, th);
            }
            
            // // Make a list of functions first (prepare for a dynamic list of these from configuration)
            // var scriptFolderTasks = new List<Func<Task>>()
            // {
            //     () => LogAndProcess(knownFolders.RunBeforeUp!, changeDropFolder, versionId, ct, th),
            //     () => LogAndProcess(knownFolders.Up!, changeDropFolder, versionId, ct, th),
            //     () => LogAndProcess(knownFolders.RunFirstAfterUp!, changeDropFolder, versionId, ct, th),
            //     () => LogAndProcess(knownFolders.Functions!, changeDropFolder, versionId, ct, th),
            //     () => LogAndProcess(knownFolders.Views!, changeDropFolder, versionId, ct, th),
            //     () => LogAndProcess(knownFolders.Sprocs!, changeDropFolder, versionId, ct, th),
            //     () => LogAndProcess(knownFolders.Triggers!, changeDropFolder, versionId, ct, th),
            //     () => LogAndProcess(knownFolders.Indexes!, changeDropFolder, versionId, ct, th),
            //     () => LogAndProcess(knownFolders.RunAfterOtherAnyTimeScripts!, changeDropFolder, versionId, ct, th)
            // };
            //
            // // Execute them all in order
            // foreach (var task in scriptFolderTasks)
            // {
            //     await task();
            // }

            bool exceptionOccured = false;

            foreach (var folder in knownFolders)
            {
                // Don't run any more folders that runs in the transaction, if the transaction is already aborted
                // (due to an error in a script, or something else)
                if (
                    runInTransaction && exceptionOccured
                    && folder?.TransactionHandling == TransactionHandling.Default)
                {
                    continue;
                }
                
                try {
                    await LogAndProcess(folder!, changeDropFolder, versionId, 
                        folder!.ConnectionType, folder.TransactionHandling);
                }catch (DbException ex)
                {
                    // Catch exceptions, so that we run the rest of the scripts, that should always be run.
                    exceptions.Add(ex);
                    
                    //await dbMigrator.CloseConnection();
                    //scope?.Dispose();
                    
                    exceptionOccured = true;
                }
                // catch (TransactionException ex)
                // {
                //     // Catch exceptions, so that we run the rest of the scripts, that should always be run.
                //     exceptions.Add(ex);
                //     
                //     await dbMigrator.CloseConnection();
                //     scope?.Dispose();
                //     
                //     exceptionOccured = true;
                // }
            }
            await dbMigrator.CloseConnection();
            
            if (!exceptionOccured)
            {
                scope?.Complete();
            }
        }
        // catch (DbException ex)
        // {
        //     // Catch exceptions, so that we run the rest of the scripts, that should always be run.
        //     exceptions.Add(ex);
        // }catch (TransactionException ex)
        // {
        //     // Catch exceptions, so that we run the rest of the scripts, that should always be run.
        //     exceptions.Add(ex);
        // }
        finally
        {
            try
            {
                scope?.Dispose();
            }
            catch (TransactionAbortedException) { }
        }

        // await LogAndProcess(knownFolders.Permissions!, changeDropFolder, versionId, ct,
        //     TransactionHandling.Autonomous);
        // await LogAndProcess(knownFolders.AfterMigration!, changeDropFolder, versionId, ct,
        //     TransactionHandling.Autonomous);

        if (exceptions.Any())
        {
            throw new MigrationFailed(exceptions);
        }

        _logger.LogInformation(
            "\n\ngrate v{Version} has grated your database ({DatabaseName})! You are now at version {NewVersion}. All changes and backups can be found at \"{ChangeDropFolder}\".",
            ApplicationInfo.Version,
            dbMigrator.Database.DatabaseName,
            newVersion,
            changeDropFolder);

        Separator(' ');


    }

    private async Task AlterDatabase(IDbMigrator dbMigrator, KnownFolders knownFolders, string changeDropFolder,
        long versionId)
    {
        await dbMigrator.OpenAdminConnection();
        await LogAndProcess(knownFolders.AlterDatabase!, changeDropFolder, versionId, ConnectionType.Admin,
            TransactionHandling.Autonomous);
        await dbMigrator.CloseAdminConnection();
    }

    private async Task BeforeMigration(KnownFolders knownFolders, string changeDropFolder, long versionId)
    {
        await LogAndProcess(knownFolders.BeforeMigration!, changeDropFolder, versionId, ConnectionType.Default,
            TransactionHandling.Autonomous);
    }

    private async Task CreateGrateStructure(IDbMigrator dbMigrator)
    {
        Separator('=');
        _logger.LogInformation("Grate Structure");
        Separator('=');

        if (dbMigrator.Configuration.DryRun)
        {
            _logger.LogInformation("Skipping creation of versioning structures due to --dryrun");
        }
        else
        {
            await dbMigrator.RunSupportTasks();
        }
    }

    private async Task<(long, string)> VersionTheDatabase(IDbMigrator dbMigrator)
    {
        Separator('=');
        _logger.LogInformation("Versioning");
        Separator('=');

        var currentVersion = await dbMigrator.GetCurrentVersion();
        var newVersion = dbMigrator.Configuration.Version;
        _logger.LogInformation(" Migrating {DatabaseName} from version {CurrentVersion} to {NewVersion}.", dbMigrator.Database.DatabaseName, currentVersion, newVersion);
        var versionId = await dbMigrator.VersionTheDatabase(newVersion);

        return (versionId, newVersion);
    }

    private static async Task<bool> CreateDatabaseIfItDoesNotExist(IDbMigrator dbMigrator)
    {
        bool databaseCreated;
        if (await dbMigrator.DatabaseExists())
        {
            databaseCreated = false;
        }
        else
        {
            databaseCreated = await dbMigrator.CreateDatabase();
        }
        return databaseCreated;
    }

    private static async Task RestoreDatabaseFromPath(string backupPath, IDbMigrator dbMigrator)
    {
        await dbMigrator.RestoreDatabase(backupPath);
    }

    private async Task LogAndProcess(MigrationsFolder folder, string changeDropFolder, long versionId, ConnectionType connectionType, TransactionHandling transactionHandling)
    {
        if (!folder.Path.Exists)
        {
            _logger.LogInformation("Skipping '{FolderName}', {Path} does not exist.", folder.Name, folder.Path);
            return;
        }

        if (!folder.Path.EnumerateFileSystemInfos().Any()) // Ensure we check for subdirectories as well as files
        {
            _logger.LogInformation("Skipping '{FolderName}', {Path} is empty.", folder.Name, folder.Path);
            return;
        }

        Separator(' ');

        var msg = folder.Type switch
        {
            MigrationType.Once => " These should be one time only scripts.",
            MigrationType.EveryTime => " These scripts will run every time.",
            MigrationType.AnyTime => "",
            _ => throw new ArgumentOutOfRangeException(nameof(folder), $"Unexpected MigrationsFolder: {folder.Type}")
        };

        _logger.LogInformation("Looking for {FolderName} scripts in \"{Path}\".{Message}",
            folder.Name,
            folder.Path,
            msg);

        Separator('-');
        await Process(folder, changeDropFolder, versionId, connectionType, transactionHandling);
        Separator('-');
        Separator(' ');
    }

    private async Task Process(MigrationsFolder folder, string changeDropFolder, long versionId,
        ConnectionType connectionType, TransactionHandling transactionHandling)
    {
        var pattern = "*.sql";
        var files = FileSystem.GetFiles(folder.Path, pattern);

        var anySqlRun = false;

        foreach (var file in files)
        {
            var sql = await File.ReadAllTextAsync(file.FullName);

            // Normalize file names to log, so that results won't vary if you run on *nix VS Windows
            var fileNameToLog = string.Join('/',
                Path.GetRelativePath(folder.Path.ToString(), file.FullName).Split(Path.DirectorySeparatorChar));

            bool theSqlRan = await _migrator.RunSql(sql, fileNameToLog, folder.Type, versionId, _migrator.Configuration.Environment,
                connectionType, transactionHandling);

            if (theSqlRan)
            {
                anySqlRun = true;
                try
                {
                    CopyToChangeDropFolder(folder.Path.Parent!, file, changeDropFolder);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Unable to copy {File} to {ChangeDropFolder}. \n{Exception}", file, changeDropFolder, ex.Message);
                }
            }
        }

        if (!anySqlRun)
        {
            _logger.LogInformation(" No sql run, either an empty folder, or all files run against destination previously.");
        }

    }

    private void CopyToChangeDropFolder(DirectoryInfo migrationRoot, FileSystemInfo file, string changeDropFolder)
    {
        var relativePath = Path.GetRelativePath(migrationRoot.ToString(), file.FullName);

        string destinationFile = Path.Combine(changeDropFolder, "itemsRan", relativePath);

        var parent = Path.GetDirectoryName(destinationFile)!;
        var parentDir = new DirectoryInfo(parent);
        parentDir.Create();

        _logger.LogTrace("Copying file {Filename} to {Destination}", file.FullName, destinationFile);

        File.Copy(file.FullName, destinationFile);
    }

    // ReSharper disable once TemplateIsNotCompileTimeConstantProblem
#pragma warning disable CA2254 // Template should be a static expression.
    private void Separator(char c) => _logger.LogInformation(new string(c, 80));
#pragma warning restore CA2254 // Template should be a static expression

    private static void CreateChangeDropFolder(string folder)
    {
        if (Directory.Exists(folder))
        {
            Directory.Delete(folder, recursive: true);
        }
        Directory.CreateDirectory(folder);
    }

    public static string ChangeDropFolder(GrateConfiguration config, string? server, string? database)
    {
        var folder = Path.Combine(
            config.OutputPath.ToString(),
            "migrations",
            RemoveInvalidPathChars(database),
            RemoveInvalidPathChars(server),
            RemoveInvalidPathChars(DateTime.Now.ToString("O"))
        );
        return folder;
    }

    private static readonly char[] InvalidPathCharacters = Path.GetInvalidPathChars()
        .Append(':')
        .Append(',')
        .Append('+')
        .ToArray();

    private static string RemoveInvalidPathChars(string? path)
    {
        var builder = new StringBuilder(path);
        foreach (var c in InvalidPathCharacters)
        {
            builder.Replace(c, '_');
        }
        return builder.ToString();
    }

    private void PressEnterWhenReady(bool silent)
    {
        if (!silent)
        {
            _logger.LogInformation("Please press enter when ready to kick...");
            Console.ReadLine();
        }
    }

    private bool MakeSureWeCanRunInTransaction(bool runInTransaction, bool silent, IDbMigrator dbMigrator)
    {
        if (runInTransaction && !dbMigrator.Database.SupportsDdlTransactions)
        {
            _logger.LogWarning("You asked to run in a transaction, but this database type doesn't support DDL transactions.");
            if (!silent)
            {
                _logger.LogInformation("Please press enter to continue without transaction support...");
                Console.ReadLine();
            }
            return false;
        }

        return runInTransaction;
    }

    public async ValueTask DisposeAsync()
    {
        await _migrator.DisposeAsync();
        GC.SuppressFinalize(this);
    }
}
