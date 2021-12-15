using grate.unittests.TestInfrastructure;
using NUnit.Framework;

namespace grate.unittests.PostgreSQL;

[SetUpFixture]
[Category("PostgreSQL")]
public class SetupTestEnvironment : Generic.SetupDockerTestEnvironment
{
    protected override IGrateTestContext GrateTestContext => unittests.GrateTestContext.PostgreSql;
    protected override IDockerTestContext DockerTestContext => unittests.GrateTestContext.PostgreSql;
}