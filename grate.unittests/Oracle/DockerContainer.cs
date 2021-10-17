using grate.unittests.TestInfrastructure;
using NUnit.Framework;

namespace grate.unittests.Oracle
{
    [TestFixture]
    [Category("Oracle")]
    public class DockerContainer: Generic.GenericDockerContainer
    {
        protected override IGrateTestContext Context => GrateTestContext.Oracle;
    }
}
