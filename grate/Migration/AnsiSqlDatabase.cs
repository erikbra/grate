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
using static System.StringSplitOptions;

namespace grate.Migration;

public abstract class AnsiSqlDatabase : IDatabase
{
    private string SchemaName { get; set; } = "";
    protected ILogger Logger { get; }
    protected DbConnection? _connection;
    private DbConnection? _adminConnection;
    private readonly ISyntax _syntax;

    private IDictionary<string, string>? _scriptsRunCache;

    protected AnsiSqlDatabase(ILogger logger, ISyntax syntax)
    {
        Logger = logger;
        _syntax = syntax;
    }

    public string ServerName => Connection.DataSource;
    public virtual string DatabaseName => Connection.Database;

    //protected string? Password => Connection.ConnectionString.Split(";", TrimEntries | RemoveEmptyEntries)
    protected string? Password => ConnectionString?.Split(";", TrimEntries | RemoveEmptyEntries)
        .SingleOrDefault(entry => entry.StartsWith("Password"))?
        .Split("=", TrimEntries | RemoveEmptyEntries).Last();

    public abstract bool SupportsDdlTransactions { get; }
    public abstract bool SupportsSchemas { get; }
    public bool SplitBatchStatements => true;

    public string StatementSeparatorRegex => _syntax.StatementSeparatorRegex;

    public string ScriptsRunTable => _syntax.TableWithSchema(SchemaName, "ScriptsRun");
    public string ScriptsRunErrorsTable => _syntax.TableWithSchema(SchemaName, "ScriptsRunErrors");
    public string VersionTable => _syntax.TableWithSchema(SchemaName, "Version");

    public Task InitializeConnections(GrateConfiguration configuration)
    {
        Logger.LogInformation("Initializing connections.");

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

    public async Task OpenConnection() => await Open(Connection);
    // Don't use the properties, they can open a connection just to dispose it!
    public async Task CloseConnection() => await Close(_connection);

    public async Task OpenAdminConnection() => await Open(AdminConnection);
    // Don't use the properties, they can open a connection just to dispose it!
    public async Task CloseAdminConnection() => await Close(_adminConnection);

    public async Task CreateDatabase()
    {
        if (!await DatabaseExists())
        {
            Logger.LogTrace($"Creating database {DatabaseName}");
                
            using var s = new TransactionScope(TransactionScopeOption.Suppress, TransactionScopeAsyncFlowOption.Enabled);
            var sql = _syntax.CreateDatabase(DatabaseName, Password);

            await ExecuteNonQuery(AdminConnection, sql);
            s.Complete();
        }

        await CloseAdminConnection();
        await WaitUntilDatabaseIsReady();
    }

    public virtual async Task DropDatabase()
    {
        if (await DatabaseExists())
        {
            using var s = new TransactionScope(TransactionScopeOption.Suppress, TransactionScopeAsyncFlowOption.Enabled);
            await CloseConnection(); // try and ensure there's nobody else in there...
            await OpenAdminConnection();
            await ExecuteNonQuery(AdminConnection, _syntax.DropDatabase(DatabaseName));
            s.Complete();
        }
    }


    /// <summary>
    /// Gets whether the Database currently exists on the server or not.
    /// </summary>
    /// <returns></returns>
    public virtual async Task<bool> DatabaseExists()
    {
        var sql = _syntax.ListDatabases;

        try
        {
            await OpenConnection();
            var databases = await Connection.QueryAsync<string>(sql);
                
            Logger.LogTrace("Current databases: ");
            foreach (var db in databases)
            {
                Logger.LogTrace(" * " + db);
            }
                
            return databases.Contains(DatabaseName);
        }
        catch (DbException e)
        {
            Logger.LogDebug(e, "Got error: " + e.Message);
            return false;
        }
        finally
        {
            await CloseConnection();
        }
    }

    protected async Task WaitUntilDatabaseIsReady()
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
        using (var s = new TransactionScope(TransactionScopeOption.Suppress, TransactionScopeAsyncFlowOption.Enabled))
        {
            await CreateRunSchema();
            await CreateScriptsRunTable();
            await CreateScriptsRunErrorsTable();
            await CreateVersionTable();
            s.Complete();
        }
        await CloseConnection();
    }

    private async Task CreateRunSchema()
    {
        if (SupportsSchemas && !await RunSchemaExists())
        {
            await ExecuteNonQuery(Connection, _syntax.CreateSchema(SchemaName));
        }
    }

    private async Task<bool> RunSchemaExists()
    {
        string sql = $"SELECT s.schema_name FROM information_schema.schemata s WHERE s.schema_name = '{SchemaName}'";
        var res = await ExecuteScalarAsync<string>(Connection, sql, null);
        return res == SchemaName;
    }

    // TODO: Change MySQL/MariaDB from using schemas to using grate_ prefix

