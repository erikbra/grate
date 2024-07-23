using MariaDB.TestInfrastructure;

namespace MariaDB.Bootstrapping;

[Collection(nameof(MariaDbGrateTestContext))]
// ReSharper disable once InconsistentNaming
// ReSharper disable once UnusedType.Global
public class Grate_Internal_Scripts(MariaDbGrateTestContext testContext, ITestOutputHelper testOutput)
    : TestCommon.Generic.Bootstrapping.Grate_Internal_Scripts(testContext, testOutput);
