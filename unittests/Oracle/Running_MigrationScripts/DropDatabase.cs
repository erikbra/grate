using TestCommon.TestInfrastructure;

namespace Oracle.Running_MigrationScripts;

// ReSharper disable once UnusedType.Global
[Collection(nameof(OracleTestContainer))]
public class DropDatabase(IGrateTestContext testContext, ITestOutputHelper testOutput)
    : TestCommon.Generic.Running_MigrationScripts.DropDatabase(testContext, testOutput);