    protected virtual async Task CreateScriptsRunTable()
    {
        string createSql = $@"
CREATE TABLE {ScriptsRunTable}(
	{_syntax.PrimaryKeyColumn("id")},
	version_id {_syntax.BigintType} NULL,
	script_name {_syntax.VarcharType}(255) NULL,
	text_of_script {_syntax.TextType} NULL,
	text_hash {_syntax.VarcharType}(512) NULL,
	one_time_script {_syntax.BooleanType} NULL,
	entry_date {_syntax.TimestampType} NULL,
	modified_date {_syntax.TimestampType} NULL,
	entered_by {_syntax.VarcharType}(50) NULL
	{_syntax.PrimaryKeyConstraint("ScriptsRun","id")}
)";
            
        if (!await ScriptsRunTableExists())
        {
            await ExecuteNonQuery(Connection, createSql);
        }
    }

    protected virtual async Task CreateScriptsRunErrorsTable()
    {
        string createSql = $@"
CREATE TABLE {ScriptsRunErrorsTable}(
	{_syntax.PrimaryKeyColumn("id")},
	repository_path {_syntax.VarcharType}(255) NULL,
	version {_syntax.VarcharType}(50) NULL,
	script_name {_syntax.VarcharType}(255) NULL,
	text_of_script {_syntax.TextType} NULL,
	erroneous_part_of_script {_syntax.TextType} NULL,
	error_message {_syntax.TextType} NULL,
	entry_date {_syntax.TimestampType} NULL,
	modified_date {_syntax.TimestampType} NULL,
	entered_by {_syntax.VarcharType}(50) NULL
	{_syntax.PrimaryKeyConstraint("ScriptsRunErrors","id")}
)";
        if (!await ScriptsRunErrorsTableExists())
        {
            await ExecuteNonQuery(Connection, createSql);
        }
    }

    protected virtual async Task CreateVersionTable()
    {
        string createSql = $@"
CREATE TABLE {VersionTable}(
	{_syntax.PrimaryKeyColumn("id")},
	repository_path {_syntax.VarcharType}(255) NULL,
	version {_syntax.VarcharType}(50) NULL,
	entry_date {_syntax.TimestampType} NULL,
	modified_date {_syntax.TimestampType} NULL,
	entered_by {_syntax.VarcharType}(50) NULL
	{_syntax.PrimaryKeyConstraint("Version","id")}
)";
        if (!await VersionTableExists())
        {
            await ExecuteNonQuery(Connection, createSql);
        }
    }

    protected async Task<bool> ScriptsRunTableExists() => await TableExists(SchemaName, "ScriptsRun");
    protected async Task<bool> ScriptsRunErrorsTableExists() => await TableExists(SchemaName, "ScriptsRunErrors");
    protected async Task<bool> VersionTableExists() => await TableExists(SchemaName, "Version");

    private async Task<bool> TableExists(string schemaName, string tableName)
    {
        var fullTableName = SupportsSchemas ? tableName : _syntax.TableWithSchema(schemaName, tableName);
        var tableSchema = SupportsSchemas ? schemaName : DatabaseName;

        string existsSql = ExistsSql(tableSchema, fullTableName);

        var res = await ExecuteScalarAsync<object>(Connection, existsSql, null);
        return !DBNull.Value.Equals(res) && res is not null;
    }
        
    protected virtual string ExistsSql(string tableSchema, string fullTableName)
    {
        return $@"
SELECT * FROM information_schema.tables 
WHERE 
table_schema = '{tableSchema}' AND
table_name = '{fullTableName}'
";
    }
        
