using grate.unittests.TestInfrastructure;
using NUnit.Framework;

namespace grate.unittests.SqlServer.Running_MigrationScripts
{
    [TestFixture]
    public class Failing_Scripts: Generic.Running_MigrationScripts.Failing_Scripts
    {
        protected override IGrateTestContext Context => GrateTestContext.SqlServer;
        protected override string ExpextedErrorMessageForInvalidSql => "Incorrect syntax near 'TOP'.";
    }
}
