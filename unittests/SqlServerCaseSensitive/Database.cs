using NUnit.Framework;
using TestCommon.Generic;
using TestCommon.TestInfrastructure;

namespace SqlServerCaseSensitive
{
    [TestFixture]
    [Category("SqlServerCaseSensitive")]
    public class Database : GenericDatabase
    {
        protected override IGrateTestContext Context => GrateTestContext.SqlServerCaseSensitive;

    }
}
