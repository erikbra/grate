using Oracle.TestInfrastructure;

namespace Oracle.Bootstrapping;

[Collection(nameof(OracleGrateTestContext))]
// ReSharper disable once InconsistentNaming
// ReSharper disable once UnusedType.Global
public class Grate_Internal_Scripts(OracleGrateTestContext testContext, ITestOutputHelper testOutput)
    : TestCommon.Generic.Bootstrapping.Grate_Internal_Scripts(testContext, testOutput);
