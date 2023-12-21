using Sqlite.TestInfrastructure;
using TestCommon.TestInfrastructure;

namespace Sqlite.Running_MigrationScripts;

[Collection(nameof(SqliteTestContainer))]
// ReSharper disable once InconsistentNaming
public class Order_Of_Scripts : TestCommon.Generic.Running_MigrationScripts.Order_Of_Scripts, IClassFixture<DependencyService>
{

    protected override IGrateTestContext Context { get; }

    protected override ITestOutputHelper TestOutput { get; }

    public Order_Of_Scripts(SqliteTestContainer testContainer, DependencyService simpleService, ITestOutputHelper testOutput)
    {
        Context = new SqliteGrateTestContext(simpleService.ServiceProvider, testContainer);
        TestOutput = testOutput;
    }

}
