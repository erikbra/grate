using NUnit.Framework;
using TestCommon.Generic;
using TestCommon.TestInfrastructure;

namespace SqlServerCaseSensitive
{
    [TestFixture]
    [Category("SqlServerCaseSensitive")]
    public class MigrationTables : GenericMigrationTables
    {
        protected override IGrateTestContext Context => GrateTestContext.SqlServerCaseSensitive;
    }
}
