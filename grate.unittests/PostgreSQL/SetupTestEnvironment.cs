using NUnit.Framework;
using Unit_tests.TestInfrastructure;

namespace Unit_tests.PostgreSQL;

[SetUpFixture]
[Category("PostgreSQL")]
public class SetupTestEnvironment : Generic.SetupDockerTestEnvironment
{
    protected override IGrateTestContext GrateTestContext => Unit_tests.GrateTestContext.PostgreSql;
    protected override IDockerTestContext DockerTestContext => Unit_tests.GrateTestContext.PostgreSql;
}
