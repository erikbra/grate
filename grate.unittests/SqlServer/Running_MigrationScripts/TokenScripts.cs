using grate.unittests.TestInfrastructure;
using NUnit.Framework;

namespace grate.unittests.SqlServer.Running_MigrationScripts
{
    [TestFixture]
    [Category("SqlServer")]
    public class TokenScripts : Generic.Running_MigrationScripts.TokenScripts
    {
        protected override IGrateTestContext Context => GrateTestContext.SqlServer;
    }
}
