using grate.unittests.TestInfrastructure;
using NUnit.Framework;

namespace grate.unittests.Sqlite.Running_MigrationScripts
{
    [TestFixture]
    [Category("Sqlite")]
    public class Environment_scripts: Generic.Running_MigrationScripts.Environment_scripts
    {
        protected override IGrateTestContext Context => GrateTestContext.Sqlite;
    }
}
