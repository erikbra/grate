using grate.unittests.TestInfrastructure;
using NUnit.Framework;

namespace grate.unittests.PostgreSQL.Running_MigrationScripts
{
    [TestFixture]
    [Category("PostgreSQL")]
    public class TokenScripts : Generic.Running_MigrationScripts.TokenScripts
    {
        protected override IGrateTestContext Context => GrateTestContext.PostgreSql;
    }
}
