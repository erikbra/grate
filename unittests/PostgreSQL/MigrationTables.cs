using PostgreSQL.TestInfrastructure;
using TestCommon.Generic;
using TestCommon.TestInfrastructure;

namespace PostgreSQL;

[Collection(nameof(PostgreSqlGrateTestContext))]
public class MigrationTables(PostgreSqlGrateTestContext testContext, ITestOutputHelper testOutput) :
    GenericMigrationTables(testContext, testOutput);
