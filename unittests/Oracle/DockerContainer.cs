using NUnit.Framework;
using TestCommon.Generic;
using TestCommon.TestInfrastructure;

namespace Oracle;

[TestFixture]
[Category("Oracle")]
public class DockerContainer: GenericDockerContainer
{
    protected override IGrateTestContext Context => GrateTestContext.Oracle;
}
