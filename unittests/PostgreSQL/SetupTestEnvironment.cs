using NUnit.Framework;
using TestCommon.Generic;
using TestCommon.TestInfrastructure;

namespace PostgreSQL;

[SetUpFixture]
[Category("PostgreSQL")]
public class SetupTestEnvironment : SetupDockerTestEnvironment
{
    protected override IGrateTestContext GrateTestContext => TestCommon.GrateTestContext.PostgreSql;
    protected override IDockerTestContext DockerTestContext => TestCommon.GrateTestContext.PostgreSql;
}
