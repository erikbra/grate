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

        public void OpenConnection()
        {
            _logger.LogInformation("TODO: OpenConnection");
        }

        public void CloseConnection()
        {
            _logger.LogInformation("TODO: CloseConnection");
        }

        public void OpenAdminConnection()
        {
            _logger.LogInformation("TODO: OpenAdminConnection");
        }

        public void CloseAdminConnection()
        {
            _logger.LogInformation("TODO: CloseAdminConnection");
        }

        public void CreateDatabase()
        {
            _logger.LogInformation("TODO: CreateDatabase");
        }

        public void RunSupportTasks()
        {
            _logger.LogInformation("TODO: RunSupportTasks");
        }

        public string GetCurrentVersion()
        {
            _logger.LogInformation("TODO: GetCurrentVersion");
            return "1.2.3.4";
        }

        public string VersionTheDatabase(string newVersion)
        {
            _logger.LogInformation("TODO: VersionTheDatabase");
            return "1.2.3.5";
        }

        public void Rollback()
        {
            _logger.LogInformation("TODO: Rollback");
        }

        public void RunSql(string sql, ConnectionType connectionType)
        {
            _logger.LogInformation("TODO: RunSql");
        }

        public string GetCurrentHash(string scriptName)
        {
            _logger.LogInformation("TODO: GetCurrentHash");
            return "1.2.3.3";
        }

        public bool HasRun(string scriptName)
        {
            _logger.LogInformation("TODO: HasRun");
            return false;
        }

        public void InsertScriptRun(string scriptName, string sql, string hash, bool runOnce, object versionId)
        {
            _logger.LogInformation("TODO: InsertScriptRun");
        }

        public void InsertScriptRunError(string scriptName, string sql, string errorSql, string errorMessage, object versionId)
        {
            _logger.LogInformation("TODO: InsertScriptRunError");
        }

        private static string? BuildConnectionString(MooConfiguration configuration)
        {
            return $"Initial Catalog={configuration.Database}";
        }

        public string? ConnectionString { get; set; }
    }
}