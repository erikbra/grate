using grate.unittests.TestInfrastructure;
using NUnit.Framework;

namespace grate.unittests.SqlServer;

[SetUpFixture]
[Category("SqlServer")]
public class SetupTestEnvironment : Generic.SetupDockerTestEnvironment
{
    protected override IGrateTestContext GrateTestContext => unittests.GrateTestContext.SqlServer;
    protected override IDockerTestContext DockerTestContext => unittests.GrateTestContext.SqlServer;
}
