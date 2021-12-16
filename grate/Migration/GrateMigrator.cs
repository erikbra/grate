using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using grate.Configuration;
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


        _logger.LogInformation("Running grate v{version} against {serverName} - {databaseName}.",
            ApplicationInfo.Version,
            database?.ServerName,
            database?.DatabaseName
        );

        _logger.LogInformation("Looking in {upFolder} for scripts to run.", knownFolders?.Up?.Path);

        PressEnterWhenReady(silent);

        var runInTransaction = MakeSureWeCanRunInTransaction(config.Transaction, silent, dbMigrator);

        var changeDropFolder = ChangeDropFolder(config, database?.ServerName, database?.DatabaseName);
        CreateChangeDropFolder(changeDropFolder);

        _logger.LogDebug("The change_drop (output) folder is: {changeDropFolder}", changeDropFolder);
        Separator('=');

        _logger.LogInformation("Setup, Backup, Create/Restore/Drop");
        Separator('=');

        if (config.Drop)
        {
            await dbMigrator.DropDatabase();
            _logger.LogInformation("{appName} has removed database ({databaseName}) if it existed.", ApplicationInfo.Name, database?.DatabaseName);
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

        TransactionScope? scope = null;
        try
        {
            // Run these first without a transaction, to make sure the tables are created even on a potential rollback
            await CreateGrateStructure(dbMigrator);

            // Start the transaction, if configured
            if (runInTransaction)
            {
                scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
            }

            await dbMigrator.OpenConnection();
            var (versionId, newVersion) = await VersionTheDatabase(dbMigrator);

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

            if (databaseCreated)
            {
                await LogAndProcess(knownFolders.RunAfterCreateDatabase!, changeDropFolder, versionId, ConnectionType.Default);
            }

            await LogAndProcess(knownFolders.RunBeforeUp!, changeDropFolder, versionId, ConnectionType.Default);
            await LogAndProcess(knownFolders.Up!, changeDropFolder, versionId, ConnectionType.Default);
            await LogAndProcess(knownFolders.RunFirstAfterUp!, changeDropFolder, versionId, ConnectionType.Default);
            await LogAndProcess(knownFolders.Functions!, changeDropFolder, versionId, ConnectionType.Default);
            await LogAndProcess(knownFolders.Views!, changeDropFolder, versionId, ConnectionType.Default);
            await LogAndProcess(knownFolders.Sprocs!, changeDropFolder, versionId, ConnectionType.Default);
            await LogAndProcess(knownFolders.Triggers!, changeDropFolder, versionId, ConnectionType.Default);
            await LogAndProcess(knownFolders.Indexes!, changeDropFolder, versionId, ConnectionType.Default);
            await LogAndProcess(knownFolders.RunAfterOtherAnyTimeScripts!, changeDropFolder, versionId, ConnectionType.Default);

            scope?.Complete();

            using (new TransactionScope(TransactionScopeOption.Suppress, TransactionScopeAsyncFlowOption.Enabled))
            {
                await LogAndProcess(knownFolders.Permissions!, changeDropFolder, versionId, ConnectionType.Default);
                await LogAndProcess(knownFolders.AfterMigration!, changeDropFolder, versionId, ConnectionType.Default);
            }

            _logger.LogInformation(
                "\n\ngrate v{version} has grated your database ({databaseName})! You are now at version {newVersion}. All changes and backups can be found at \"{changeDropFolder}\".",
                ApplicationInfo.Version,
                dbMigrator?.Database?.DatabaseName,
                newVersion,
                changeDropFolder);

            Separator(' ');

        }
        finally
        {
            scope?.Dispose();
        }

    }

    private async Task AlterDatabase(IDbMigrator dbMigrator, KnownFolders knownFolders, string changeDropFolder,
        long versionId)
    {
        using (new TransactionScope(TransactionScopeOption.Suppress,
                   TransactionScopeAsyncFlowOption.Enabled))
        {
            await dbMigrator.OpenAdminConnection();
            await LogAndProcess(knownFolders!.AlterDatabase!, changeDropFolder, versionId, ConnectionType.Admin);
            await dbMigrator.CloseAdminConnection();
        }
    }

    private async Task BeforeMigration(KnownFolders knownFolders, string changeDropFolder, long versionId)
    {
        using (new TransactionScope(TransactionScopeOption.Suppress, TransactionScopeAsyncFlowOption.Enabled))
        {
            await LogAndProcess(knownFolders!.BeforeMigration!, changeDropFolder, versionId, ConnectionType.Default);
        }
    }

    private async Task CreateGrateStructure(IDbMigrator dbMigrator)
    {
        await dbMigrator.OpenConnection();

        Separator('=');
        _logger.LogInformation("Grate Structure");
        Separator('=');

        await dbMigrator.RunSupportTasks();
        await dbMigrator.CloseConnection();
    }

    private async Task<(long, string)> VersionTheDatabase(IDbMigrator dbMigrator)
    {
        Separator('=');
        _logger.LogInformation("Versioning");
        Separator('=');

        var currentVersion = await dbMigrator.GetCurrentVersion();
        var newVersion = dbMigrator.Configuration.Version;
        _logger.LogInformation(" Migrating {databaseName} from version {currentVersion} to {newVersion}.", dbMigrator.Database?.DatabaseName, currentVersion, newVersion);
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
            await dbMigrator.OpenAdminConnection();
            databaseCreated = await dbMigrator.CreateDatabase();
            await dbMigrator.CloseAdminConnection();
        }
        return databaseCreated;
    }

    private static async Task RestoreDatabaseFromPath(string backupPath, IDbMigrator dbMigrator)
    {
        await dbMigrator.RestoreDatabase(backupPath);
    }

    private async Task LogAndProcess(MigrationsFolder folder, string changeDropFolder, long versionId, ConnectionType connectionType)
    {
        Separator(' ');

        var msg = folder.Type switch
        {
            MigrationType.Once => " These should be one time only scripts.",
            MigrationType.EveryTime => " These scripts will run every time.",
            MigrationType.AnyTime => "",
            _ => throw new ArgumentOutOfRangeException(nameof(folder), $"Unexpected MigrationsFolder: {folder.Type}")
        };

        _logger.LogInformation("Looking for {folderName} scripts in \"{path}\".{message}",
            folder.Name,
            folder.Path,
            msg);

        Separator('-');
        await Process(folder, changeDropFolder, versionId, connectionType);
        Separator('-');
        Separator(' ');
    }

    private async Task Process(MigrationsFolder folder, string changeDropFolder, long versionId, ConnectionType connectionType)
    {
        if (!folder.Path.Exists)
        {
            _logger.LogDebug("{Path} does not exist. Skipping.", folder.Path);
            return;
        }

            var pattern = "*.sql";
            var files = FileSystem.GetFiles(folder.Path, pattern);

        foreach (var file in files)
        {
            var sql = await File.ReadAllTextAsync(file.FullName);

                // Normalize file names to log, so that results won't vary if you run on *nix VS Windows
                var fileNameToLog = string.Join('/',
                    Path.GetRelativePath(folder.Path.ToString(), file.FullName).Split(Path.DirectorySeparatorChar));

                bool theSqlRan = await _migrator.RunSql(sql, fileNameToLog, folder.Type, versionId, _migrator.Configuration.Environment,
                    connectionType);

                if (theSqlRan)
                {
                    try
                    {
                        CopyToChangeDropFolder(folder.Path.Parent!, file, changeDropFolder);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "Unable to copy {file} to {changeDropFolder}. \n{exception}", file, changeDropFolder, ex.Message);
                    }
                }
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
        
#pragma warning disable CA2254 // Template should be a static expression.  Bug in pre-release .net 6: https://github.com/dotnet/roslyn-analyzers/issues/5415
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
                config.OutputPath!.ToString(),
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
        if (runInTransaction && !dbMigrator.Database!.SupportsDdlTransactions)
        {
            _logger.LogWarning("You asked to run in a transaction, but this databasetype doesn't support DDL transactions.");
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
