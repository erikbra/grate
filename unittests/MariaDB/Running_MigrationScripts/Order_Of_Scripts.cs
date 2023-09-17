using NUnit.Framework;
using TestCommon;
using TestCommon.TestInfrastructure;

namespace MariaDB.Running_MigrationScripts;

[TestFixture]
[Category("MariaDB")]
// ReSharper disable once InconsistentNaming
public class Order_Of_Scripts: TestCommon.Generic.Running_MigrationScripts.Order_Of_Scripts
{
    protected override IGrateTestContext Context => GrateTestContext.MariaDB;
}
