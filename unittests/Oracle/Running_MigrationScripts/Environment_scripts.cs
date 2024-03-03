using Oracle.TestInfrastructure;
using TestCommon.TestInfrastructure;

namespace Oracle.Running_MigrationScripts;

// ReSharper disable once UnusedType.Global
[Collection(nameof(OracleGrateTestContext))]
public class Environment_scripts(OracleGrateTestContext testContext, ITestOutputHelper testOutput)
    : TestCommon.Generic.Running_MigrationScripts.Environment_scripts(testContext, testOutput);

