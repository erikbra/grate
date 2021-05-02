using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Transactions;
using Dapper;
using grate.Configuration;
using grate.Infrastructure;
using Microsoft.Extensions.Logging;

namespace grate.Migration
{
    public abstract class AnsiSqlDatabase : IDatabase, IDisposable
    {
        private string SchemaName { get; set; } = ""; 
        private readonly ILogger _logger;
        private DbConnection? _connection;
        private DbConnection? _adminConnection;
        private readonly ISyntax _syntax;

        private IDictionary<string, string>? _scriptsRunCache;

        protected AnsiSqlDatabase(ILogger logger, ISyntax syntax)
        {
            _logger = logger;
            _syntax = syntax;
        }

        public string? ServerName => Connection?.DataSource;
        public string? DatabaseName => Connection?.Database;
        
        public bool SupportsDdlTransactions => true;
        public bool SplitBatchStatements => true;

        public string StatementSeparatorRegex => _syntax.StatementSeparatorRegex;
     
        
        public Task InitializeConnections(GrateConfiguration configuration)
        {
            _logger.LogInformation("Initializing connections.");

            ConnectionString = configuration.ConnectionString;
            AdminConnectionString = configuration.AdminConnectionString;
            SchemaName = configuration.SchemaName;
            
            return Task.CompletedTask;
        }

        private string? AdminConnectionString { get; set; }
        private string? ConnectionString { get; set; }

        protected abstract DbConnection GetSqlConnection(string? connectionString);

        protected DbConnection AdminConnection => _adminConnection ??= GetSqlConnection(AdminConnectionString);
        protected DbConnection Connection => _connection ??= GetSqlConnection(ConnectionString);

        public async Task OpenConnection()
        {
            if (Connection.State != ConnectionState.Open)
            {
                await Connection.OpenAsync();
                await Connection.QueryAsync<string>(_syntax.CurrentDatabase);
            }
        }

