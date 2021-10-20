using grate.unittests.TestInfrastructure;
using NUnit.Framework;

namespace grate.unittests.SqLite.Running_MigrationScripts
{
    [TestFixture]
    [Category("Sqlite")]
    public class Versioning_The_Database: Generic.Running_MigrationScripts.Versioning_The_Database
    {
        protected override IGrateTestContext Context => GrateTestContext.Sqlite;
    }
}
