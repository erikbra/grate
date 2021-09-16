using grate.unittests.TestInfrastructure;
using NUnit.Framework;

namespace grate.unittests.PostgreSQL.Running_MigrationScripts
{
    [TestFixture]
    [Category("PostgreSQL")]
    public class One_time_scripts: Generic.Running_MigrationScripts.One_time_scripts
    {
        protected override IGrateTestContext Context => GrateTestContext.PostgreSql;
    }
}
