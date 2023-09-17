using NUnit.Framework;
using TestCommon.TestInfrastructure;

namespace MariaDB;

[TestFixture]
[Category("MariaDB")]
public class DockerContainer: TestCommon.Generic.GenericDockerContainer
{
    protected override IGrateTestContext Context => GrateTestContext.MariaDB;
}
