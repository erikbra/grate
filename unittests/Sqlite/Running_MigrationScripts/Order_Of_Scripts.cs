using NUnit.Framework;
using TestCommon;
using TestCommon.TestInfrastructure;

namespace Sqlite.Running_MigrationScripts;

[TestFixture]
[Category("Sqlite")]
// ReSharper disable once InconsistentNaming
public class Order_Of_Scripts: TestCommon.Generic.Running_MigrationScripts.Order_Of_Scripts
{
    protected override IGrateTestContext Context => GrateTestContext.Sqlite;
}
