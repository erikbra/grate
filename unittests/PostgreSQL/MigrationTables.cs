using TestCommon.Generic;
using TestCommon.TestInfrastructure;

namespace PostgreSQL;

[Collection(nameof(PostgreSqlTestContainer))]
public class MigrationTables(IGrateTestContext testContext, ITestOutputHelper testOutput) :
    GenericMigrationTables(testContext, testOutput);
