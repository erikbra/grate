using grate.unittests.TestInfrastructure;
using NUnit.Framework;

namespace grate.unittests.PostgreSQL
{
    [TestFixture]
    public class DockerContainer: Generic.GenericDockerContainer
    {
        protected override IGrateTestContext Context => GrateTestContext.PostgreSql;
    }
}
