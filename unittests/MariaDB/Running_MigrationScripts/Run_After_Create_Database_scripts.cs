using MariaDB.TestInfrastructure;
using TestCommon.TestInfrastructure;

namespace MariaDB.Running_MigrationScripts;

[Collection(nameof(MariaDbGrateTestContext))]
// ReSharper disable once InconsistentNaming
// ReSharper disable once UnusedType.Global
public class Run_After_Create_Database_scripts(MariaDbGrateTestContext testContext, ITestOutputHelper testOutput)
    : TestCommon.Generic.Running_MigrationScripts.Run_After_Create_Database_scripts(testContext, testOutput);
