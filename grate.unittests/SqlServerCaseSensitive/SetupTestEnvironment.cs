using grate.unittests.TestInfrastructure;
using NUnit.Framework;

namespace grate.unittests.SqlServerCaseSensitive
{
    [SetUpFixture]
    [Category("SqlServerCaseSensitive")]
    public class SetupTestEnvironment : Generic.SetupDockerTestEnvironment
    {
        protected override IGrateTestContext GrateTestContext => unittests.GrateTestContext.SqlServerCaseSensitive;
        protected override IDockerTestContext DockerTestContext => unittests.GrateTestContext.SqlServerCaseSensitive;
    }
}
