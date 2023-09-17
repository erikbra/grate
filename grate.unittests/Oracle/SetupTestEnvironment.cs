using NUnit.Framework;
using Unit_tests.TestInfrastructure;

namespace Unit_tests.Oracle;

[SetUpFixture]
[Category("Oracle")]
public class SetupTestEnvironment : Generic.SetupDockerTestEnvironment
{
    protected override IGrateTestContext GrateTestContext => Unit_tests.GrateTestContext.Oracle;
    protected override IDockerTestContext DockerTestContext => Unit_tests.GrateTestContext.Oracle;
}
