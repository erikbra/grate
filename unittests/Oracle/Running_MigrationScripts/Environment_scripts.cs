using TestCommon.TestInfrastructure;

namespace Oracle.Running_MigrationScripts;

// ReSharper disable once UnusedType.Global
[Collection(nameof(OracleTestContainer))]
public class Environment_scripts(IGrateTestContext testContext, ITestOutputHelper testOutput)
    : TestCommon.Generic.Running_MigrationScripts.Environment_scripts(testContext, testOutput);

