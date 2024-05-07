using System.Data;
using grate.Infrastructure;
using grate.MariaDb.Infrastructure;
using grate.MariaDb.Migration;
using grate.Migration;
using MySqlConnector;
using TestCommon.TestInfrastructure;

namespace MySQL.TestInfrastructure;

[CollectionDefinition(nameof(MySqlGrateTestContext))]
public class MySqlTestCollection : ICollectionFixture<MySqlGrateTestContext>;

public record MySqlGrateTestContext(
    IGrateMigrator migrator,
    ITestDatabase testDatabase) : GrateTestContext(migrator, testDatabase)
{
    public override IDbConnection GetDbConnection(string connectionString) => new MySqlConnection(connectionString);

    public override ISyntax Syntax { get; } = new MariaDbSyntax();
    public override Type DbExceptionType => typeof(MySqlException);

    public override Type DatabaseType => typeof(MariaDbDatabase);
    public override bool SupportsTransaction => false;

    public override SqlStatements Sql => new()
    {
        SelectVersion = "SELECT VERSION()",
        SleepTwoSeconds = "SELECT SLEEP(2);",
        CreateUser = (_, user, password) => $"CREATE USER '{user}'@'%' IDENTIFIED BY '{password}';",
        GrantAccess =  (db, user) =>
            $"""
             -- GRANT SELECT, INSERT, UPDATE, DELETE, CREATE, INDEX, DROP, ALTER, CREATE TEMPORARY TABLES, 
             -- LOCK TABLES ON {db}.* TO '{user}'@'%';
             GRANT ALL PRIVILEGES ON {db}.* TO '{user}'@'%';
             FLUSH PRIVILEGES;
             """
    };


    public override string ExpectedVersionPrefix => "10.10.7-MySQL";
    public override bool SupportsCreateDatabase => true;
    public override bool SupportsSchemas => false;
}
