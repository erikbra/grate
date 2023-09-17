using NUnit.Framework;
using Unit_tests.TestInfrastructure;

namespace Unit_tests.SqlServer;

[TestFixture]
[Category("SqlServer")]
public class DockerContainer: Generic.GenericDockerContainer
{
    protected override IGrateTestContext Context => GrateTestContext.SqlServer;
}
