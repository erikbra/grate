using System;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using moo.Configuration;

namespace moo.Migration
{
    public class SqlServerDatabase : IDatabase, IDisposable
    {
        private readonly ILogger<SqlServerDatabase> _logger;
        private SqlConnection _connection;
        private SqlConnection _adminConnection;

        public SqlServerDatabase(ILogger<SqlServerDatabase> logger)
        {
            _logger = logger;
        }
        
        public string? ServerName { get; set; }
        public string? DatabaseName => _connection?.Database;
        public bool SupportsDdlTransactions => true;
        
        public Task InitializeConnections(MooConfiguration configuration)
        {
            _logger.LogInformation("Initializing connections.");

            ConnectionString = configuration.ConnectionString;
            _connection = new SqlConnection(ConnectionString);
            
            AdminConnectionString = configuration.AdminConnectionString;
            _adminConnection = new SqlConnection(AdminConnectionString);
            
            _logger.LogInformation("ConnectionString is: " + ConnectionString);
            _logger.LogInformation("AdminConnectionString is: " + AdminConnectionString);
            
            return Task.CompletedTask;
        }

        private string AdminConnectionString { get; set; }
        private string? ConnectionString { get; set; }

        public async Task OpenConnection()
        {
            await _connection.OpenAsync();
        }

        public void CloseConnection()
        {
            _logger.LogInformation("TODO: CloseConnection");
        }

        public async Task OpenAdminConnection()
        {
            await _adminConnection.OpenAsync();
        }

        public async Task CloseAdminConnection()
        {
            _logger.LogInformation("TODO: CloseAdminConnection");
        }

        public async Task CreateDatabase()
        {
            const string? sql = "SELECT name FROM sys.databases";
            var databases = await _adminConnection.QueryAsync<string>(sql);

            if (!databases.Contains(DatabaseName))
            {
                var cmd = _adminConnection.CreateCommand();
                //var res = await _cmd.Execute($"CREATE database {DatabaseName}");
                cmd.CommandText = $"CREATE database {DatabaseName}";
                var res = await cmd.ExecuteNonQueryAsync();
            }
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

        public void Dispose()
        {
            _connection.Dispose();
        }
    }
}