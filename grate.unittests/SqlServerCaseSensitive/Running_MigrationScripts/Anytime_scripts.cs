using grate.unittests.TestInfrastructure;
using NUnit.Framework;

namespace grate.unittests.SqlServerCaseSensitive.Running_MigrationScripts
{
    [TestFixture]
    [Category("SqlServerCaseSensitive")]
    // ReSharper disable once InconsistentNaming
    public class Anytime_scripts : Generic.Running_MigrationScripts.Anytime_scripts
    {
        protected override IGrateTestContext Context => GrateTestContext.SqlServerCaseSensitive;
    }
}
