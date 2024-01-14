using MariaDB.TestInfrastructure;
using TestCommon.Generic;
using TestCommon.TestInfrastructure;

namespace MariaDB;

[Collection(nameof(MariaDbTestContainer))]
public class Database(IGrateTestContext testContext, ITestOutputHelper testOutput)
    : GenericDatabase(testContext, testOutput);

