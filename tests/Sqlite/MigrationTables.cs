using Sqlite.TestInfrastructure;
using TestCommon.TestInfrastructure;

namespace Sqlite;

[Collection(nameof(SqliteTestContainer))]
public class MigrationTables : TestCommon.Generic.GenericMigrationTables, IClassFixture<SimpleService>
{

    protected override IGrateTestContext Context { get; }

    protected ITestOutputHelper TestOutput { get; }

    public MigrationTables(SqliteTestContainer testContainer, SimpleService simpleService, ITestOutputHelper testOutput)
    {
        Context = new SqliteGrateTestContext(simpleService.ServiceProvider, testContainer);
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
