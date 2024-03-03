using MariaDB.TestInfrastructure;
using TestCommon.Generic;
using TestCommon.TestInfrastructure;

namespace MariaDB;

[Collection(nameof(MariaDbGrateTestContext))]
// ReSharper disable once UnusedType.Global
public class Database(MariaDbGrateTestContext testContext, ITestOutputHelper testOutput)
    : GenericDatabase(testContext, testOutput);

