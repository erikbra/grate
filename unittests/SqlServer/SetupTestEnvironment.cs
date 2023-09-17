using NUnit.Framework;
using TestCommon.TestInfrastructure;

namespace SqlServer;

[SetUpFixture]
[Category("SqlServer")]
public class SetupTestEnvironment : TestCommon.Generic.SetupDockerTestEnvironment
{
    protected override IGrateTestContext GrateTestContext => SqlServer.GrateTestContext.SqlServer;
    protected override IDockerTestContext DockerTestContext => SqlServer.GrateTestContext.SqlServer;
}
