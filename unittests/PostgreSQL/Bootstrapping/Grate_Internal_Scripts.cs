using PostgreSQL.TestInfrastructure;

namespace PostgreSQL.Bootstrapping;

[Collection(nameof(PostgreSqlGrateTestContext))]
// ReSharper disable once InconsistentNaming
// ReSharper disable once UnusedType.Global
public class Grate_Internal_Scripts(PostgreSqlGrateTestContext testContext, ITestOutputHelper testOutput)
    : TestCommon.Generic.Bootstrapping.Grate_Internal_Scripts(testContext, testOutput);
