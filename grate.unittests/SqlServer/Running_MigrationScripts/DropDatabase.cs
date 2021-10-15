using grate.unittests.TestInfrastructure;
using NUnit.Framework;

namespace grate.unittests.SqlServer.Running_MigrationScripts
{
    [TestFixture]
    [Category("SqlServer")]
    public class DropDatabase : Generic.Running_MigrationScripts.DropDatabase
    {
        protected override IGrateTestContext Context => GrateTestContext.SqlServer;
    }
}
