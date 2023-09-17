using NUnit.Framework;
using Unit_tests.TestInfrastructure;

namespace Unit_tests.SqlServerCaseSensitive.Running_MigrationScripts
{
    [TestFixture]
    [Category("SqlServerCaseSensitive")]
    // ReSharper disable once InconsistentNaming
    public class Failing_Scripts : Generic.Running_MigrationScripts.Failing_Scripts
    {
        protected override IGrateTestContext Context => GrateTestContext.SqlServerCaseSensitive;
        protected override string ExpectedErrorMessageForInvalidSql => "Incorrect syntax near 'TOP'.";
    }
}
