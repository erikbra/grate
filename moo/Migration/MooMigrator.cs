using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using Microsoft.Extensions.Logging;
using moo.Configuration;

namespace moo.Migration
{
    public class MooMigrator
    {
        private readonly ILogger<MooMigrator> _logger;
        private readonly IDbMigrator _migrator;

        public MooMigrator(ILogger<MooMigrator> logger, IDbMigrator migrator)
        {
            _logger = logger;
            _migrator = migrator;
        }

        public async Task Migrate()
        {
            IDbMigrator dbMigrator = _migrator;
            await dbMigrator.InitializeConnections();

            //TODO: Read from/put in config
            var silent = true;

            var database = dbMigrator.Database;
            
            _logger.LogInformation("Running {0} v{1} against {2} - {3}.",
                ApplicationInfo.Name,
                ApplicationInfo.Version,
                database?.ServerName,
                database?.DatabaseName
                );

            var config = _migrator.Configuration;
            KnownFolders knownFolders = config.KnownFolders;
            
            Info("Looking in {0} for scripts to run.", knownFolders?.Up?.Path);
            
            PressEnterWhenReady(silent);
            var runInTransaction = MakeSureWeCanRunInTransaction(config.RunInTransaction, silent, dbMigrator);

            var changeDropFolder = ChangeDropFolder(config, database?.ServerName, database?.DatabaseName);
            CreateChangeDropFolder(config, changeDropFolder);
            
            Debug("The change_drop (output) folder is: {0}", changeDropFolder);
            Separator('=');
            
            Info("Setup, Backup, Create/Restore/Drop");
            Separator('=');

            var databaseCreated = false;

            if (config.CreateDatabase)
            {
                await dbMigrator.OpenAdminConnection();
                databaseCreated = await dbMigrator.CreateDatabase();
            }

            TransactionScope? scope = null; 
            if (runInTransaction)
            {
                scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
            }

            await dbMigrator.OpenConnection();
            
            Separator('=');
            Info("Moo Structure");
            Separator('=');
            await dbMigrator.RunSupportTasks();
            
            Separator('=');
            Info("Versioning");
            Separator('=');
            var currentVersion = dbMigrator.GetCurrentVersion();
            var newVersion = config.Version;
            Info(" Migrating {0} from version {1} to {2}.", 
                database?.DatabaseName, currentVersion, newVersion);
            var versionId = dbMigrator.VersionTheDatabase(newVersion);
            
            Separator('=');
            Info("Migration Scripts");
            Separator('=');

            using (new TransactionScope(TransactionScopeOption.Suppress, TransactionScopeAsyncFlowOption.Enabled))
            {
                LogAndProcess(knownFolders!.BeforeMigration!, changeDropFolder, versionId, newVersion, ConnectionType.Default);
            }

            if (config.AlterDatabase)
            {
                dbMigrator.OpenAdminConnection();
                LogAndProcess(knownFolders!.AlterDatabase!, changeDropFolder, versionId, newVersion, ConnectionType.Admin);
                dbMigrator.CloseAdminConnection();
            }

            if (databaseCreated)
            {
                LogAndProcess(knownFolders.RunAfterCreateDatabase!, changeDropFolder, versionId, newVersion, ConnectionType.Default);
            }
            
            LogAndProcess(knownFolders.RunBeforeUp!, changeDropFolder, versionId, newVersion, ConnectionType.Default);
            LogAndProcess(knownFolders.Up!, versionId, changeDropFolder, newVersion, ConnectionType.Default);
            LogAndProcess(knownFolders.RunFirstAfterUp!, changeDropFolder, versionId, newVersion, ConnectionType.Default);
            LogAndProcess(knownFolders.Views!, versionId, changeDropFolder, newVersion, ConnectionType.Default);
            LogAndProcess(knownFolders.Sprocs!, versionId, changeDropFolder, newVersion, ConnectionType.Default);
            LogAndProcess(knownFolders.Triggers!, versionId, changeDropFolder, newVersion, ConnectionType.Default);
            LogAndProcess(knownFolders.Indexes!, versionId, changeDropFolder, newVersion, ConnectionType.Default);
            LogAndProcess(knownFolders.RunAfterOtherAnyTimeScripts!, changeDropFolder, versionId, newVersion, ConnectionType.Default);
            
            scope?.Complete();

            using (new TransactionScope(TransactionScopeOption.Suppress, TransactionScopeAsyncFlowOption.Enabled))
            {
                LogAndProcess(knownFolders.Permissions!, changeDropFolder, versionId, newVersion, ConnectionType.Default);
                LogAndProcess(knownFolders.AfterMigration!, changeDropFolder, versionId, newVersion, ConnectionType.Default);
            }
            
            Info(
                "\n\n{1} v{2} has moo'd your database ({3})! You are now at version {4}. All changes and backups can be found at \"{5}\".",
                ApplicationInfo.Name,
                ApplicationInfo.Version,
                dbMigrator?.Database?.DatabaseName,
                newVersion,
                changeDropFolder);
            
            Separator(' ');
            
        }

