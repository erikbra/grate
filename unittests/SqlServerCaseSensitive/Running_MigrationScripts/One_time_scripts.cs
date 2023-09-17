using NUnit.Framework;
using TestCommon.TestInfrastructure;

namespace SqlServerCaseSensitive.Running_MigrationScripts
{
    [TestFixture]
    [Category("SqlServerCaseSensitive")]
    // ReSharper disable once InconsistentNaming
    public class One_time_scripts : TestCommon.Generic.Running_MigrationScripts.One_time_scripts
    {
        protected override IGrateTestContext Context => GrateTestContext.SqlServerCaseSensitive;
    }
}
