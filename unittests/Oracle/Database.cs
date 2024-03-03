using Oracle.TestInfrastructure;
using TestCommon.Generic;
using TestCommon.TestInfrastructure;

namespace Oracle;

[Collection(nameof(OracleGrateTestContext))]
public class Database(OracleGrateTestContext testContext, ITestOutputHelper testOutput)
    : GenericDatabase(testContext, testOutput);
