using grate.unittests.TestInfrastructure;
using NUnit.Framework;

namespace grate.unittests.SqlServer.Running_MigrationScripts
{
    [TestFixture]
    [Category("SqlServer")]
    public class Anytime_scripts: Generic.Running_MigrationScripts.Anytime_scripts
    {
        protected override IGrateTestContext Context => GrateTestContext.SqlServer;
    }
}
