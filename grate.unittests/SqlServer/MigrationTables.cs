using grate.unittests.TestInfrastructure;
using NUnit.Framework;

namespace grate.unittests.SqlServer
{
    [TestFixture]
    public class MigrationTables: Generic.GenericMigrationTables
    {
        protected override IGrateTestContext Context => GrateTestContext.SqlServer;
    }
}
