using grate.unittests.TestInfrastructure;
using NUnit.Framework;

namespace grate.unittests.MariaDB.Running_MigrationScripts;

[TestFixture]
[Category("MariaDB")]
// ReSharper disable once InconsistentNaming
public class Order_Of_Scripts: Generic.Running_MigrationScripts.Order_Of_Scripts
{
    protected override IGrateTestContext Context => GrateTestContext.MariaDB;
}
