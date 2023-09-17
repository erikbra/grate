using NUnit.Framework;
using TestCommon;
using TestCommon.TestInfrastructure;

namespace SqlServer.Running_MigrationScripts;

[TestFixture]
[Category("SqlServer")]
// ReSharper disable once InconsistentNaming
public class Anytime_scripts: TestCommon.Generic.Running_MigrationScripts.Anytime_scripts
{
    protected override IGrateTestContext Context => GrateTestContext.SqlServer;
}
