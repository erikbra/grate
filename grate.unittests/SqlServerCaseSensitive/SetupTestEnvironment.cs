using NUnit.Framework;
using Unit_tests.TestInfrastructure;

namespace Unit_tests.SqlServerCaseSensitive
{
    [SetUpFixture]
    [Category("SqlServerCaseSensitive")]
    public class SetupTestEnvironment : Generic.SetupDockerTestEnvironment
    {
        protected override IGrateTestContext GrateTestContext => Unit_tests.GrateTestContext.SqlServerCaseSensitive;
        protected override IDockerTestContext DockerTestContext => Unit_tests.GrateTestContext.SqlServerCaseSensitive;
    }
}
