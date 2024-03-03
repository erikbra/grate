using System.Data;
using grate.Infrastructure;
using grate.MariaDb.Infrastructure;
using grate.MariaDb.Migration;
using grate.Migration;
using MySqlConnector;
using TestCommon.TestInfrastructure;

namespace MariaDB.TestInfrastructure;

[CollectionDefinition(nameof(MariaDbGrateTestContext))]
public class MariaDbTestCollection : ICollectionFixture<MariaDbGrateTestContext>;

public class MariaDbGrateTestContext : GrateTestContext
{
    public MariaDbGrateTestContext(
        IGrateMigrator migrator, 
        ITestDatabase testDatabase) : base(testDatabase)
    {
        Migrator = migrator;
    }

    public override IGrateMigrator Migrator { get; }

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
             GRANT SELECT, INSERT, UPDATE, DELETE, CREATE, INDEX, DROP, ALTER, CREATE TEMPORARY TABLES, 
             LOCK TABLES ON {db}.* TO '{user}'@'%';
             FLUSH PRIVILEGES;
             """
    };


    public override string ExpectedVersionPrefix => "10.10.7-MariaDB";
    public override bool SupportsCreateDatabase => true;
    public override bool SupportsSchemas => false;
}
