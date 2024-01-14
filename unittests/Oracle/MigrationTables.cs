using grate.Configuration;
using TestCommon.Generic;
using TestCommon.TestInfrastructure;

namespace Oracle;

[Collection(nameof(OracleTestContainer))]
public class MigrationTables(IGrateTestContext testContext, ITestOutputHelper testOutput)
    : GenericMigrationTables(testContext, testOutput)
{

    protected override Task CheckTableCasing(string tableName, string funnyCasing, Func<GrateConfiguration, string, GrateConfiguration> setTableName)
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
