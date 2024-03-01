using System.Data;
using grate.Infrastructure;
using grate.Migration;
using grate.SqlServer.Infrastructure;
using grate.SqlServer.Migration;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.DependencyInjection;
using TestCommon.TestInfrastructure;

namespace SqlServer.TestInfrastructure;

class SqlServerGrateTestContext : IGrateTestContext
{
    public IGrateMigrator Migrator { get; }
    private readonly SqlServerTestContainer _testContainer;

    public SqlServerGrateTestContext(
        IGrateMigrator migrator,
        string serverCollation, 
        SqlServerTestContainer container)
    {
        Migrator = migrator;
        _testContainer = container;
        ServerCollation = serverCollation;
        Syntax = new SqlServerSyntax();
    }
    public SqlServerGrateTestContext(
        IGrateMigrator migrator, 
        SqlServerTestContainer container) : this(migrator, "Danish_Norwegian_CI_AS", container)
    {
    }
    public string AdminPassword => _testContainer.AdminPassword;
    public int? Port => _testContainer.TestContainer!.GetMappedPublicPort(_testContainer.Port);
    
    public string AdminConnectionString =>
        $"Data Source=localhost,{Port};Initial Catalog=master;User Id=sa;Password={AdminPassword};Encrypt=false;Pooling=false";

    public string ConnectionString(string database) =>
        $"Data Source=localhost,{Port};Initial Catalog={database};User Id=sa;Password={AdminPassword};Encrypt=false;Pooling=false;Connect Timeout=2";

    public string UserConnectionString(string database) =>
        $"Data Source=localhost,{Port};Initial Catalog={database};User Id=zorro;Password=batmanZZ4;Encrypt=false;Pooling=false;Connect Timeout=2";

    public IDbConnection GetDbConnection(string connectionString) => new SqlConnection(connectionString);



    public ISyntax Syntax { get; init; }
    public Type DbExceptionType => typeof(SqlException);

    public Type DatabaseType => typeof(SqlServerDatabase);
    public bool SupportsTransaction => true;

    public SqlStatements Sql => new()
    {
        SelectVersion = "SELECT @@VERSION",
        SleepTwoSeconds = "WAITFOR DELAY '00:00:02'",
        CreateUser = (db, user, password) => 
$"""
    USE {db};
    CREATE LOGIN {user} WITH PASSWORD = '{password}';
    CREATE USER {user} FOR LOGIN {user};
""",
        GrantAccess = (db, user) => 
$"""
    USE {db};
    ALTER ROLE db_owner ADD MEMBER {user};
""",
    };

    public string ExpectedVersionPrefix => "Microsoft SQL Server 2019";

    public bool SupportsCreateDatabase => true;
    public bool SupportsSchemas => true;

    public string ServerCollation { get; }
}
