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
        IFoldersConfiguration knownFolders = config.Folders ?? throw new ArgumentException(nameof(config.Folders));


        _logger.LogInformation("Running grate v{Version} against {ServerName} - {DatabaseName}.",
            ApplicationInfo.Version,
            database.ServerName,
            database.DatabaseName
        );

        _logger.LogInformation("Looking in {UpFolder} for scripts to run.", config.SqlFilesDirectory);

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
        dbMigrator.SetDefaultConnectionActive();

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

        // Run these first without a transaction, to make sure the tables are created even on a potential rollback
        await CreateGrateStructure(dbMigrator);

        string? newVersion;
        long versionId;
        (versionId, newVersion) = await VersionTheDatabase(dbMigrator);

        Separator('=');
        _logger.LogInformation("Migration Scripts");
        Separator('=');

        try
        {
            // Start the transaction, if configured
            if (runInTransaction)
            {
                scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
            }

            bool exceptionOccured = false;

            foreach (var folder in knownFolders.Values)
            {
                var processingFolderInDefaultTransaction = folder?.TransactionHandling == TransactionHandling.Default;

                // Don't run any more folders that runs in the default transaction, if the transaction is already aborted
                // (due to an error in a script, or something else)
                if (exceptionOccured && processingFolderInDefaultTransaction)
                {
                    continue;
                }

                // This is an ugly "if" run on every script, to check one special folder which has conditions.
                // If possible, we should find a 'cleaner' way to do this.
                if (nameof(KnownFolderNames.RunAfterCreateDatabase).Equals(folder?.Name) && !databaseCreated)
                {
                    continue;
                }

                try
                {
                    if (processingFolderInDefaultTransaction)
                    {
                        await LogAndProcess(config.SqlFilesDirectory, folder!, changeDropFolder, versionId, folder!.ConnectionType, folder.TransactionHandling);
                    }
                    else
                    {
                        using var s = new TransactionScope(TransactionScopeOption.Suppress, TransactionScopeAsyncFlowOption.Enabled);
                        using (await dbMigrator.OpenNewActiveConnection())
                        {
                            await LogAndProcess(config.SqlFilesDirectory, folder!, changeDropFolder, versionId, folder!.ConnectionType, folder.TransactionHandling);
                        }
                        s.Complete();
                    }
                }
                catch (DbException ex)
                {
                    // Catch exceptions, so that we run the rest of the scripts, that should always be run.
                    exceptions.Add(ex);
                    exceptionOccured = true;
                }
            }

            await dbMigrator.CloseConnection();

            if (!exceptionOccured)
            {
                scope?.Complete();
            }
        }
        finally
        {
            try
            {
                scope?.Dispose();
            }
            catch (TransactionAbortedException) { }
        }

        if (exceptions.Any())
        {
            throw new MigrationFailed(exceptions);
        }

        if (!config.DryRun)
        {
            //If we get here this means no exceptions are thrown above, so we can conclude the migration was successfull!
            await _migrator.Database.ChangeVersionStatus(MigrationStatus.Finished, versionId);
        }

        _logger.LogInformation(
            "\n\ngrate v{Version} has grated your database ({DatabaseName})! You are now at version {NewVersion}. All changes and backups can be found at \"{ChangeDropFolder}\".",
            ApplicationInfo.Version,
            dbMigrator.Database.DatabaseName,
            newVersion,
            changeDropFolder);

        Separator(' ');


    }

    private async Task EnsureConnectionIsOpen(ConnectionType connectionType)
    {
        switch (connectionType)
        {
            case ConnectionType.Default:
                await _migrator.OpenActiveConnection();
                break;
            case ConnectionType.Admin:
                await _migrator.OpenAdminConnection();
                break;
            default:
                throw new UnknownConnectionType(connectionType);
        }
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
            using var s = new TransactionScope(TransactionScopeOption.Suppress, TransactionScopeAsyncFlowOption.Enabled);
            using (await dbMigrator.OpenNewActiveConnection())
            {
                await dbMigrator.RunSupportTasks();
            }
            s.Complete();
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

    private async Task<bool> CreateDatabaseIfItDoesNotExist(IDbMigrator dbMigrator)
    {
        bool databaseCreated;
        if (await dbMigrator.DatabaseExists())
        {
            databaseCreated = false;
        }
        else
        {
            var config = dbMigrator.Configuration;
            var createDatabaseFolder = config.Folders?.CreateDatabase;
            var database = _migrator.Database;

            var path = Wrap(config.SqlFilesDirectory, createDatabaseFolder?.Path ?? "zz-xx-øø-definitely-does-not-exist");

            if (createDatabaseFolder is not null && path.Exists)
            {
                //await LogAndProcess(config.SqlFilesDirectory, folder!, changeDropFolder, versionId, folder!.ConnectionType, folder.TransactionHandling);
                var changeDropFolder = ChangeDropFolder(config, database.ServerName, database.DatabaseName);
                databaseCreated = await ProcessWithoutLogging(
                    config.SqlFilesDirectory,
                    createDatabaseFolder,
                    changeDropFolder,
                    createDatabaseFolder.ConnectionType,
                    createDatabaseFolder.TransactionHandling
                );
            }
            else
            {
                databaseCreated = await dbMigrator.CreateDatabase();
            }
        }
        return databaseCreated;
    }

    private static async Task RestoreDatabaseFromPath(string backupPath, IDbMigrator dbMigrator)
    {
        await dbMigrator.RestoreDatabase(backupPath);
    }

    private static DirectoryInfo Wrap(DirectoryInfo root, string subFolder) => new(Path.Combine(root.ToString(), subFolder));

    private async Task LogAndProcess(DirectoryInfo root, MigrationsFolder folder, string changeDropFolder, long versionId, ConnectionType connectionType, TransactionHandling transactionHandling)
    {
        var path = Wrap(root, folder.Path);

        if (!path.Exists)
        {
            _logger.LogInformation("Skipping '{FolderName}', {Path} does not exist.", folder.Name, folder.Path);
            return;
        }

        if (!path.EnumerateFileSystemInfos().Any()) // Ensure we check for subdirectories as well as files
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
            Wrap(root, folder.Path),
            msg);

        Separator('-');
        await Process(root, folder, changeDropFolder, versionId, connectionType, transactionHandling);
        Separator('-');
        Separator(' ');
    }

    private async Task Process(DirectoryInfo root, MigrationsFolder folder, string changeDropFolder, long versionId,
        ConnectionType connectionType, TransactionHandling transactionHandling)
    {
        var path = Wrap(root, folder.Path);

        await EnsureConnectionIsOpen(connectionType);

        var pattern = "*.sql";
        var files = FileSystem.GetFiles(path, pattern, _migrator.Configuration.IgnoreDirectoryNames);

        var anySqlRun = false;

        foreach (var file in files)
        {
            var sql = await File.ReadAllTextAsync(file.FullName);

            // Normalize file names to log, so that results won't vary if you run on *nix VS Windows
            var fileNameToLog = _migrator.Configuration.IgnoreDirectoryNames
                ? file.Name
                : string.Join('/', Path.GetRelativePath(path.ToString(), file.FullName).Split(Path.DirectorySeparatorChar));

            bool theSqlRan = await _migrator.RunSql(sql, fileNameToLog, folder.Type, versionId, _migrator.Configuration.Environment,
                connectionType, transactionHandling);

            if (theSqlRan)
            {
                anySqlRun = true;
                try
                {
                    CopyToChangeDropFolder(path.Parent!, file, changeDropFolder);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Unable to copy {File} to {ChangeDropFolder}. \n{Exception}", file, changeDropFolder, ex.Message);
                }
            }
        }

        if (!anySqlRun && !_migrator.Configuration.DryRun)
        {
            _logger.LogInformation(" No sql run, either an empty folder, or all files run against destination previously.");
        }

    }

    private async Task<bool> ProcessWithoutLogging(DirectoryInfo root, MigrationsFolder folder, string changeDropFolder,
        ConnectionType connectionType, TransactionHandling transactionHandling)
    {
        var path = Wrap(root, folder.Path);

        await EnsureConnectionIsOpen(connectionType);

        var pattern = "*.sql";
        var files = FileSystem.GetFiles(path, pattern, _migrator.Configuration.IgnoreDirectoryNames);

        var anySqlRun = false;

        foreach (var file in files)
        {
            var sql = await File.ReadAllTextAsync(file.FullName);

            // Normalize file names to log, so that results won't vary if you run on *nix VS Windows
            var fileNameToLog = _migrator.Configuration.IgnoreDirectoryNames
            ? file.Name
            : string.Join('/', Path.GetRelativePath(path.ToString(), file.FullName).Split(Path.DirectorySeparatorChar));

            bool theSqlRan = await _migrator.RunSqlWithoutLogging(sql, fileNameToLog, _migrator.Configuration.Environment,
                connectionType, transactionHandling);

            if (theSqlRan)
            {
                anySqlRun = true;
                try
                {
                    CopyToChangeDropFolder(path.Parent!, file, changeDropFolder);
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

        return anySqlRun;

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
