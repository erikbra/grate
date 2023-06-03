using System;
using System.Data.Common;
using System.Threading.Tasks;
using grate.Configuration;

namespace grate.Migration;

public interface IDatabase : IAsyncDisposable
{
    string? ServerName { get; }
    string? DatabaseName { get; }
    bool SupportsDdlTransactions { get; }

    bool SplitBatchStatements { get; }
    string StatementSeparatorRegex { get; }
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
    Task RunSupportTasks();
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
    void SetDefaultConnectionActive();
    Task<IDisposable> OpenNewActiveConnection();
    Task OpenActiveConnection();
    Task<string?> ExistingTable(string schemaName, string tableName);
}
