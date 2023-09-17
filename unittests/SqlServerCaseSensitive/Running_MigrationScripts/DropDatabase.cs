using NUnit.Framework;
using TestCommon.TestInfrastructure;

namespace SqlServerCaseSensitive.Running_MigrationScripts
{
    [TestFixture]
    [Category("SqlServerCaseSensitive")]
    public class DropDatabase : TestCommon.Generic.Running_MigrationScripts.DropDatabase
    {
        protected override IGrateTestContext Context => GrateTestContext.SqlServerCaseSensitive;
    }
}
