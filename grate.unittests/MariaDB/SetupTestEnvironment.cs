using grate.unittests.TestInfrastructure;
using NUnit.Framework;

namespace grate.unittests.MariaDB
{
    [SetUpFixture]
    [Category("MariaDB")]
    public class SetupTestEnvironment : Generic.GenericSetupTestEnvironment
    {
        protected override IGrateTestContext Context => GrateTestContext.MariaDB;
    }
}