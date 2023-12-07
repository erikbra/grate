using System.Data.Common;
using grate.Configuration;
using grate.Infrastructure;
using grate.Migration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MySqlConnector;
using TestCommon.TestInfrastructure;

namespace MariaDB.TestInfrastructure;

public class MariaDbGrateTestContext : IGrateTestContext
{
    public string AdminPassword => _testContainer.AdminPassword;
    public int? Port => _testContainer.TestContainer!.GetMappedPublicPort(_testContainer.Port);
    public IServiceProvider ServiceProvider { get; private set; }
    private readonly MariaDbTestContainer _testContainer;
    public MariaDbGrateTestContext(IServiceProvider serviceProvider, MariaDbTestContainer container)
    {
        ServiceProvider = serviceProvider;
        _testContainer = container;
    }

    // public string DockerCommand(string serverName, string adminPassword) =>
    //     $"run -d --name {serverName} -e MYSQL_ROOT_PASSWORD={adminPassword} -P mariadb:10.5.9";

    public string AdminConnectionString => $"Server={_testContainer.TestContainer!.Hostname};Port={Port};Database=mysql;Uid=root;Pwd={AdminPassword}";
    public string ConnectionString(string database) => $"Server={_testContainer.TestContainer!.Hostname};Port={Port};Database={database};Uid=root;Pwd={AdminPassword}";
    public string UserConnectionString(string database) => $"Server={_testContainer.TestContainer!.Hostname};Port={Port};Database={database};Uid={database};Pwd=mooo1213";

    public DbConnection GetDbConnection(string connectionString) => new MySqlConnection(connectionString);

    public ISyntax Syntax => new MariaDbSyntax();
    public Type DbExceptionType => typeof(MySqlException);

    public DatabaseType DatabaseType => DatabaseType.mariadb;
    public bool SupportsTransaction => false;
    public string DatabaseTypeName => "MariaDB Server";
    public string MasterDatabase => "mysql";

    public IDatabase DatabaseMigrator => new MariaDbDatabase(ServiceProvider.GetRequiredService<ILogger<MariaDbDatabase>>());

    public SqlStatements Sql => new()
    {
        SelectVersion = "SELECT VERSION()",
        SleepTwoSeconds = "SELECT SLEEP(2);",
        CreateUser = "CREATE USER '{0}'@'%' IDENTIFIED BY '{1}';",
        GrantAccess = "GRANT SELECT, INSERT, UPDATE, DELETE, CREATE, INDEX, DROP, ALTER, CREATE TEMPORARY TABLES, LOCK TABLES ON {0}.* TO '{1}'@'%';FLUSH PRIVILEGES;"
    };


    public string ExpectedVersionPrefix => "10.5.9-MariaDB";
    public bool SupportsCreateDatabase => true;
}
