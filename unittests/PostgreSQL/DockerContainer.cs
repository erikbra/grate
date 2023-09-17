using NUnit.Framework;
using TestCommon.TestInfrastructure;

namespace TestCommon.PostgreSQL;

[TestFixture]
[Category("PostgreSQL")]
public class DockerContainer: Generic.GenericDockerContainer
{
    protected override IGrateTestContext Context => GrateTestContext.PostgreSql;
}
