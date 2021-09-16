using grate.unittests.TestInfrastructure;
using NUnit.Framework;

namespace grate.unittests.MariaDB
{
    [TestFixture]
    [Category("MariaDB")]
    public class DockerContainer: Generic.GenericDockerContainer
    {
        protected override IGrateTestContext Context => GrateTestContext.MariaDB;
    }
}
