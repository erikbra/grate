using NUnit.Framework;
using TestCommon.TestInfrastructure;

namespace TestCommon.PostgreSQL;

[SetUpFixture]
[Category("PostgreSQL")]
public class SetupTestEnvironment : Generic.SetupDockerTestEnvironment
{
    protected override IGrateTestContext GrateTestContext => TestCommon.GrateTestContext.PostgreSql;
    protected override IDockerTestContext DockerTestContext => TestCommon.GrateTestContext.PostgreSql;
}