    protected virtual string CurrentVersionSql => $@"
SELECT 
{_syntax.LimitN($@"
version
FROM {VersionTable}
ORDER BY id DESC", 1)}
"; 

    public async Task<string> GetCurrentVersion()
    {
        var sql = CurrentVersionSql;
        var res = await ExecuteScalarAsync<string>(Connection, sql, null);
        return res ?? "0.0.0.0";
    }

    public virtual async Task<long> VersionTheDatabase(string newVersion)
    {
        var sql = Parameterize($@"
INSERT INTO {VersionTable}
(version, entry_date, modified_date, entered_by)
VALUES(@newVersion, @entryDate, @modifiedDate, @enteredBy)

{_syntax.ReturnId}
");
        var res = (long)await Connection.ExecuteScalarAsync<int>(
            sql,
            new
            {
                newVersion,
                entryDate = DateTime.UtcNow,
                modifiedDate = DateTime.UtcNow,
                enteredBy = ClaimsPrincipal.Current?.Identity?.Name ?? Environment.UserName
            });

        Logger.LogInformation(" Versioning {dbName} database with version {version}.", DatabaseName, newVersion);

        return res;
    }

    public void Rollback()
    {
        Logger.LogInformation("Rolling back changes.");
        Transaction.Current?.Rollback();
    }

    public async Task RunSql(string sql, ConnectionType connectionType)
    {
        Logger.LogTrace("[SQL] Running (on connection '{ConnType}'): \n{Sql}", connectionType.ToString(), sql);

        var conn = connectionType switch
        {
            ConnectionType.Default => Connection,
            ConnectionType.Admin => AdminConnection,
            _ => throw new ArgumentOutOfRangeException(nameof(connectionType), connectionType, "Unknown connection type: " + connectionType)
        };

        await ExecuteNonQuery(conn, sql);
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
FROM {ScriptsRunTable} sr
WHERE id = (SELECT MAX(id) FROM {ScriptsRunTable} sr2 WHERE sr2.script_name = sr.script_name)
";
        var results = await Connection.QueryAsync<ScriptsRunCacheItem>(sql);
        return results.ToDictionary(item => item.script_name, item => item.text_hash);
    }

    private async Task<IDictionary<string, string>> GetScriptsRunCache() => _scriptsRunCache ??= await GetAllScriptsRun();

    protected virtual string Parameterize(string sql) => sql;

    public async Task<string?> GetCurrentHash(string scriptName)
    {
        var cache = await GetScriptsRunCache();
        if (cache.ContainsKey(scriptName))
        {
            return cache[scriptName];
        }

        var hashSql = Parameterize($@"
SELECT text_hash FROM  {ScriptsRunTable}
WHERE script_name = @scriptName");

        var hash = await ExecuteScalarAsync<string?>(Connection,  hashSql, new { scriptName });
        return hash;
    }

    public async Task<bool> HasRun(string scriptName)
    {
        var cache = await GetScriptsRunCache();
        if (cache.ContainsKey(scriptName))
        {
            return true;
        }

        var hasRunSql = Parameterize($@"
SELECT 1 FROM  {ScriptsRunTable}
WHERE script_name = @scriptName");

        var run = await ExecuteScalarAsync<bool?>(Connection, hasRunSql, new { scriptName });
        return run ?? false;
    }

    protected virtual object Bool(bool source) => source;

    public async Task InsertScriptRun(string scriptName, string? sql, string hash, bool runOnce, long versionId)
    {
        var cache = await GetScriptsRunCache();
        cache.Remove(scriptName);

        var insertSql = Parameterize($@"
INSERT INTO {ScriptsRunTable}
(version_id, script_name, text_of_script, text_hash, one_time_script, entry_date, modified_date, entered_by)
VALUES (@versionId, @scriptName, @sql, @hash, @runOnce, @now, @now, @usr)");

        var scriptRun = new
        {
            versionId,
            scriptName,
            sql,
            hash,
            runOnce = Bool(runOnce),
            now = DateTime.UtcNow,
            usr = Environment.UserName
        };

        await ExecuteAsync(Connection, insertSql, scriptRun);
    }

    public async Task InsertScriptRunError(string scriptName, string? sql, string errorSql, string errorMessage, long versionId)
    {
        var insertSql = Parameterize($@"
INSERT INTO {ScriptsRunErrorsTable}
(version, script_name, text_of_script, erroneous_part_of_script, error_message, entry_date, modified_date, entered_by)
VALUES (@version, @scriptName, @sql, @errorSql, @errorMessage, @now, @now, @usr)");

        var versionSql = Parameterize($"SELECT version FROM {VersionTable} WHERE id = @versionId");
        var version = await ExecuteScalarAsync<string>(Connection, versionSql, new { versionId });
            
        var scriptRunErrors = new
        {
            version,
            scriptName,
            sql,
            errorSql,
            errorMessage,
            now = DateTime.UtcNow,
            usr = Environment.UserName,
        };

        using var s = new TransactionScope(TransactionScopeOption.Suppress, TransactionScopeAsyncFlowOption.Enabled);
        await ExecuteAsync(Connection, insertSql, scriptRunErrors);

        s.Complete();
    }

    private static async Task Close(DbConnection? conn)
    {
        if (conn?.State == ConnectionState.Open)
        {
            await conn.CloseAsync();
        }
    }
        
    protected virtual async Task Open(DbConnection? conn)
    {
        if (conn != null && conn.State != ConnectionState.Open)
        {
            await conn.OpenAsync();
            await conn.QueryAsync<string>(_syntax.CurrentDatabase);
        }
    }

    protected async Task<T?> ExecuteScalarAsync<T>(DbConnection conn, string sql, object? parameters = null)
    {
        Logger.LogTrace("SQL: {Sql}", sql);
        Logger.LogTrace("Parameters: {@Parameters}", parameters);
            
        return await conn.ExecuteScalarAsync<T?>(sql, parameters);
    }

    protected async Task<int> ExecuteAsync(DbConnection conn, string sql, object? parameters = null)
    {
        Logger.LogTrace("SQL: {Sql}", sql);
        Logger.LogTrace("Parameters: {@Parameters}", parameters);
            
        return await conn.ExecuteAsync(sql, parameters);
    }

    protected async Task ExecuteNonQuery(DbConnection conn, string sql)
    {
        Logger.LogTrace("SQL: {Sql}", sql);
        
        await using var cmd = conn.CreateCommand();
        cmd.CommandText = sql;
        cmd.CommandType = CommandType.Text;
        await cmd.ExecuteNonQueryAsync();
    }

    public async ValueTask DisposeAsync()
    {
        await CloseConnection();
        await CloseAdminConnection();
            
        GC.SuppressFinalize(this);
    }

    public abstract Task RestoreDatabase(string backupPath);
}