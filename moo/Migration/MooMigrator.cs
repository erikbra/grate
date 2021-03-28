using System;
using System.Threading.Tasks;
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
            
            if (!silent)
            {
                _logger.LogInformation("Please press enter when ready to kick...");
                Console.ReadLine();
            }

            if (runInTransaction && !dbMigrator.Database!.SupportsDdlTransactions)
            {
                _logger.LogWarning("You asked to run in a transaction, but this databasetype doesn't support DDL transactions.");
                if (!silent)
                {
                    _logger.LogInformation("Please press enter to continue without transaction support...");
                    Console.ReadLine();
                }
                runInTransaction = false;
            }
            
            
            Console.WriteLine("Moo.Migrate: _migrator.Database is: " + _migrator.Database);
        }
    }
}