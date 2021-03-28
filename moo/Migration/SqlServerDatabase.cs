using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using moo.Configuration;

namespace moo.Migration
{
    public class SqlServerDatabase : IDatabase
    {
        private readonly ILogger<SqlServerDatabase> _logger;

        public SqlServerDatabase(ILogger<SqlServerDatabase> logger)
        {
            _logger = logger;
        }
        
        public string? ServerName { get; set; }
        public string? DatabaseName { get; set; }
        public bool SupportsDdlTransactions => true;
        
        public Task InitializeConnections(MooConfiguration configuration)
        {
            _logger.LogInformation("Initializing connections.");

            DatabaseName = configuration.Database;
            ConnectionString = configuration.ConnectionString ?? BuildConnectionString(configuration);
            
            _logger.LogInformation("ConnectionString is: " + ConnectionString);
            
            return Task.CompletedTask;
        }

        private static string? BuildConnectionString(MooConfiguration configuration)
        {
            return $"Initial Catalog={configuration.Database}";
        }

        public string? ConnectionString { get; set; }
    }
}