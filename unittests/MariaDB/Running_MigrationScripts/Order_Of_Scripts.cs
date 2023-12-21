using MariaDB.TestInfrastructure;
using TestCommon.TestInfrastructure;

namespace MariaDB.Running_MigrationScripts;

[Collection(nameof(MariaDbTestContainer))]
// ReSharper disable once InconsistentNaming
public class Order_Of_Scripts : TestCommon.Generic.Running_MigrationScripts.Order_Of_Scripts, IClassFixture<DependencyService>
{
    protected override IGrateTestContext Context { get; }

    protected override ITestOutputHelper TestOutput { get; }

    public Order_Of_Scripts(MariaDbTestContainer testContainer, DependencyService dependencyService, ITestOutputHelper testOutput)
    {
        Context = new MariaDbGrateTestContext(dependencyService.ServiceProvider, testContainer);
        TestOutput = testOutput;
    }
}
