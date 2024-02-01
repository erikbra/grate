using TestCommon.TestInfrastructure;

namespace SqlServerCaseSensitive;

[Collection(nameof(SqlServerTestContainer))]
public class Database(IGrateTestContext testContext, ITestOutputHelper testOutput)
    : TestCommon.Generic.GenericDatabase(testContext, testOutput);
