using System.Data;
using grate.Infrastructure;
using grate.Migration;
using grate.Oracle.Infrastructure;
using grate.Oracle.Migration;
using Oracle.ManagedDataAccess.Client;
using TestCommon.TestInfrastructure;

namespace Oracle.TestInfrastructure;

public class OracleGrateTestContext : IGrateTestContext
{
    private readonly OracleTestContainer _testContainer;

    public OracleGrateTestContext(
        IGrateMigrator grateMigrator, 
        OracleTestContainer container)
    {
        Migrator = grateMigrator;
        _testContainer = container;
    }
    
    public IGrateMigrator Migrator { get; }

    public string AdminPassword => _testContainer.AdminPassword;
    public int? Port => _testContainer.TestContainer!.GetMappedPublicPort(_testContainer.Port);


    // public string DockerCommand(string serverName, string adminPassword) =>
    //     $"run -d --name {serverName} -p 1521 -e ORACLE_ENABLE_XDB=true -e ORACLE_PWD={adminPassword} -P container-registry.oracle.com/database/express:21.3.0-xe";


    public string AdminConnectionString => $@"Data Source={_testContainer.TestContainer!.Hostname}:{Port}/XEPDB1;User ID=system;Password={AdminPassword};Pooling=False";
    public string ConnectionString(string database) => $@"Data Source={_testContainer.TestContainer!.Hostname}:{Port}/XEPDB1;User ID={database.ToUpper()};Password={AdminPassword};Pooling=False";
    public string UserConnectionString(string database) => $@"Data Source={_testContainer.TestContainer!.Hostname}:{Port}/XEPDB1;User ID={database.ToUpper()};Password={AdminPassword};Pooling=False";

    public IDbConnection GetDbConnection(string connectionString) => new OracleConnection(connectionString);

    public ISyntax Syntax => new OracleSyntax();
    public Type DbExceptionType => typeof(OracleException);

    public Type DatabaseType => typeof(OracleDatabase);
    public bool SupportsTransaction => false;

    public SqlStatements Sql => new()
    {
        SelectVersion = "SELECT * FROM v$version WHERE banner LIKE 'Oracle%'",
        SleepTwoSeconds = "sys.dbms_session.sleep(2);"
    };

    public string ExpectedVersionPrefix => "Oracle Database 21c Express Edition Release 21.0.0.0.0 - Production";
    public bool SupportsCreateDatabase => true;
    public bool SupportsSchemas => false;
}
