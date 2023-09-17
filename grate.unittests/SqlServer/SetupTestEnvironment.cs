using NUnit.Framework;
using Unit_tests.TestInfrastructure;

namespace Unit_tests.SqlServer;

[SetUpFixture]
[Category("SqlServer")]
public class SetupTestEnvironment : Generic.SetupDockerTestEnvironment
{
    protected override IGrateTestContext GrateTestContext => Unit_tests.GrateTestContext.SqlServer;
    protected override IDockerTestContext DockerTestContext => Unit_tests.GrateTestContext.SqlServer;
}