        public async Task CloseConnection()
        {
            if (Connection.State == ConnectionState.Open)
            {
                await Connection.CloseAsync();
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
            string? sql = _syntax.ListDatabases;
            
            await OpenAdminConnection();
            var databases = await AdminConnection.QueryAsync<string>(sql);

            if (!databases.Contains(DatabaseName))
            {
                using var s = new TransactionScope(TransactionScopeOption.Suppress, TransactionScopeAsyncFlowOption.Enabled);
                var cmd = AdminConnection.CreateCommand();
                cmd.CommandText = $"CREATE database \"{DatabaseName}\"";
                await cmd.ExecuteNonQueryAsync();
                s.Complete();
            }
            
            await CloseAdminConnection();
            await WaitUntilDatabaseIsReady();
        }

        private async Task WaitUntilDatabaseIsReady()
        {
            const int maxDelay = 10_000;
            int totalDelay = 0;
            
            var databaseReady = false;
            do
            {
                try
                {
                    await OpenConnection();
                    databaseReady = true;
                }
                catch (DbException)
                {
                    await Task.Delay(1000);
                    totalDelay += 1000;
                }
            } while (!databaseReady && totalDelay < maxDelay);
        }

        public async Task RunSupportTasks()
        {
            using var s = new TransactionScope(TransactionScopeOption.Suppress, TransactionScopeAsyncFlowOption.Enabled);
            await CreateRunSchema();
            await CreateScriptsRunTable();
            await CreateScriptsRunErrorsTable();
            await CreateVersionTable();
            s.Complete();
            await CloseConnection();
        }

        private async Task CreateRunSchema()
        {
            string createSql = @$"CREATE SCHEMA ""{SchemaName}"";";
            
            if (!await RunSchemaExists())
            {
                await using var cmd = Connection.CreateCommand();
                cmd.CommandText = createSql;
                await cmd.ExecuteNonQueryAsync();
            }
        }

        private async Task<bool> RunSchemaExists()
        {
            string sql = $"SELECT s.schema_name FROM information_schema.schemata s WHERE s.schema_name = '{SchemaName}'";
            await using var cmd = Connection.CreateCommand();
            cmd.CommandText = sql;
            var res = await cmd.ExecuteScalarAsync();
            return res?.ToString() == SchemaName;
        }

        private async Task CreateScriptsRunTable()
        {
            string createSql = $@"
CREATE TABLE {SchemaName}.""ScriptsRun""(
	id bigint {_syntax.Identity} NOT NULL,
	version_id bigint NULL,
	script_name {_syntax.VarcharType}(255) NULL,
	text_of_script {_syntax.TextType} NULL,
	text_hash {_syntax.VarcharType}(512) NULL,
	one_time_script {_syntax.BooleanType} NULL,
	entry_date {_syntax.TimestampType} NULL,
	modified_date {_syntax.TimestampType} NULL,
	entered_by {_syntax.VarcharType}(50) NULL,
    CONSTRAINT PK_ScriptsRun_Id {_syntax.PrimaryKey("id")}
);";
            if (!await ScriptsRunTableExists())
            {
                await using var cmd = Connection.CreateCommand();
                cmd.CommandText = createSql;
                var res = await cmd.ExecuteNonQueryAsync();
            }
        }
        
        private async Task CreateScriptsRunErrorsTable()
        {
            string createSql = $@"
CREATE TABLE {SchemaName}.""ScriptsRunErrors""(
	id bigint {_syntax.Identity} NOT NULL,
	repository_path {_syntax.VarcharType}(255) NULL,
	version {_syntax.VarcharType}(50) NULL,
	script_name {_syntax.VarcharType}(255) NULL,
	text_of_script {_syntax.TextType} NULL,
	erroneous_part_of_script {_syntax.TextType} NULL,
	error_message {_syntax.TextType} NULL,
	entry_date {_syntax.TimestampType} NULL,
	modified_date {_syntax.TimestampType} NULL,
	entered_by {_syntax.VarcharType}(50) NULL,
    CONSTRAINT PK_ScriptsRunErrors_Id {_syntax.PrimaryKey("id")}
);";
            if (!await ScriptsRunErrorsTableExists())
            {
                await using var cmd = Connection.CreateCommand();
                cmd.CommandText = createSql;
                await cmd.ExecuteNonQueryAsync();
            }
        }
        
        private async Task CreateVersionTable()
        {
            string createSql = $@"
CREATE TABLE {SchemaName}.""Version""(
	id bigint {_syntax.Identity} NOT NULL,
	repository_path {_syntax.VarcharType}(255) NULL,
	version {_syntax.VarcharType}(50) NULL,
	entry_date {_syntax.TimestampType} NULL,
	modified_date {_syntax.TimestampType} NULL,
	entered_by {_syntax.VarcharType}(50) NULL,
    CONSTRAINT PK_Version_Id {_syntax.PrimaryKey("id")}
);";
            if (!await VersionTableExists())
            {
                await using var cmd = Connection.CreateCommand();
                cmd.CommandText = createSql;
                await cmd.ExecuteNonQueryAsync();
            }
        }

        private async Task<bool> ScriptsRunTableExists() => await TableExists("ScriptsRun");
        private async Task<bool> ScriptsRunErrorsTableExists() => await TableExists("ScriptsRunErrors");
        private async Task<bool> VersionTableExists() => await TableExists("Version");
        
        
        private async Task<bool> TableExists(string tableName)
        {
            string existsSql = $@"
SELECT * FROM information_schema.tables 
WHERE table_schema = '{SchemaName}'
AND table_name = '{tableName}'
";
            
            //string existsSql = $@"SELECT OBJECT_ID(N'[{SchemaName}].[{tableName}]', N'U');";
            
            await using var cmd = Connection.CreateCommand();
            cmd.CommandText = existsSql;
            var res = await cmd.ExecuteScalarAsync();
            return !DBNull.Value.Equals(res) && res is not null;
        }
        
        public async Task<string> GetCurrentVersion()
        {
            var sql = $@"
SELECT 
{_syntax.LimitN($@"
version
FROM {SchemaName}.""Version""
ORDER BY id DESC", 1)}
";
            await using var cmd = Connection.CreateCommand();
            cmd.CommandText = sql;
            var res = (string?) await cmd.ExecuteScalarAsync();
            
            return res ?? "0.0.0.0";
        }

        public async Task<long> VersionTheDatabase(string newVersion)
        {
            var sql = $@"
INSERT INTO {SchemaName}.""Version""
(version, entry_date, modified_date, entered_by)
VALUES(@newVersion, @entryDate, @modifiedDate, @enteredBy)

{_syntax.ReturnId}
";
            var res = (long) await  Connection.ExecuteAsync(
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
            _logger.LogInformation("Rolling back changes.");
            Transaction.Current?.Rollback();
        }

        public async Task RunSql(string sql, ConnectionType connectionType)
        {
            _logger.LogDebug("[SQL] Running (on connection '{0}'): {1}{2}", connectionType.ToString(), Environment.NewLine, sql);

            var conn = connectionType switch
            {
                ConnectionType.Default => Connection,
                ConnectionType.Admin => AdminConnection,
                _ => throw new ArgumentOutOfRangeException(nameof(connectionType), connectionType, "Unknown connection type: " + connectionType)
            };

            await using var cmd = conn.CreateCommand();
            cmd.CommandText = sql;
            cmd.CommandType = CommandType.Text;
            await cmd.ExecuteNonQueryAsync();
        }

        // ReSharper disable once ClassNeverInstantiated.Local
        private class ScriptsRunCacheItem
        {
#pragma warning disable 8618
            // ReSharper disable InconsistentNaming
            // ReSharper disable UnusedAutoPropertyAccessor.Local
            
            public string script_name { get; init; }
            public string text_hash { get; init; }
            
            // ReSharper restore InconsistentNaming
            // ReSharper restore UnusedAutoPropertyAccessor.Local
#pragma warning restore 8618
        }

        private async Task<IDictionary<string, string>> GetAllScriptsRun()
        {
            var sql = $@"
SELECT script_name, text_hash
FROM {SchemaName}.""ScriptsRun"" sr
WHERE id = (SELECT MAX(id) FROM {SchemaName}.""ScriptsRun"" sr2 WHERE sr2.script_name = sr.script_name)
";
            var results = await Connection.QueryAsync<ScriptsRunCacheItem>(sql);
            return results.ToDictionary(item => item.script_name, item => item.text_hash);
        }

        private async Task<IDictionary<string, string>> GetScriptsRunCache() => _scriptsRunCache ??= await GetAllScriptsRun();

        public async Task<string?> GetCurrentHash(string scriptName)
        {
            var cache = await GetScriptsRunCache();
            if (cache.ContainsKey(scriptName))
            {
                return cache[scriptName];
            }
            
            var hashSql = $@"
SELECT text_hash FROM  {SchemaName}.""ScriptsRun""
WHERE script_name = @scriptName";
            
            var hash = await Connection.ExecuteScalarAsync<string?>(hashSql, new {scriptName});
            return hash;
        }

        public async Task<bool> HasRun(string scriptName)
        {
            var cache = await GetScriptsRunCache();
            if (cache.ContainsKey(scriptName))
            {
                return true;
            }
            
            var hasRunSql = $@"
SELECT 1 FROM  {SchemaName}.""ScriptsRun""
WHERE script_name = @scriptName";

            var run = await Connection.ExecuteScalarAsync<bool?>(hasRunSql, new {scriptName});
            return run ?? false;
        }

        public async Task InsertScriptRun(string scriptName, string sql, string hash, bool runOnce, object versionId)
        {
            var cache = await GetScriptsRunCache();
            cache.Remove(scriptName);
            
            var insertSql = $@"
INSERT INTO {SchemaName}.""ScriptsRun""
(version_id, script_name, text_of_script, text_hash, one_time_script, entry_date, modified_date, entered_by)
VALUES (@versionId, @scriptName, @sql, @hash, @runOnce, @now, @now, @user)";
            
            var scriptRun = new 
            {
                versionId,
                scriptName,
                sql,
                hash,
                runOnce,
                now = DateTime.UtcNow,
                user = Environment.UserName
            };

            await Connection.ExecuteAsync(insertSql, scriptRun);
        }

        public async Task InsertScriptRunError(string scriptName, string sql, string errorSql, string errorMessage, long versionId)
        {
            var insertSql = $@"
INSERT INTO {SchemaName}.""ScriptsRunErrors""
(version, script_name, text_of_script, erroneous_part_of_script, error_message, entry_date, modified_date, entered_by)
VALUES ((SELECT version FROM {SchemaName}.""Version"" WHERE id = @versionId), @scriptName, @sql, @errorSql, @errorMessage, @now, @now, @user)";
            
            var scriptRunErrors = new 
            {
                versionId,
                scriptName,
                sql,
                errorSql,
                errorMessage,
                now = DateTime.UtcNow,
                user = Environment.UserName
            };
            
            using var s = new TransactionScope(TransactionScopeOption.Suppress, TransactionScopeAsyncFlowOption.Enabled);
            await Connection.ExecuteAsync(insertSql, scriptRunErrors);
            
            s.Complete();
        }

        public void Dispose()
        {
            Connection.Dispose();
        }
    }
}