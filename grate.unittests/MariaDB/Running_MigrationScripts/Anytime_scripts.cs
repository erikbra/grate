using grate.unittests.TestInfrastructure;
using NUnit.Framework;

namespace grate.unittests.MariaDB.Running_MigrationScripts
{
    [TestFixture]
    [Category("MariaDB")]
    public class Anytime_scripts: Generic.Running_MigrationScripts.Anytime_scripts
    {
        protected override IGrateTestContext Context => GrateTestContext.MariaDB;
    }
}
