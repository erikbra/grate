using System.Data;
using grate.Infrastructure;
using grate.MariaDb.Infrastructure;
using grate.MariaDb.Migration;
using grate.Migration;
using MySqlConnector;
using TestCommon.TestInfrastructure;

namespace MariaDB.TestInfrastructure;

public class MariaDbGrateTestContext : IGrateTestContext
{
    private readonly MariaDbTestContainer _testContainer;
    
    public MariaDbGrateTestContext(
        IGrateMigrator migrator, 
        MariaDbTestContainer container)
    {
        Migrator = migrator;
        _testContainer = container;
    }

    public IGrateMigrator Migrator { get; }
    
    private string AdminPassword => _testContainer.AdminPassword;
    private int? Port => _testContainer.TestContainer!.GetMappedPublicPort(_testContainer.Port);
    private string Hostname => _testContainer.TestContainer!.Hostname;


    public string AdminConnectionString => $"Server={Hostname};Port={Port};Database=mysql;Uid=root;Pwd={AdminPassword}";
    public string ConnectionString(string database) => $"Server={_testContainer.TestContainer!.Hostname};Port={Port};Database={database};Uid=root;Pwd={AdminPassword}";
    public string UserConnectionString(string database) => $"Server={_testContainer.TestContainer!.Hostname};Port={Port};Database={database};Uid={database};Pwd=mooo1213";

    public IDbConnection GetDbConnection(string connectionString) => new MySqlConnection(connectionString);

    public ISyntax Syntax { get; } = new MariaDbSyntax();
    public Type DbExceptionType => typeof(MySqlException);

    public Type DatabaseType => typeof(MariaDbDatabase);
    public bool SupportsTransaction => false;

    public SqlStatements Sql => new()
    {
        SelectVersion = "SELECT VERSION()",
        SleepTwoSeconds = "SELECT SLEEP(2);",
        CreateUser = (_, user, password) => $"CREATE USER '{user}'@'%' IDENTIFIED BY '{password}';",
        GrantAccess =  (db, user) =>
            $"""
             GRANT SELECT, INSERT, UPDATE, DELETE, CREATE, INDEX, DROP, ALTER, CREATE TEMPORARY TABLES, 
             LOCK TABLES ON {db}.* TO '{user}'@'%';
             FLUSH PRIVILEGES;
             """
    };


    public string ExpectedVersionPrefix => "10.10.7-MariaDB";
    public bool SupportsCreateDatabase => true;
}
