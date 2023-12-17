using SqlServer.TestInfrastructure;
using TestCommon.TestInfrastructure;

namespace SqlServer;

[Collection(nameof(SqlServerTestContainer))]

public class MigrationTables : TestCommon.Generic.GenericMigrationTables, IClassFixture<SimpleService>
{

    protected override IGrateTestContext Context { get; }

    protected ITestOutputHelper TestOutput { get; }

    public MigrationTables(SqlServerTestContainer testContainer, SimpleService simpleService, ITestOutputHelper testOutput)
    {
        Context = new SqlServerGrateTestContext(simpleService.ServiceProvider, testContainer);
        TestOutput = testOutput;
    }
    protected override string CountTableSql(string schemaName, string tableName)
    {
        return $@"
SELECT count(table_name) FROM INFORMATION_SCHEMA.TABLES
WHERE
TABLE_SCHEMA = '{schemaName}' AND
TABLE_NAME = '{tableName}' COLLATE Latin1_General_CS_AS
";
    }
}
