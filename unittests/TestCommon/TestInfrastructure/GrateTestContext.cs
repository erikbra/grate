using System.Data;
using grate.Infrastructure;
using grate.Migration;

namespace TestCommon.TestInfrastructure;

public abstract class GrateTestContext(IGrateMigrator migrator, ITestDatabase testDatabase) : IGrateTestContext, IAsyncLifetime
{
    public IGrateMigrator Migrator { get; } = migrator;
    protected ITestDatabase TestDatabase { get; } = testDatabase;

    public string AdminConnectionString => TestDatabase.AdminConnectionString;
    public string ConnectionString(string database) => TestDatabase.ConnectionString(database);
    public string UserConnectionString(string database) => TestDatabase.UserConnectionString(database);
    
    public async Task InitializeAsync()
    {
        if (TestDatabase is IAsyncLifetime asyncLifetime)
        {
            await asyncLifetime.InitializeAsync();
        }
    }

    public async Task DisposeAsync()
    {
        if (TestDatabase is IAsyncLifetime asyncLifetime)
        {
            await asyncLifetime.DisposeAsync();
        }
    }
    
    public abstract ISyntax Syntax { get; }
    public abstract Type DbExceptionType { get; }
    public abstract Type DatabaseType { get; }
    public abstract bool SupportsTransaction { get; }
    public abstract SqlStatements Sql { get; }
    public abstract string ExpectedVersionPrefix { get; }
    public abstract bool SupportsCreateDatabase { get; }
    public abstract bool SupportsSchemas { get; }
    public abstract IDbConnection GetDbConnection(string connectionString);


}
