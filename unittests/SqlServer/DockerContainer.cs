using NUnit.Framework;
using TestCommon;
using TestCommon.TestInfrastructure;

namespace SqlServer;

[TestFixture]
[Category("SqlServer")]
public class DockerContainer: TestCommon.Generic.GenericDockerContainer
{
    protected override IGrateTestContext Context => GrateTestContext.SqlServer;
}
