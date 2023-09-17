using NUnit.Framework;
using Unit_tests.TestInfrastructure;

namespace Unit_tests.SqlServerCaseSensitive
{
    [TestFixture]
    [Category("SqlServerCaseSensitive")]
    public class MigrationTables : Generic.GenericMigrationTables
    {
        protected override IGrateTestContext Context => GrateTestContext.SqlServerCaseSensitive;
    }
}
