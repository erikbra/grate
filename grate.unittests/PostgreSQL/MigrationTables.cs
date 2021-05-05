using grate.unittests.TestInfrastructure;
using NUnit.Framework;

namespace grate.unittests.PostgreSQL
{
    [TestFixture]
    public class MigrationTables: Generic.GenericMigrationTables
    {
        protected override IGrateTestContext Context => GrateTestContext.PostgreSql;
    }
}
