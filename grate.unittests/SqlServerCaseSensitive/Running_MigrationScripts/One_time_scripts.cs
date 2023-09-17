using NUnit.Framework;
using Unit_tests.TestInfrastructure;

namespace Unit_tests.SqlServerCaseSensitive.Running_MigrationScripts
{
    [TestFixture]
    [Category("SqlServerCaseSensitive")]
    // ReSharper disable once InconsistentNaming
    public class One_time_scripts : Generic.Running_MigrationScripts.One_time_scripts
    {
        protected override IGrateTestContext Context => GrateTestContext.SqlServerCaseSensitive;
    }
}
