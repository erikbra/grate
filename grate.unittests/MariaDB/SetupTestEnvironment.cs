using grate.unittests.TestInfrastructure;
using NUnit.Framework;

namespace grate.unittests.MariaDB;

[SetUpFixture]
[Category("MariaDB")]
public class SetupTestEnvironment : Generic.SetupDockerTestEnvironment
{
    protected override IGrateTestContext GrateTestContext => unittests.GrateTestContext.MariaDB;
    protected override IDockerTestContext DockerTestContext => unittests.GrateTestContext.MariaDB;
}