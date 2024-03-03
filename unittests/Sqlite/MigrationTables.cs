using Sqlite.TestInfrastructure;
using TestCommon.TestInfrastructure;

namespace Sqlite;

[Collection(nameof(SqliteTestDatabase))]
// ReSharper disable once InconsistentNaming
// ReSharper disable once UnusedType.Global
public class MigrationTables(SqliteGrateTestContext testContext, ITestOutputHelper testOutput)
    : TestCommon.Generic.GenericMigrationTables(testContext, testOutput)
{

    protected override string CountTableSql(string schemaName, string tableName)
    {
        return $@"
SELECT COUNT(name) FROM sqlite_master 
WHERE type ='table' AND 
name = '{tableName}';
";
    }
}
