using TestCommon.Generic;
using TestCommon.TestInfrastructure;

namespace SqlServer;

[Collection(nameof(SqlServerTestContainer))]
public class MigrationTables(IGrateTestContext testContext, ITestOutputHelper testOutput)
    : GenericMigrationTables(testContext, testOutput)
{
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
