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
            
            _logger.LogInformation("Running {0} v{1} against {2} - {3}.",
                ApplicationInfo.Name,
                ApplicationInfo.Version,
                dbMigrator.Database?.ServerName,
                dbMigrator.Database?.DatabaseName
                );
            
            Console.WriteLine("Moo.Migrate: _migrator.Database is: " + _migrator.Database);
        }
    }
}