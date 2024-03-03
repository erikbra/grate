using System.Data;
using grate.Infrastructure;
using grate.Migration;
using grate.PostgreSql.Infrastructure;
using grate.PostgreSql.Migration;
using Npgsql;
using TestCommon.TestInfrastructure;

namespace PostgreSQL.TestInfrastructure;


[CollectionDefinition(nameof(PostgreSqlGrateTestContext))]
public class PostgresqlTestCollection : ICollectionFixture<PostgreSqlGrateTestContext>;

public class PostgreSqlGrateTestContext : GrateTestContext
{
    public PostgreSqlGrateTestContext(
        IGrateMigrator migrator,
        ITestDatabase testDatabase) : base(testDatabase)
    {
        Migrator = migrator;
    }

    public override IGrateMigrator Migrator { get; }
  
    public override IDbConnection GetDbConnection(string connectionString) => new NpgsqlConnection(connectionString);

    public override ISyntax Syntax => new PostgreSqlSyntax();
    public override Type DbExceptionType => typeof(PostgresException);

    public override Type DatabaseType => typeof(PostgreSqlDatabase);
    public override bool SupportsTransaction => true;

    public override SqlStatements Sql => new()
    {
        SelectVersion = "SELECT version()",
        SleepTwoSeconds = "SELECT pg_sleep(2);"
    };

    public override string ExpectedVersionPrefix => "PostgreSQL 16.";
    public override bool SupportsCreateDatabase => true;
    public override bool SupportsSchemas => true;

}
