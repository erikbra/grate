using grate.unittests.TestInfrastructure;
using NUnit.Framework;

namespace grate.unittests.SqlServer.Running_MigrationScripts
{
    [TestFixture]
    [Category("SqlServer")]
    public class Environment_scripts: Generic.Running_MigrationScripts.Environment_scripts
    {
        protected override IGrateTestContext Context => GrateTestContext.SqlServer;
    }
}
