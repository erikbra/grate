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
using grate.Exceptions;
using grate.Infrastructure;
using Microsoft.Extensions.Logging;
using static System.StringSplitOptions;
using IsolationLevel = System.Transactions.IsolationLevel;

namespace grate.Migration;

public abstract class AnsiSqlDatabase : IDatabase
{
    private string SchemaName { get; set; } = "";

    protected GrateConfiguration? Config { get; private set; }

    protected ILogger Logger { get; }
    private DbConnection? _connection;
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

    private string? Password => ConnectionString?.Split(";", TrimEntries | RemoveEmptyEntries)
        .SingleOrDefault(entry => entry.StartsWith("Password"))?
        .Split("=", TrimEntries | RemoveEmptyEntries).Last();

    public abstract bool SupportsDdlTransactions { get; }
    protected abstract bool SupportsSchemas { get; }
    public bool SplitBatchStatements => true;

    public string StatementSeparatorRegex => _syntax.StatementSeparatorRegex;

    public string ScriptsRunTable => _syntax.TableWithSchema(SchemaName, "ScriptsRun");
    public string ScriptsRunErrorsTable => _syntax.TableWithSchema(SchemaName, "ScriptsRunErrors");
    public string VersionTable => _syntax.TableWithSchema(SchemaName, "Version");

    public virtual Task InitializeConnections(GrateConfiguration configuration)
    {
        Logger.LogInformation("Initializing connections.");

        ConnectionString = configuration.ConnectionString;
        AdminConnectionString = configuration.AdminConnectionString;
        SchemaName = configuration.SchemaName;
        Config = configuration;
        return Task.CompletedTask;
    }

    private string? AdminConnectionString { get; set; }
    protected string? ConnectionString { get; set; }

    protected abstract DbConnection GetSqlConnection(string? connectionString);

    protected DbConnection AdminConnection => _adminConnection ??= GetSqlConnection(AdminConnectionString);
    protected DbConnection Connection => _connection ??= GetSqlConnection(ConnectionString);

    protected async Task<TResult> RunInAutonomousTransaction<TResult>(string? connectionString, Func<DbConnection, Task<TResult>> func)
    {
        TResult res;
        using (var s = new TransactionScope(
                   TransactionScopeOption.Suppress, 
                   new TransactionOptions() { IsolationLevel = IsolationLevel.ReadUncommitted} , 
                   TransactionScopeAsyncFlowOption.Enabled))
        {
            await using (var connection = GetSqlConnection(connectionString))
            {
                await Open(connection);
                res = await func(connection);
            }
            s.Complete();
        }
        return res;
    }
    
    protected async Task RunInAutonomousTransaction(string? connectionString, Func<DbConnection, Task> func)
    {
        using (var s = new TransactionScope(TransactionScopeOption.Suppress, TransactionScopeAsyncFlowOption.Enabled))
        {
            await using (var connection = GetSqlConnection(connectionString))
            {
                await Open(connection);
                await func(connection);
            }
            s.Complete();
        }
    }

    public async Task OpenConnection() => await Open(Connection);
    // Don't use the properties, they can open a connection just to dispose it!
    public async Task CloseConnection()
    {
        await Close(_connection);
        _connection = null;
    }

    public async Task OpenAdminConnection() => await Open(AdminConnection);
    // Don't use the properties, they can open a connection just to dispose it!
    public async Task CloseAdminConnection()
    {
        await Close(_adminConnection);
        _adminConnection = null;
    }

    protected async Task<DbConnection> OpenNewAdminConnection()
    {
        var conn = GetSqlConnection(AdminConnectionString);
        await Open(conn);
        return conn;
    }


    public async Task CreateDatabase()
    {
        if (!await DatabaseExists())
        {
            Logger.LogTrace("Creating database {DatabaseName}", DatabaseName);
            
            var sql = _syntax.CreateDatabase(DatabaseName, Password);
            await RunInAutonomousTransaction(AdminConnectionString, async conn => await ExecuteNonQuery(conn, sql, Config?.AdminCommandTimeout));
        }

        await CloseAdminConnection();
        await WaitUntilDatabaseIsReady();
    }

