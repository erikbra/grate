using Sqlite.TestInfrastructure;
using TestCommon.TestInfrastructure;

namespace Sqlite.Bootstrapping;


[Collection(nameof(SqliteTestDatabase))]
// ReSharper disable once InconsistentNaming
// ReSharper disable once UnusedType.Global
public class Grate_Internal_Scripts(SqliteGrateTestContext testContext, ITestOutputHelper testOutput)
    : TestCommon.Generic.Bootstrapping.Grate_Internal_Scripts(testContext, testOutput);
