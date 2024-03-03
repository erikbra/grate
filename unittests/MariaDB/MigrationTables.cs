using MariaDB.TestInfrastructure;
using TestCommon.Generic;
using TestCommon.TestInfrastructure;

namespace MariaDB;

[Collection(nameof(MariaDbGrateTestContext))]
public class MigrationTables(MariaDbGrateTestContext testContext, ITestOutputHelper testOutput) :
    GenericMigrationTables(testContext, testOutput);
