using MariaDB.TestInfrastructure;
using TestCommon.TestInfrastructure;

namespace MariaDB.Running_MigrationScripts;

[Collection(nameof(MariaDbTestContainer))]
// ReSharper disable once InconsistentNaming
public class Versioning_The_Database : TestCommon.Generic.Running_MigrationScripts.Versioning_The_Database
{

    public Versioning_The_Database(IGrateTestContext testContext, ITestOutputHelper testOutput)
    {
        Context = testContext;
        TestOutput = testOutput;
    }
}
