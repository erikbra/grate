using grate.unittests.TestInfrastructure;
using NUnit.Framework;

namespace grate.unittests.MariaDB
{
    [TestFixture]
    public class DockerContainer: Generic.GenericDockerContainer
    {
        protected override IGrateTestContext Context => GrateTestContext.MariaDB;
    }
}
