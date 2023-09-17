using NUnit.Framework;
using TestCommon.Generic;
using TestCommon.TestInfrastructure;

namespace SqlServerCaseSensitive
{
    [SetUpFixture]
    [Category("SqlServerCaseSensitive")]
    public class SetupTestEnvironment : SetupDockerTestEnvironment
    {
        protected override IGrateTestContext GrateTestContext => SqlServerCaseSensitive.GrateTestContext.SqlServerCaseSensitive;
        protected override IDockerTestContext DockerTestContext => SqlServerCaseSensitive.GrateTestContext.SqlServerCaseSensitive;
    }
}
