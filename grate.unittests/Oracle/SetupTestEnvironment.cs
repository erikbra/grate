using grate.unittests.TestInfrastructure;
using NUnit.Framework;

namespace grate.unittests.Oracle
{
    [SetUpFixture]
    [Category("Oracle")]
    public class SetupTestEnvironment : Generic.SetupDockerTestEnvironment
    {
        protected override IGrateTestContext GrateTestContext => unittests.GrateTestContext.Oracle;
        protected override IDockerTestContext DockerTestContext => unittests.GrateTestContext.Oracle;
    }
}