    public virtual async Task DropDatabase()
    {
        if (await DatabaseExists())
        {
            await CloseConnection(); // try and ensure there's nobody else in there...
            await RunInAutonomousTransaction(AdminConnectionString, async conn => await ExecuteNonQuery(conn, _syntax.DropDatabase(DatabaseName), Config?.AdminCommandTimeout));
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
            var databases = (await RunInAutonomousTransaction(ConnectionString, async conn => await conn.QueryAsync<string>(sql))).ToArray();

            Logger.LogTrace("Current databases: ");
            foreach (var db in databases)
            {
                Logger.LogTrace(" * {Database}", db);
            }

            return databases.Contains(DatabaseName);
        }
        catch (DbException e)
        {
            Logger.LogDebug(e, "Got error: {ErrorMessage}", e.Message);
            return false;
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
                _ = await RunInAutonomousTransaction(ConnectionString, async conn => await Task.FromResult(1));
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
            await CreateRunSchema();
            await CreateScriptsRunTable();
            await CreateScriptsRunErrorsTable();
            await CreateVersionTable();
    }

    private async Task CreateRunSchema()
    {
        if (SupportsSchemas && !await RunSchemaExists())
        {
            await RunInAutonomousTransaction(ConnectionString, async conn => await ExecuteNonQuery(conn, _syntax.CreateSchema(SchemaName), Config?.CommandTimeout));
        }
    }

    private async Task<bool> RunSchemaExists()
    {
        string sql = $"SELECT s.schema_name FROM information_schema.schemata s WHERE s.schema_name = '{SchemaName}'";
        var res = await RunInAutonomousTransaction(ConnectionString, async conn => await ExecuteScalarAsync<string>(conn, sql));
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
	{_syntax.PrimaryKeyConstraint("ScriptsRun", "id")}
)";

        if (!await ScriptsRunTableExists())
        {
            await RunInAutonomousTransaction(ConnectionString, async conn => await ExecuteNonQuery(conn, createSql, Config?.CommandTimeout));
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
	{_syntax.PrimaryKeyConstraint("ScriptsRunErrors", "id")}
)";
        if (!await ScriptsRunErrorsTableExists())
        {
            await RunInAutonomousTransaction(ConnectionString, async conn => await ExecuteNonQuery(conn, createSql, Config?.CommandTimeout));
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
	{_syntax.PrimaryKeyConstraint("Version", "id")}
)";
        if (!await VersionTableExists())
        {
            await RunInAutonomousTransaction(ConnectionString, async conn => await ExecuteNonQuery(conn, createSql, Config?.CommandTimeout));
        }
    }

    protected async Task<bool> ScriptsRunTableExists() => await TableExists(SchemaName, "ScriptsRun");
    protected async Task<bool> ScriptsRunErrorsTableExists() => await TableExists(SchemaName, "ScriptsRunErrors");
    public async Task<bool> VersionTableExists() => await TableExists(SchemaName, "Version");

    public async Task<bool> TableExists(string schemaName, string tableName)
    {
        var fullTableName = SupportsSchemas ? tableName : _syntax.TableWithSchema(schemaName, tableName);
        var tableSchema = SupportsSchemas ? schemaName : DatabaseName;

        string existsSql = ExistsSql(tableSchema, fullTableName);

        var res = await RunInAutonomousTransaction(ConnectionString, async conn => await ExecuteScalarAsync<object>(conn, existsSql));
        
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
        try
        {
            var sql = CurrentVersionSql;
            var res = await RunInAutonomousTransaction(ConnectionString, async conn => await ExecuteScalarAsync<string>(conn, sql));
            return res ?? "0.0.0.0";
        }
        catch (Exception ex)
        {
            Logger.LogDebug(ex, "An error occurred getting the current database version, new database + --dryrun?");
            return "0.0.0.0";
        }
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

        Logger.LogInformation(" Versioning {DbName} database with version {Version}.", DatabaseName, newVersion);

        return res;
    }

    public void Rollback()
    {
        Logger.LogInformation("Rolling back changes.");
        Transaction.Current?.Rollback();
    }

    public async Task RunSql(string sql, ConnectionType connectionType, TransactionHandling transactionHandling)
    {
        Logger.LogTrace("[SQL] Running (on connection '{ConnType}', transaction handling '{TransactionHandling}'): \n{Sql}", 
                            connectionType.ToString(), 
                            transactionHandling,
                            sql);

        int? timeout = GetTimeout(connectionType);
        var connection = GetDbConnection(connectionType);
        var connectionString = GetConnectionString(connectionType);

        var task = transactionHandling switch
        {
            TransactionHandling.Default => ExecuteNonQuery(connection, sql, timeout),
            TransactionHandling.Autonomous => RunInAutonomousTransaction(connectionString, async conn => await ExecuteNonQuery(conn, sql, timeout)),
            _ => throw new UnknownConnectionType(connectionType)
        };
        
        await task;
    }

    
    private DbConnection GetDbConnection(ConnectionType connectionType) => 
        connectionType switch
        {
            ConnectionType.Default => Connection,
            ConnectionType.Admin => AdminConnection,
            _ => throw new UnknownConnectionType(connectionType)
        };
    
