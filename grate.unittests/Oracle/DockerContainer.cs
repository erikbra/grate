using NUnit.Framework;
using Unit_tests.TestInfrastructure;

namespace Unit_tests.Oracle;

[TestFixture]
[Category("Oracle")]
public class DockerContainer: Generic.GenericDockerContainer
{
    protected override IGrateTestContext Context => GrateTestContext.Oracle;
}
