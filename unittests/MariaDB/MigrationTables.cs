using MariaDB.TestInfrastructure;
using TestCommon.Generic;
using TestCommon.TestInfrastructure;

namespace MariaDB;

[Collection(nameof(MariaDbTestContainer))]
public class MigrationTables(IGrateTestContext testContext, ITestOutputHelper testOutput) :
    GenericMigrationTables(testContext, testOutput);
