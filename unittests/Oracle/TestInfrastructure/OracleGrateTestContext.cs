using System.Data;
using grate.Configuration;
using grate.Infrastructure;
using grate.Migration;
using grate.Oracle.Migration;
using Oracle.ManagedDataAccess.Client;
using TestCommon.TestInfrastructure;

namespace Oracle.TestInfrastructure;

public class OracleGrateTestContext : IGrateTestContext
{
    public IServiceProvider ServiceProvider { get; private set; }
    private readonly Func<GrateConfiguration, GrateMigrator> _getGrateMigrator;
    private readonly OracleTestContainer _testContainer;

    private readonly IDatabaseConnectionFactory _databaseConnectionFactory;
    
    public OracleGrateTestContext(
        //IServiceProvider serviceProvider,
        Func<GrateConfiguration, GrateMigrator> getGrateMigrator,
        IDatabase dbMigrator, 
        ISyntax syntax, 
        IDatabaseConnectionFactory databaseConnectionFactory, 
        OracleTestContainer container)
    {
        ServiceProvider = null!;
        _getGrateMigrator = getGrateMigrator;
        _testContainer = container;
        DatabaseMigrator = dbMigrator;
        Syntax = syntax;
        _databaseConnectionFactory = databaseConnectionFactory;
    }
    
    public IGrateMigrator GetMigrator(GrateConfiguration config) => _getGrateMigrator(config);
   
    public string AdminPassword => _testContainer.AdminPassword;
    public int? Port => _testContainer.TestContainer!.GetMappedPublicPort(_testContainer.Port);


    // public string DockerCommand(string serverName, string adminPassword) =>
    //     $"run -d --name {serverName} -p 1521 -e ORACLE_ENABLE_XDB=true -e ORACLE_PWD={adminPassword} -P container-registry.oracle.com/database/express:21.3.0-xe";


    public string AdminConnectionString => $@"Data Source={_testContainer.TestContainer!.Hostname}:{Port}/XEPDB1;User ID=system;Password={AdminPassword};Pooling=False";
    public string ConnectionString(string database) => $@"Data Source={_testContainer.TestContainer!.Hostname}:{Port}/XEPDB1;User ID={database.ToUpper()};Password={AdminPassword};Pooling=False";
    public string UserConnectionString(string database) => $@"Data Source={_testContainer.TestContainer!.Hostname}:{Port}/XEPDB1;User ID={database.ToUpper()};Password={AdminPassword};Pooling=False";

    public IDbConnection GetDbConnection(string connectionString) => _databaseConnectionFactory.GetDbConnection(connectionString);

    public ISyntax Syntax { get; init; }
    public Type DbExceptionType => typeof(OracleException);

    public string DatabaseType => OracleDatabase.Type;
    public bool SupportsTransaction => false;

    // public string DatabaseTypeName => "Oracle";
    // public string MasterDatabase => "oracle";

    public IDatabase DatabaseMigrator { get; init; }

    public SqlStatements Sql => new()
    {
        SelectVersion = "SELECT * FROM v$version WHERE banner LIKE 'Oracle%'",
        SleepTwoSeconds = "sys.dbms_session.sleep(2);"
    };

    public string ExpectedVersionPrefix => "Oracle Database 21c Express Edition Release 21.0.0.0.0 - Production";
    public bool SupportsCreateDatabase => true;
}
