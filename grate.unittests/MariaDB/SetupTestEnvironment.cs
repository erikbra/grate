using grate.unittests.TestInfrastructure;
using NUnit.Framework;

namespace grate.unittests.MariaDB;

public class SetupTestEnvironment
{
    [SetUpFixture]
    [Category("MariaDB")]
    public class Default : Generic.SetupDockerTestEnvironment
    {
        protected override IGrateTestContext GrateTestContext => unittests.GrateTestContext.MariaDB;
        protected override IDockerTestContext DockerTestContext => unittests.GrateTestContext.MariaDB;
    }

    [SetUpFixture]
    [Category("MariaDBCaseInsensitive")]
    public class CaseInsensitive : Generic.SetupDockerTestEnvironment
    {
        protected override IGrateTestContext GrateTestContext => unittests.GrateTestContext.MariaDBCaseInsensitive;
        protected override IDockerTestContext DockerTestContext => unittests.GrateTestContext.MariaDBCaseInsensitive;
    }
}
