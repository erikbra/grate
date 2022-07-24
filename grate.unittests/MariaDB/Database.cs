using grate.unittests.TestInfrastructure;
using NUnit.Framework;

namespace grate.unittests.MariaDB;

public class Database
{
    [TestFixture]
    [Category("MariaDB")]
    public class Default : Generic.GenericDatabase
    {
        protected override IGrateTestContext Context => GrateTestContext.MariaDB;
    }

    [TestFixture]
    [Category("MariaDBCaseInsensitive")]
    public class CaseInsensitive : Generic.GenericDatabase
    {
        protected override IGrateTestContext Context => GrateTestContext.MariaDBCaseInsensitive;
    }
}
