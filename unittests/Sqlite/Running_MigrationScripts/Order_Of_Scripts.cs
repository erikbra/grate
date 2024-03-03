using Sqlite.TestInfrastructure;
using TestCommon.TestInfrastructure;

namespace Sqlite.Running_MigrationScripts;

[Collection(nameof(SqliteTestDatabase))]
// ReSharper disable once InconsistentNaming
// ReSharper disable once UnusedType.Global
public class Order_Of_Scripts(SqliteGrateTestContext testContext, ITestOutputHelper testOutput)
    : TestCommon.Generic.Running_MigrationScripts.Order_Of_Scripts(testContext, testOutput);
