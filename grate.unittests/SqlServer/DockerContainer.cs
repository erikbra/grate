using System.Data;
using System.Threading.Tasks;
using FluentAssertions;
using grate.unittests.TestInfrastructure;
using Microsoft.Data.SqlClient;
using NUnit.Framework;

namespace grate.unittests.SqlServer
{
    [TestFixture]
    public class DockerContainer: Generic.GenericDockerContainer
    {
        protected override IGrateTestContext Context => GrateTestContext.SqlServer;
    }
}
