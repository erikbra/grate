using grate.unittests.TestInfrastructure;
using NUnit.Framework;

namespace grate.unittests.Sqlite
{
    [TestFixture]
    [Category("Sqlite")]
    public class MigrationTables: Generic.GenericMigrationTables
    {
        protected override IGrateTestContext Context => GrateTestContext.Sqlite;
    }
}
