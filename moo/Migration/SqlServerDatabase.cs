using System;
using System.Data;
using System.Linq;
using System.Reflection.Metadata;
using System.Security.Claims;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using moo.Configuration;

namespace moo.Migration
{
    public class SqlServerDatabase : IDatabase, IDisposable
    {
        private const string SchemaName = "moo";
        private readonly ILogger<SqlServerDatabase> _logger;
        private SqlConnection _connection;
        private SqlConnection? _adminConnection;

        public SqlServerDatabase(ILogger<SqlServerDatabase> logger)
        {
            _logger = logger;
        }

        public string? ServerName => _connection.DataSource;
        public string? DatabaseName => _connection?.Database;
        public bool SupportsDdlTransactions => true;
        
        public Task InitializeConnections(MooConfiguration configuration)
        {
            _logger.LogInformation("Initializing connections.");

            ConnectionString = configuration.ConnectionString;
            _connection = new SqlConnection(ConnectionString);
            
            AdminConnectionString = configuration.AdminConnectionString;
            
            //_logger.LogInformation("ConnectionString is: " + ConnectionString);
            //_logger.LogInformation("AdminConnectionString is: " + AdminConnectionString);
            
            return Task.CompletedTask;
        }

        private string? AdminConnectionString { get; set; }
        private string? ConnectionString { get; set; }

        private SqlConnection AdminConnection => _adminConnection ??= new SqlConnection(AdminConnectionString);

        public async Task OpenConnection()
        {
            if (_connection.State != ConnectionState.Open)
            {
                await _connection.OpenAsync();
                var res = await _connection.QueryAsync<string>("SELECT DB_NAME()");
            }
        }

        public async Task CloseConnection()
        {
            if (_connection.State == ConnectionState.Open)
            {
                await _connection.CloseAsync();
            }
        }

        public async Task OpenAdminConnection()
        {
            if (AdminConnection.State != ConnectionState.Open)
            {
                await AdminConnection.OpenAsync();
            }
        }

        public async Task CloseAdminConnection()
        {
            if (AdminConnection.State == ConnectionState.Open)
            {
                await AdminConnection.CloseAsync();
            }
        }

        public async Task CreateDatabase()
        {
            const string? sql = "SELECT name FROM sys.databases";
            
            await OpenAdminConnection();
            var databases = await AdminConnection.QueryAsync<string>(sql);

            if (!databases.Contains(DatabaseName))
            {
                var cmd = AdminConnection.CreateCommand();
                //var res = await _cmd.Execute($"CREATE database {DatabaseName}");
                cmd.CommandText = $"CREATE database {DatabaseName}";
                var res = await cmd.ExecuteNonQueryAsync();
                await Task.Delay(5000);
            }
            
            await CloseAdminConnection();
        }

        public async Task RunSupportTasks()
        {
            _logger.LogInformation("TODO: RunSupportTasks");
            await CreateRunSchema();
            await CreateScriptsRunTable();
            await CreateScriptsRunErrorsTable();
            await CreateVersionTable();
        }

        private async Task CreateRunSchema()
        {
            string createSql = @$"CREATE SCHEMA {SchemaName};";
            
            if (!await RunSchemaExists())
            {
                await using var cmd = _connection.CreateCommand();
                cmd.CommandText = createSql;
                await cmd.ExecuteNonQueryAsync();
            }
        }

        private async Task<bool> RunSchemaExists()
        {
            string sql = "SELECT s.name FROM sys.schemas s WHERE name = '" + SchemaName + "'";
            await using var cmd = _connection.CreateCommand();
            cmd.CommandText = sql;
            var res = await cmd.ExecuteScalarAsync();
            return res?.ToString() == SchemaName;
        }

        private async Task CreateScriptsRunTable()
        {
            string createSql = $@"
CREATE TABLE [{SchemaName}].[ScriptsRun](
	[id] [bigint] IDENTITY(1,1) NOT NULL,
	[version_id] [bigint] NULL,
	[script_name] [nvarchar](255) NULL,
	[text_of_script] [text] NULL,
	[text_hash] [nvarchar](512) NULL,
	[one_time_script] [bit] NULL,
	[entry_date] [datetime] NULL,
	[modified_date] [datetime] NULL,
	[entered_by] [nvarchar](50) NULL,
    CONSTRAINT PK_ScriptsRun_Id PRIMARY KEY CLUSTERED (id)
);";
            if (!await ScriptsRunTableExists())
            {
                await using var cmd = _connection.CreateCommand();
                cmd.CommandText = createSql;
                var res = await cmd.ExecuteNonQueryAsync();
            }
        }
        
