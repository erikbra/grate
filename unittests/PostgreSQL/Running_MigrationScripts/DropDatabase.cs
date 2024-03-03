using PostgreSQL.TestInfrastructure;
using TestCommon.TestInfrastructure;

namespace PostgreSQL.Running_MigrationScripts;

[Collection(nameof(PostgreSqlGrateTestContext))]
public class DropDatabase(PostgreSqlGrateTestContext testContext, ITestOutputHelper testOutput)
    : TestCommon.Generic.Running_MigrationScripts.DropDatabase(testContext, testOutput);