    private string? GetConnectionString(ConnectionType connectionType) => 
        connectionType switch
        {
            ConnectionType.Default => ConnectionString,
            ConnectionType.Admin => AdminConnectionString,
            _ => throw new UnknownConnectionType(connectionType)
        };
    
    private int? GetTimeout(ConnectionType connectionType) => 
        connectionType switch
        {
            ConnectionType.Default => Config?.CommandTimeout,
            ConnectionType.Admin => Config?.AdminCommandTimeout,
            _ => throw new UnknownConnectionType(connectionType)
        };

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

        try
        {

            var sql = $@"
SELECT script_name, text_hash
FROM {ScriptsRunTable} sr
WHERE id = (SELECT MAX(id) FROM {ScriptsRunTable} sr2 WHERE sr2.script_name = sr.script_name)
";
            var results = await Connection.QueryAsync<ScriptsRunCacheItem>(sql);
            return results.ToDictionary(item => item.script_name, item => item.text_hash);

        }
        catch (Exception ex) when (Config?.DryRun ?? throw new InvalidOperationException("No configuration available."))
        {
            Logger.LogDebug(ex, "Ignoring error getting ScriptsRun when in --dryrun, probable missing table");
            return new Dictionary<string, string>(); // return empty set if nothing has ever been run
        }
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

        var hash = await ExecuteScalarAsync<string?>(Connection, hashSql, new { scriptName });
        return hash;
    }

    public async Task<bool> HasRun(string scriptName, TransactionHandling transactionHandling)
    {
        var cache = await GetScriptsRunCache();
        if (cache.ContainsKey(scriptName))
        {
            return true;
        }

        try
        {
            var hasRunSql = Parameterize($@"
SELECT 1 FROM  {ScriptsRunTable}
WHERE script_name = @scriptName");

            var task = transactionHandling switch
            {
                TransactionHandling.Default => ExecuteScalarAsync<bool?>(Connection, hasRunSql, new { scriptName }),
                TransactionHandling.Autonomous => RunInAutonomousTransaction(ConnectionString,
                    async conn => await ExecuteScalarAsync<bool?>(conn, hasRunSql, new { scriptName })),
                _ => throw new UnknownTransactionHandling(transactionHandling)
            };

            var run = await task;
            return run ?? false;
        }
        catch (Exception ex) when (Config?.DryRun ?? throw new InvalidOperationException("No configuration available"))
        {
            Logger.LogDebug(ex, "Ignoring exception in dryrun, missing table?");
            return false;
        }
    }

    protected virtual object Bool(bool source) => source;

    public async Task InsertScriptRun(string scriptName, string? sql, string hash, bool runOnce, long versionId,
        TransactionHandling transactionHandling)
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

        var execution = transactionHandling switch
        {
            TransactionHandling.Default => ExecuteAsync(Connection, insertSql, scriptRun),
            TransactionHandling.Autonomous => RunInAutonomousTransaction(ConnectionString,
                conn => ExecuteAsync(conn, insertSql, scriptRun)),
            _ => throw new UnknownTransactionHandling(transactionHandling)
        };

        await execution;
    }

    public async Task InsertScriptRunError(string scriptName, string? sql, string errorSql, string errorMessage, long versionId)
    {
        var insertSql = Parameterize($@"
INSERT INTO {ScriptsRunErrorsTable}
(version, script_name, text_of_script, erroneous_part_of_script, error_message, entry_date, modified_date, entered_by)
VALUES (@version, @scriptName, @sql, @errorSql, @errorMessage, @now, @now, @usr)");

        var versionSql = Parameterize($"SELECT version FROM {VersionTable} WHERE id = @versionId");

        await RunInAutonomousTransaction(ConnectionString, async conn =>
            {
                var version = await ExecuteScalarAsync<string>(conn, versionSql, new { versionId });

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

                await ExecuteAsync(conn, insertSql, scriptRunErrors);
        });
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

    protected async Task ExecuteNonQuery(DbConnection conn, string sql, int? timeout)
    {
        Logger.LogTrace("SQL: {Sql}", sql);

        await using var cmd = conn.CreateCommand();
        cmd.CommandText = sql;
        cmd.CommandType = CommandType.Text;

        if (timeout.HasValue)
        {
            cmd.CommandTimeout = timeout.Value;
        }

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
