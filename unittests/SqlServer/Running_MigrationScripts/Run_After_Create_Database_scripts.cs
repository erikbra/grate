using SqlServer.TestInfrastructure;
using TestCommon.TestInfrastructure;

namespace SqlServer.Running_MigrationScripts;

[Collection(nameof(SqlServerGrateTestContext))]
// ReSharper disable once InconsistentNaming
// ReSharper disable once UnusedType.Global
public class Run_After_Create_Database_scripts(SqlServerGrateTestContext testContext, ITestOutputHelper testOutput)
    : TestCommon.Generic.Running_MigrationScripts.Run_After_Create_Database_scripts(testContext, testOutput);
