using NUnit.Framework;
using TestCommon.TestInfrastructure;

namespace SqlServerCaseSensitive.Running_MigrationScripts
{
    [TestFixture]
    [Category("SqlServerCaseSensitive")]
    // ReSharper disable once InconsistentNaming
    public class Failing_Scripts : TestCommon.Generic.Running_MigrationScripts.Failing_Scripts
    {
        protected override IGrateTestContext Context => GrateTestContext.SqlServerCaseSensitive;
        protected override string ExpectedErrorMessageForInvalidSql => "Incorrect syntax near 'TOP'.";
    }
}
