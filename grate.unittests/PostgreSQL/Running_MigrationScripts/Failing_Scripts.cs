using grate.unittests.TestInfrastructure;
using NUnit.Framework;

namespace grate.unittests.PostgreSQL.Running_MigrationScripts
{
    [TestFixture]
    [Category("PostgreSQL")]
    public class Failing_Scripts: Generic.Running_MigrationScripts.Failing_Scripts
    {
        protected override IGrateTestContext Context => GrateTestContext.PostgreSql;

        protected override string ExpextedErrorMessageForInvalidSql =>
@"42703: column ""top"" does not exist

POSITION: 8";
    }
}
