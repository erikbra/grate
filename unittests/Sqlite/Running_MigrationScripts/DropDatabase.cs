using Sqlite.TestInfrastructure;
using TestCommon.TestInfrastructure;

namespace Sqlite.Running_MigrationScripts;

[Collection(nameof(SqliteTestDatabase))]
// ReSharper disable once InconsistentNaming
// ReSharper disable once UnusedType.Global
public class DropDatabase(SqliteGrateTestContext testContext, ITestOutputHelper testOutput)
    : TestCommon.Generic.Running_MigrationScripts.DropDatabase(testContext, testOutput);
