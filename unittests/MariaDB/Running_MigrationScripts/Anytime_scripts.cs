using MariaDB.TestInfrastructure;
using TestCommon.TestInfrastructure;

namespace MariaDB.Running_MigrationScripts;

[Collection(nameof(MariaDbGrateTestContext))]
// ReSharper disable once InconsistentNaming
public class Anytime_scripts(MariaDbGrateTestContext testContext, ITestOutputHelper testOutput)
    : TestCommon.Generic.Running_MigrationScripts.Anytime_scripts(testContext, testOutput);
