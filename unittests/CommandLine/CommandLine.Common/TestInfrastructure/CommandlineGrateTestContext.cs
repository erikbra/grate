using System.Data;
using grate.Infrastructure;
using grate.Migration;
using TestCommon.TestInfrastructure;

namespace CommandLine.Common.TestInfrastructure;

public class CommandLineGrateTestContext : IGrateTestContext
{
    private readonly IGrateTestContext _testContext;
    
    public CommandLineGrateTestContext(
        IGrateMigrator migrator, 
        IGrateTestContext testContext)
    {
        _testContext = testContext;
        Migrator = migrator;
    }

    public IGrateMigrator Migrator { get; }

    public string AdminConnectionString => _testContext.AdminConnectionString;
    public string ConnectionString(string database) => _testContext.ConnectionString(database);
    public string UserConnectionString(string database) => _testContext.UserConnectionString(database);

    public IDbConnection GetDbConnection(string connectionString) => _testContext.GetDbConnection(connectionString);

    public ISyntax Syntax => _testContext.Syntax;
    public Type DbExceptionType => _testContext.DbExceptionType;

    public Type DatabaseType => _testContext.DbExceptionType;
    public bool SupportsTransaction => _testContext.SupportsTransaction;

    public SqlStatements Sql => _testContext.Sql;

    public string ExpectedVersionPrefix => _testContext.ExpectedVersionPrefix;
    public bool SupportsCreateDatabase => _testContext.SupportsCreateDatabase;
    public bool SupportsSchemas => _testContext.SupportsSchemas;
}
