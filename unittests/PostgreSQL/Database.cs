using PostgreSQL.TestInfrastructure;
using TestCommon.Generic;
using TestCommon.TestInfrastructure;

namespace PostgreSQL;

[Collection(nameof(PostgreSqlGrateTestContext))]
public class Database(PostgreSqlGrateTestContext testContext, ITestOutputHelper testOutput) :
    GenericDatabase(testContext, testOutput);
