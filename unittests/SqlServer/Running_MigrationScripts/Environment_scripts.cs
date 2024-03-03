using SqlServer.TestInfrastructure;
using TestCommon.TestInfrastructure;

namespace SqlServer.Running_MigrationScripts;

[Collection(nameof(SqlServerGrateTestContext))]
// ReSharper disable once InconsistentNaming
public class Environment_scripts(SqlServerGrateTestContext testContext, ITestOutputHelper testOutput)
    : TestCommon.Generic.Running_MigrationScripts.Environment_scripts(testContext, testOutput);
