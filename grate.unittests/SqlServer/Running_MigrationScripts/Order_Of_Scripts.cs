using NUnit.Framework;
using Unit_tests.TestInfrastructure;

namespace Unit_tests.SqlServer.Running_MigrationScripts;

[TestFixture]
[Category("SqlServer")]
// ReSharper disable once InconsistentNaming
public class Order_Of_Scripts: Generic.Running_MigrationScripts.Order_Of_Scripts
{
    protected override IGrateTestContext Context => GrateTestContext.SqlServer;
}
