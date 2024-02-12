using TestCommon.Generic;
using TestCommon.TestInfrastructure;

namespace PostgreSQL;

[Collection(nameof(PostgreSqlTestContainer))]
public class Database(IGrateTestContext testContext, ITestOutputHelper testOutput) :
    GenericDatabase(testContext, testOutput);
