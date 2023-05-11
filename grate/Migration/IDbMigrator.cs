using System;
using System.Threading.Tasks;
using grate.Configuration;
using grate.Infrastructure;

namespace grate.Migration;

public interface IDbMigrator: IAsyncDisposable
{
    GrateConfiguration Configuration { get; set; }
    IDatabase Database { get; set; }
    Task InitializeConnections();
    Task<bool> CreateDatabase();
        
    /// <summary>
    /// Requests a database drop if configuration allows it.
    /// </summary>
    /// <returns>Returns whether the database was actually dropped or not.</returns>
    Task DropDatabase();
        
    Task<bool> DatabaseExists();
    Task OpenConnection();
    Task CloseConnection();
    Task RunSupportTasks();
    Task<string> GetCurrentVersion();
    Task<long> VersionTheDatabase(string newVersion);
    Task OpenAdminConnection();
    Task CloseAdminConnection();
    
    Task<bool> RunSql(string sql, string scriptName, MigrationType migrationType, long versionId,
        GrateEnvironment? environment,
        ConnectionType connectionType, TransactionHandling transactionHandling);

    Task<bool> RunSqlWithoutLogging(string sql, string scriptName,
        GrateEnvironment? environment,
        ConnectionType connectionType, TransactionHandling transactionHandling);

    Task RestoreDatabase(string backupPath);
    void SetDefaultConnectionActive();
    Task<IDisposable> OpenNewActiveConnection();
    Task OpenActiveConnection();
}
