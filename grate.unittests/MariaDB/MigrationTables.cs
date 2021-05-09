using grate.unittests.TestInfrastructure;
using NUnit.Framework;

namespace grate.unittests.MariaDB
{
    [TestFixture]
    public class MigrationTables: Generic.GenericMigrationTables
    {
        protected override IGrateTestContext Context => GrateTestContext.MariaDB;
    }
}
