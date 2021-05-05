using grate.unittests.TestInfrastructure;
using NUnit.Framework;

namespace grate.unittests.PostgreSQL
{
    [SetUpFixture]
    public class SetupTestEnvironment : Generic.GenericSetupTestEnvironment
    {
        protected override IGrateTestContext Context => GrateTestContext.PostgreSql;
    }
}