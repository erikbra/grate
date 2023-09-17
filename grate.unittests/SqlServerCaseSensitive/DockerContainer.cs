using NUnit.Framework;
using Unit_tests.TestInfrastructure;

namespace Unit_tests.SqlServerCaseSensitive
{
    [TestFixture]
    [Category("SqlServerCaseSensitive")]
    public class DockerContainer : Generic.GenericDockerContainer
    {
        protected override IGrateTestContext Context => GrateTestContext.SqlServerCaseSensitive;
    }
}
