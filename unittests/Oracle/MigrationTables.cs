using grate.Configuration;
using Oracle.TestInfrastructure;
using TestCommon.Generic;
using TestCommon.TestInfrastructure;

namespace Oracle;

[Collection(nameof(OracleTestContainer))]
public class MigrationTables : GenericMigrationTables, IClassFixture<SimpleService>
{
    protected override IGrateTestContext Context { get; }

    protected ITestOutputHelper TestOutput { get; }

    public MigrationTables(OracleTestContainer testContainer, SimpleService simpleService, ITestOutputHelper testOutput)
    {
        Context = new OracleGrateTestContext(simpleService.ServiceProvider, testContainer);
        TestOutput = testOutput;
    }

    protected override Task CheckTableCasing(string tableName, string funnyCasing, Action<GrateConfiguration, string> setTableName)
    {
        TestOutput.WriteLine("Oracle has never been case-sensitive for grate. No need to introduce that now.");
        return Task.CompletedTask;
    }

    protected override string CountTableSql(string schemaName, string tableName)
    {
        return $@"
SELECT COUNT(table_name) FROM user_tables
WHERE 
lower(table_name) = '{tableName.ToLowerInvariant()}'";
    }
}
