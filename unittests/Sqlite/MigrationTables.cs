using Sqlite.TestInfrastructure;
using TestCommon.TestInfrastructure;

namespace Sqlite;

[Collection(nameof(SqliteTestContainer))]
public class MigrationTables : TestCommon.Generic.GenericMigrationTables, IClassFixture<DependencyService>
{

    protected override IGrateTestContext Context { get; }

    protected ITestOutputHelper TestOutput { get; }

    public MigrationTables(SqliteTestContainer testContainer, DependencyService dependencyService, ITestOutputHelper testOutput)
    {
        Context = new SqliteGrateTestContext(dependencyService.ServiceProvider, testContainer);
        TestOutput = testOutput;
    }



    protected override string CountTableSql(string schemaName, string tableName)
    {
        return $@"
SELECT COUNT(name) FROM sqlite_master 
WHERE type ='table' AND 
name = '{tableName}';
";
    }
}
