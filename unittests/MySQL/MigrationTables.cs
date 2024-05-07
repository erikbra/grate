using MySQL.TestInfrastructure;
using TestCommon.Generic;
using TestCommon.TestInfrastructure;

namespace MySQL;

[Collection(nameof(MySqlGrateTestContext))]
public class MigrationTables(MySqlGrateTestContext testContext, ITestOutputHelper testOutput) :
    GenericMigrationTables(testContext, testOutput);
