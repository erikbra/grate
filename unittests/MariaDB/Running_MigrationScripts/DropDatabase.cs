using MariaDB.TestInfrastructure;
using TestCommon.TestInfrastructure;

namespace MariaDB.Running_MigrationScripts;

[Collection(nameof(MariaDbGrateTestContext))]
// ReSharper disable once UnusedType.Global
public class DropDatabase(MariaDbGrateTestContext testContext, ITestOutputHelper testOutput)
    : TestCommon.Generic.Running_MigrationScripts.DropDatabase(testContext, testOutput);
