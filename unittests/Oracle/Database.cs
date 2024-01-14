using TestCommon.Generic;
using TestCommon.TestInfrastructure;

namespace Oracle;

[Collection(nameof(OracleTestContainer))]
public class Database(IGrateTestContext testContext, ITestOutputHelper testOutput)
    : GenericDatabase(testContext, testOutput);
