using grate.unittests.TestInfrastructure;
using NUnit.Framework;

namespace grate.unittests.SqlServerCaseSensitive
{
    [TestFixture]
    [Category("SqlServerCaseSensitive")]
    public class Database : Generic.GenericDatabase
    {
        protected override IGrateTestContext Context => GrateTestContext.SqlServerCaseSensitive;

    }
}
