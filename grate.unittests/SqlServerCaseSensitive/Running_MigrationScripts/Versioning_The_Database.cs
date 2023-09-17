using grate.unittests.TestInfrastructure;
using NUnit.Framework;

namespace grate.unittests.SqlServerCaseSensitive.Running_MigrationScripts
{
    [TestFixture]
    [Category("SqlServerCaseSensitive")]
    // ReSharper disable once InconsistentNaming
    public class Versioning_The_Database : grate.unittests.SqlServer.Running_MigrationScripts.Versioning_The_Database
    {
        protected override IGrateTestContext Context => GrateTestContext.SqlServerCaseSensitive;
    }
}
