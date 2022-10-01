using grate.unittests.TestInfrastructure;
using NUnit.Framework;

namespace grate.unittests.Oracle;

[TestFixture]
[Category("Oracle")]
public class MigrationTables : Generic.GenericMigrationTables
{
    protected override IGrateTestContext Context => GrateTestContext.Oracle;

    protected override string CountTableSql(string schemaName, string tableName)
    {
        return $@"
SELECT COUNT(table_name) FROM user_tables
WHERE 
lower(table_name) = '{tableName.ToLowerInvariant()}'";
    }
}
