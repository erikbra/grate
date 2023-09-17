using NUnit.Framework;
using Unit_tests.TestInfrastructure;

namespace Unit_tests.MariaDB.Running_MigrationScripts;

[TestFixture]
[Category("MariaDB")]
// ReSharper disable once InconsistentNaming
public class Order_Of_Scripts: Generic.Running_MigrationScripts.Order_Of_Scripts
{
    protected override IGrateTestContext Context => GrateTestContext.MariaDB;
}
