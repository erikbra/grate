using MariaDB.TestInfrastructure;
using TestCommon.TestInfrastructure;

namespace MariaDB.Running_MigrationScripts;

[Collection(nameof(MariaDbGrateTestContext))]
// ReSharper disable once InconsistentNaming
public class Order_Of_Scripts(MariaDbGrateTestContext testContext, ITestOutputHelper testOutput)
    : TestCommon.Generic.Running_MigrationScripts.Order_Of_Scripts(testContext, testOutput);

