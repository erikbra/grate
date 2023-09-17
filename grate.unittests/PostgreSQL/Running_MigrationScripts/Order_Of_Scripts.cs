using NUnit.Framework;
using Unit_tests.TestInfrastructure;

namespace Unit_tests.PostgreSQL.Running_MigrationScripts;

[TestFixture]
[Category("PostgreSQL")]
// ReSharper disable once InconsistentNaming
public class Order_Of_Scripts: Generic.Running_MigrationScripts.Order_Of_Scripts
{
    protected override IGrateTestContext Context => GrateTestContext.PostgreSql;
}
