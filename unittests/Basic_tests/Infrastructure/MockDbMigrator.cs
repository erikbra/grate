using grate.Configuration;
using grate.Infrastructure;
using grate.Migration;
using grate.Sqlite.Migration;
using NSubstitute;

namespace Basic_tests.Infrastructure;

public class MockDbMigrator: IDbMigrator
{
    public ValueTask DisposeAsync()
    {
        throw new NotImplementedException();
    }

    public object Clone()
    {
        throw new NotImplementedException();
    }

    public GrateConfiguration Configuration { get; set; } = null!;
    public IDatabase Database { get; set; } = Substitute.For<IDatabase>();
    public  Task InitializeConnections()
    {
        return Task.CompletedTask;
    }

    public Task<bool> CreateDatabase()
    {
        return Task.FromResult(false);
    }

    public Task DropDatabase()
    {
        throw new NotImplementedException();
    }

    public Task<bool> DatabaseExists()
    {
        return Task.FromResult(false);
    }

    public Task OpenConnection()
    {
        throw new NotImplementedException();
    }

    public Task CloseConnection()
    {
        return Task.CompletedTask;
    }

    public Task RunSupportTasks()
    {
        return Task.CompletedTask;
    }

    public Task<string> GetCurrentVersion()
    {
        return Task.FromResult(string.Empty);
    }

    public Task<long> VersionTheDatabase(string newVersion)
    {
        return Task.FromResult(0L);
    }

    public Task OpenAdminConnection()
    {
        throw new NotImplementedException();
    }

    public Task CloseAdminConnection()
    {
        throw new NotImplementedException();
    }

    public Task<bool> RunSql(string sql, string scriptName, MigrationType migrationType, long versionId, GrateEnvironment? environment,
        ConnectionType connectionType, TransactionHandling transactionHandling)
    {
        return Task.FromResult(false);
    }

    public Task<bool> RunSqlWithoutLogging(string sql, string scriptName, GrateEnvironment? environment, ConnectionType connectionType,
        TransactionHandling transactionHandling)
    {
        throw new NotImplementedException();
    }

    public Task RestoreDatabase(string backupPath)
    {
        throw new NotImplementedException();
    }

    public void SetDefaultConnectionActive()
    {
        //throw new NotImplementedException();
    }

    public Task<IDisposable> OpenNewActiveConnection()
    {
        return Task.FromResult((IDisposable)null!);
    }

    public Task OpenActiveConnection()
    {
        return Task.CompletedTask;
    }
}
