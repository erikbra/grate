using NUnit.Framework;
using Unit_tests.TestInfrastructure;

namespace Unit_tests.MariaDB;

[TestFixture]
[Category("MariaDB")]
public class MigrationTables: Generic.GenericMigrationTables
{
    protected override IGrateTestContext Context => GrateTestContext.MariaDB;
}
