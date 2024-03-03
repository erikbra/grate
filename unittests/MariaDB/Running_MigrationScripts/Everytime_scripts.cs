using MariaDB.TestInfrastructure;
using TestCommon.TestInfrastructure;

namespace MariaDB.Running_MigrationScripts;

[Collection(nameof(MariaDbGrateTestContext))]
// ReSharper disable once InconsistentNaming
public class Everytime_scripts(MariaDbGrateTestContext testContext, ITestOutputHelper testOutput)
    : TestCommon.Generic.Running_MigrationScripts.Everytime_scripts(testContext, testOutput);
