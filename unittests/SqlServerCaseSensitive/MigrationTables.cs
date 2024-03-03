using TestCommon.Generic;
using TestCommon.TestInfrastructure;
using SqlServerCaseSensitive.TestInfrastructure;

namespace SqlServerCaseSensitive;

[Collection(nameof(SqlServerGrateTestContext))]
public class MigrationTables(IGrateTestContext testContext, ITestOutputHelper testOutput)
    : GenericMigrationTables(testContext, testOutput);
