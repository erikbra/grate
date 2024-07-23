using Oracle.TestInfrastructure;
using TestCommon.TestInfrastructure;

namespace Oracle.Running_MigrationScripts;

[Collection(nameof(OracleGrateTestContext))]
// ReSharper disable once InconsistentNaming
// ReSharper disable once UnusedType.Global
public class Run_After_Create_Database_scripts(OracleGrateTestContext testContext, ITestOutputHelper testOutput)
    : TestCommon.Generic.Running_MigrationScripts.Run_After_Create_Database_scripts(testContext, testOutput);
