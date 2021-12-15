using grate.unittests.TestInfrastructure;
using NUnit.Framework;

namespace grate.unittests.SqlServer.Running_MigrationScripts;

[TestFixture]
[Category("SqlServer")]
public class Everytime_scripts: Generic.Running_MigrationScripts.Everytime_scripts
{
    protected override IGrateTestContext Context => GrateTestContext.SqlServer;
}