        private void LogAndProcess(MigrationsFolder folder, string changeDropFolder, string versionId, string newVersion, ConnectionType connectionType)
        {
            Separator(' ');

            var msg = folder.Type switch
            {
                MigrationType.Once => " These should be one time only scripts.",
                MigrationType.EveryTime => " These scripts will run every time.",
                _ => throw new ArgumentOutOfRangeException()
            };

            Info("Looking for {0} scripts in \"{1}\".{2}",
                folder.Name,
                folder.Path,
                msg);

            Separator('-');
            Process(folder, changeDropFolder, versionId, newVersion, connectionType);
            Separator('-');
            Separator(' ');
        }

        private void Process(MigrationsFolder folder, string changeDropFolder, string versionId, string newVersion, ConnectionType connectionType)
        {
            if (!folder.Path.Exists)
            {
                Info("{0} does not exist. Skipping.", folder.Path);
                return;
            }

            var pattern = "*.sql";
            var files = GetFiles(folder.Path, pattern);

            foreach (var file in files)
            {
                var txt = File.ReadAllText(file.FullName);
                var sql = ReplaceTokens(txt);

                bool theSqlRan = _migrator.RunSql(sql, file.FullName, folder.Type, versionId, "", "", "",
                    connectionType);
                if (theSqlRan)
                {
                    try
                    {
                        CopyToChangeDropFolder(file, folder, changeDropFolder);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "Unable to copy {0} to {1}. {2}{3}", file, folder.Path,
                            System.Environment.NewLine, ex.Message);
                    }
                }
                
                

            }
            
        }

        private void CopyToChangeDropFolder(FileSystemInfo file, MigrationsFolder folder, string changeDropFolder)
        {
            var cfg = _migrator.Configuration;

            var relativePath = Path.GetRelativePath(folder.Path.FullName, file.FullName);
            
            string destinationFile = Path.Combine(changeDropFolder, "itemsRan", relativePath);

            var parent = Path.GetDirectoryName(destinationFile)!;
            var parentDir = new DirectoryInfo(parent);
            parentDir.Create();
            
            _logger.LogDebug("Copying file {0} to {1}.", file.FullName, destinationFile);
            
            File.Copy(file.FullName, destinationFile);
        }

        private string ReplaceTokens(string txt)
        {
            throw new NotImplementedException();
        }

        private static IEnumerable<FileSystemInfo> GetFiles(DirectoryInfo folderPath, string pattern)
        {
            return folderPath.EnumerateFileSystemInfos(pattern, SearchOption.AllDirectories);
        }

        private void Info(string format, params object?[] args) => _logger.LogInformation(format, args);
        private void Debug(string format, params object?[] args) => _logger.LogDebug(format, args);

        private void Separator(char c)
        {
            _logger.LogInformation(new string(c, 80));
        }

        private void CreateChangeDropFolder(MooConfiguration config, string folder)
        {
            Directory.CreateDirectory(folder);
        }

        private string ChangeDropFolder(MooConfiguration config, string? server, string? database)
        {
            var folder = Path.Combine(
                config.OutputPath!.ToString(),
                "migrations",
                RemoveInvalidPathChars(database),
                RemoveInvalidPathChars(server)
            );
            return folder;
        }

        private static readonly char[] InvalidPathCharacters = Path.GetInvalidPathChars().Append(':').ToArray();

        private static string RemoveInvalidPathChars(string? configDatabase)
        {
            var builder = new StringBuilder(configDatabase);
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
        
    }
}