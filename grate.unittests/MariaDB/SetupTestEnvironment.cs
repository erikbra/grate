using NUnit.Framework;
using Unit_tests.TestInfrastructure;

namespace Unit_tests.MariaDB;

[SetUpFixture]
[Category("MariaDB")]
public class SetupTestEnvironment : Generic.SetupDockerTestEnvironment
{
    protected override IGrateTestContext GrateTestContext => Unit_tests.GrateTestContext.MariaDB;
    protected override IDockerTestContext DockerTestContext => Unit_tests.GrateTestContext.MariaDB;
}
