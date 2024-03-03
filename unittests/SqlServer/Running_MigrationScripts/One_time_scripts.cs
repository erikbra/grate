using SqlServer.TestInfrastructure;
using TestCommon.TestInfrastructure;

namespace SqlServer.Running_MigrationScripts;

[Collection(nameof(SqlServerGrateTestContext))]
// ReSharper disable once InconsistentNaming
public class One_time_scripts(SqlServerGrateTestContext testContext, ITestOutputHelper testOutput)
    : TestCommon.Generic.Running_MigrationScripts.One_time_scripts(testContext, testOutput);
