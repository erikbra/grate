using grate.unittests.TestInfrastructure;
using NUnit.Framework;

namespace grate.unittests.PostgreSQL
{
    [TestFixture]
    public class Database: Generic.GenericDatabase
    {
        protected override IGrateTestContext Context => GrateTestContext.PostgreSql;
    }
}