        private async Task CreateScriptsRunErrorsTable()
        {
            string createSql = $@"
CREATE TABLE [{SchemaName}].[ScriptsRunErrors](
	[id] [bigint] IDENTITY(1,1) NOT NULL,
	[repository_path] [nvarchar](255) NULL,
	[version] [nvarchar](50) NULL,
	[script_name] [nvarchar](255) NULL,
	[text_of_script] [ntext] NULL,
	[erroneous_part_of_script] [ntext] NULL,
	[error_message] [ntext] NULL,
	[entry_date] [datetime] NULL,
	[modified_date] [datetime] NULL,
	[entered_by] [nvarchar](50) NULL,
    CONSTRAINT PK_ScriptsRunErrors_Id PRIMARY KEY CLUSTERED (id)
);";
            if (!await ScriptsRunErrorsTableExists())
            {
                await using var cmd = _connection.CreateCommand();
                cmd.CommandText = createSql;
                var res = await cmd.ExecuteNonQueryAsync();
            }
        }
        
        private async Task CreateVersionTable()
        {
            string createSql = $@"
CREATE TABLE [{SchemaName}].[Version](
	[id] [bigint] IDENTITY(1,1) NOT NULL,
	[repository_path] [nvarchar](255) NULL,
	[version] [nvarchar](50) NULL,
	[entry_date] [datetime] NULL,
	[modified_date] [datetime] NULL,
	[entered_by] [nvarchar](50) NULL,
    CONSTRAINT PK_Version_Id PRIMARY KEY CLUSTERED (id)
);";
            if (!await VersionTableExists())
            {
                await using var cmd = _connection.CreateCommand();
                cmd.CommandText = createSql;
                var res = await cmd.ExecuteNonQueryAsync();
            }
        }

        private async Task<bool> ScriptsRunTableExists() => await TableExists("ScriptsRun");
        private async Task<bool> ScriptsRunErrorsTableExists() => await TableExists("ScriptsRunErrors");
        private async Task<bool> VersionTableExists() => await TableExists("Version");
        
        
        private async Task<bool> TableExists(string tableName)
        {
            string existsSql = $@"SELECT OBJECT_ID(N'[{SchemaName}].[{tableName}]', N'U');";
            
            await using var cmd = _connection.CreateCommand();
            cmd.CommandText = existsSql;
            var res = await cmd.ExecuteScalarAsync();
            return !DBNull.Value.Equals(res);
        }

        public async Task<string> GetCurrentVersion()
        {
            var sql = $@"
SELECT TOP 1 [Version]
FROM [{SchemaName}].Version
ORDER BY id DESC
";
            await using var cmd = _connection.CreateCommand();
            cmd.CommandText = sql;
            var res = (string?) await cmd.ExecuteScalarAsync();
            
            return res ?? "0.0.0.0";
        }

        public async Task<long> VersionTheDatabase(string newVersion)
        {
            var sql = $@"
INSERT INTO [{SchemaName}].Version 
(version, entry_date, modified_date, entered_by)
VALUES(@newVersion, @entryDate, @modifiedDate, @enteredBy);

SELECT @@IDENTITY
";
            var res = (long) await  _connection.ExecuteAsync(
                sql, 
                new
                {
                    newVersion,
                    entryDate = DateTime.UtcNow,
                    modifiedDate = DateTime.UtcNow,
                    enteredBy = ClaimsPrincipal.Current?.Identity?.Name ?? Environment.UserName
                });
            
            _logger.LogInformation(" Versioning {0} database with version {1}.", DatabaseName, newVersion);
            
            return res;
        }

        public void Rollback()
        {
            _logger.LogInformation("TODO: Rollback");
        }

        public async Task RunSql(string sql, ConnectionType connectionType)
        {
            _logger.LogDebug("[SQL] Running (on connection '{0}'): {1}{2}", connectionType.ToString(), Environment.NewLine, sql);

            var conn = connectionType switch
            {
                ConnectionType.Default => _connection,
                ConnectionType.Admin => AdminConnection,
                _ => throw new ArgumentOutOfRangeException(nameof(connectionType), connectionType, "Unknown connection type: " + connectionType)
            };

            await using var cmd = conn.CreateCommand();
            cmd.CommandText = sql;
            cmd.CommandType = CommandType.Text;
            await cmd.ExecuteNonQueryAsync();
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

        public async Task InsertScriptRun(string scriptName, string sql, string hash, bool runOnce, object versionId)
        {
            var insertSql = $@"
INSERT INTO [{SchemaName}].ScriptsRun
(version_id, script_name, text_of_script, text_hash, one_time_script)
VALUES (@versionId, @scriptName, @sql, @hash, @runOnce)";
            
            var scriptRun = new 
            {
                versionId,
                scriptName,
                sql,
                hash,
                runOnce
            };

            await _connection.ExecuteAsync(insertSql, scriptRun);
            
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