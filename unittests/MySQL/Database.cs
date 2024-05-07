using MySQL.TestInfrastructure;
using TestCommon.Generic;
using TestCommon.TestInfrastructure;

namespace MySQL;

[Collection(nameof(MySqlGrateTestContext))]
// ReSharper disable once UnusedType.Global
public class Database(MySqlGrateTestContext testContext, ITestOutputHelper testOutput)
    : GenericDatabase(testContext, testOutput);

