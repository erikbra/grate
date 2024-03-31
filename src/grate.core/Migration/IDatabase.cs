using System.Data.Common;
using grate.Configuration;

namespace grate.Migration;

public interface IDatabase : IAsyncDisposable, ICloneable
{
    string? ServerName { get; }
    string? DatabaseName { get; }
    string DatabaseType { get; }
    string MasterDatabaseName { get; }
    bool SupportsDdlTransactions { get; }

    bool SplitBatchStatements { get; }
    public string ScriptsRunTable { get; }
    public string ScriptsRunErrorsTable { get; }
    public string VersionTable { get; }
    DbConnection ActiveConnection { set; }
    bool SupportsSchemas { get; }

    Task InitializeConnections(GrateConfiguration configuration);
    Task OpenConnection();
    Task CloseConnection();
    Task OpenAdminConnection();
    Task CloseAdminConnection();
    Task CreateDatabase();
    Task RestoreDatabase(string backupPath);

    /// <summary>
    /// Drops the database if it exists, and does nothing if it doesn't.
    /// </summary>
    /// <returns></returns>
    Task DropDatabase();

    Task<bool> DatabaseExists();
    Task<string> GetCurrentVersion();
    Task<long> VersionTheDatabase(string newVersion);
    void Rollback();
    Task RunSql(string sql, ConnectionType connectionType, TransactionHandling transactionHandling);
    Task<string?> GetCurrentHash(string scriptName);
    Task<bool> HasRun(string scriptName, TransactionHandling transactionHandling);
    Task InsertScriptRun(string scriptName, string? sql, string hash, bool runOnce, long versionId,
        TransactionHandling transactionHandling);
    Task InsertScriptRunError(string scriptName, string? sql, string errorSql, string errorMessage, long versionId);
    Task<bool> VersionTableExists();
    Task ChangeVersionStatus(string status, long versionId);
    Task DeleteVersionRecord(long versionId);
    void SetDefaultConnectionActive();
    Task<IDisposable> OpenNewActiveConnection();
    Task OpenActiveConnection();
    Task<string?> ExistingTable(string schemaName, string tableName);

    /// <summary>
    /// Split a sql into multiple statements (if supported), or just returns the whole sql in one item, if not supported
    /// </summary>
    /// <param name="sql"></param>
    /// <returns></returns>
    IEnumerable<string> GetStatements(string sql);

    void ThrowScriptFailed(MigrationsFolder folder, string file, string? scriptText, Exception exception);
}
