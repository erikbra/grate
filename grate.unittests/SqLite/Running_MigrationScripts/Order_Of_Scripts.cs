using NUnit.Framework;
using Unit_tests.TestInfrastructure;

namespace Unit_tests.Sqlite.Running_MigrationScripts;

[TestFixture]
[Category("Sqlite")]
// ReSharper disable once InconsistentNaming
public class Order_Of_Scripts: Generic.Running_MigrationScripts.Order_Of_Scripts
{
    protected override IGrateTestContext Context => GrateTestContext.Sqlite;
}
