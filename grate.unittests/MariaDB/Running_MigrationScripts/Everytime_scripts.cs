using grate.unittests.TestInfrastructure;
using NUnit.Framework;

namespace grate.unittests.MariaDB.Running_MigrationScripts
{
    [TestFixture]
    public class Everytime_scripts: Generic.Running_MigrationScripts.Everytime_scripts
    {
        protected override IGrateTestContext Context => GrateTestContext.MariaDB;
    }
}
