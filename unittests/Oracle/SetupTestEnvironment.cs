using NUnit.Framework;
using TestCommon.Generic;
using TestCommon.TestInfrastructure;

namespace Oracle;

[SetUpFixture]
[Category("Oracle")]
public class SetupTestEnvironment : SetupDockerTestEnvironment
{
    protected override IGrateTestContext GrateTestContext => Oracle.GrateTestContext.Oracle;
    protected override IDockerTestContext DockerTestContext => Oracle.GrateTestContext.Oracle;
}
