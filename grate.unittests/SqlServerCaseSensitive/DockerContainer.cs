using grate.unittests.TestInfrastructure;
using NUnit.Framework;

namespace grate.unittests.SqlServerCaseSensitive
{
    [TestFixture]
    [Category("SqlServerCaseSensitive")]
    public class DockerContainer : Generic.GenericDockerContainer
    {
        protected override IGrateTestContext Context => GrateTestContext.SqlServerCaseSensitive;
    }
}
