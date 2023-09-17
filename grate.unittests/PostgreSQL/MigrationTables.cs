using NUnit.Framework;
using Unit_tests.TestInfrastructure;

namespace Unit_tests.PostgreSQL;

[TestFixture]
[Category("PostgreSQL")]
public class MigrationTables: Generic.GenericMigrationTables
{
    protected override IGrateTestContext Context => GrateTestContext.PostgreSql;
}
