using System;
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
            var dbMigrator = _migrator;
            await dbMigrator.InitializeConnections();

            //TODO: Read from/put in config
            var silent = true;
            var runInTransaction = true;
            
            _logger.LogInformation("Running {0} v{1} against {2} - {3}.",
                ApplicationInfo.Name,
                ApplicationInfo.Version,
                dbMigrator.Database?.ServerName,
                dbMigrator.Database?.DatabaseName
                );

            var config = _migrator.Configuration;
            var knownFolders = config.KnownFolders;
            
            _logger.LogInformation("Looking in {0} for scripts to run.", knownFolders?.Up?.Path);
            
            PressEnterWhenReady(silent);
            runInTransaction = MakeSureWeCanRunInTransaction(runInTransaction, silent, dbMigrator);

            var changeDropFolder = ChangeDropFolder(config);
            CreateChangeDropFolder(config, changeDropFolder);
            
            _logger.LogDebug("The change_drop (output) folder is: {0}", changeDropFolder);
            LogSeparator('=');
            
            _logger.LogInformation("Setup, Backup, Create/Restore/Drop");
            LogSeparator('=');

            var databaseCreated = false;

            if (config.CreateDatabase)
            {
                databaseCreated = dbMigrator.CreateDatabase();
            }

            TransactionScope? scope = null; 
            if (config.RunInTransaction)
            {
                scope = new TransactionScope();
            }

            dbMigrator.OpenConnection();
            
            LogSeparator('=');
            _logger.LogInformation("Moo Structure");
            LogSeparator('=');
            dbMigrator.OpenConnection();
            
            
            scope?.Complete();
            

        }

        private void LogSeparator(char c)
        {
            _logger.LogInformation(new string(c, 50));
        }

        private void CreateChangeDropFolder(MooConfiguration config, string folder)
        {
            Directory.CreateDirectory(folder);
        }

        private string ChangeDropFolder(MooConfiguration config)
        {
            var folder = Path.Combine(
                config.OutputPath!.ToString(),
                "migrations",
                RemoveInvalidPathChars(config.Database),
                RemoveInvalidPathChars(config.Server)
            );
            return folder;
        }

        private static readonly char[] InvalidPathCharacters = Path.GetInvalidPathChars().Append(':').ToArray();

        private string RemoveInvalidPathChars(string? configDatabase)
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