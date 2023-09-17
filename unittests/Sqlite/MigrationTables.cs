using NUnit.Framework;
using TestCommon;
using TestCommon.TestInfrastructure;

namespace Sqlite;

[TestFixture]
[Category("Sqlite")]
public class MigrationTables: TestCommon.Generic.GenericMigrationTables
{
    protected override IGrateTestContext Context => GrateTestContext.Sqlite;

    protected override string CountTableSql(string schemaName, string tableName)
    {
        return $@"
SELECT COUNT(name) FROM sqlite_master 
WHERE type ='table' AND 
name = '{tableName}';
";
    }
}
