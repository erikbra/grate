using Sqlite.TestInfrastructure;
using TestCommon.TestInfrastructure;

namespace Sqlite.Running_MigrationScripts;

[Collection(nameof(SqliteTestDatabase))]
// ReSharper disable once InconsistentNaming
// ReSharper disable once UnusedType.Global
public class One_time_scripts(SqliteGrateTestContext testContext, ITestOutputHelper testOutput)
    : TestCommon.Generic.Running_MigrationScripts.One_time_scripts(testContext, testOutput);
