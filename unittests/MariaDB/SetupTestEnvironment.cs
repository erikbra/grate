using NUnit.Framework;
using TestCommon.Generic;
using TestCommon.TestInfrastructure;

namespace MariaDB;

[SetUpFixture]
[Category("MariaDB")]
public class SetupTestEnvironment : SetupDockerTestEnvironment
{
    protected override IGrateTestContext GrateTestContext => MariaDB.GrateTestContext.MariaDB;
    protected override IDockerTestContext DockerTestContext => MariaDB.GrateTestContext.MariaDB;
}
