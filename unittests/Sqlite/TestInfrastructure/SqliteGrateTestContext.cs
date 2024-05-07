using System.Data;
using grate.Infrastructure;
using grate.Migration;
using grate.Sqlite.Infrastructure;
using grate.Sqlite.Migration;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.DependencyInjection;
using TestCommon.TestInfrastructure;

namespace Sqlite.TestInfrastructure;


[CollectionDefinition(nameof(SqliteGrateTestContext))]
public class SqliteTestCollection : ICollectionFixture<SqliteGrateTestContext>;

public record SqliteGrateTestContext(
    IGrateMigrator migrator,
    ITestDatabase testDatabase) : GrateTestContext(migrator, testDatabase)
{
    public override IDbConnection GetDbConnection(string connectionString) => new SqliteConnection(connectionString);

    public override ISyntax Syntax => new SqliteSyntax();
    public override Type DbExceptionType => typeof(SqliteException);

    public override Type DatabaseType => typeof(SqliteDatabase);
    public override bool SupportsTransaction => false;

    public override SqlStatements Sql => new()
    {
        SelectVersion = "SELECT sqlite_version();",
    };


    public override string ExpectedVersionPrefix => throw new NotSupportedException("Sqlite does not support versioning");
    public override bool SupportsCreateDatabase => false;
    public override bool SupportsSchemas => false;
}
