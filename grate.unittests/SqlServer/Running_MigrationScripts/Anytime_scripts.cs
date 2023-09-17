using NUnit.Framework;
using Unit_tests.TestInfrastructure;

namespace Unit_tests.SqlServer.Running_MigrationScripts;

[TestFixture]
[Category("SqlServer")]
// ReSharper disable once InconsistentNaming
public class Anytime_scripts: Generic.Running_MigrationScripts.Anytime_scripts
{
    protected override IGrateTestContext Context => GrateTestContext.SqlServer;
}
