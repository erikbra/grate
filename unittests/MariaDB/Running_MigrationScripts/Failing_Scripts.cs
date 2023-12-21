using MariaDB.TestInfrastructure;
using TestCommon.TestInfrastructure;

namespace MariaDB.Running_MigrationScripts;

[Collection(nameof(MariaDbTestContainer))]
// ReSharper disable once InconsistentNaming
public class Failing_Scripts : TestCommon.Generic.Running_MigrationScripts.Failing_Scripts, IClassFixture<DependencyService>
{
    protected override IGrateTestContext Context { get; }

    protected override ITestOutputHelper TestOutput { get; }

    public Failing_Scripts(MariaDbTestContainer testContainer, DependencyService dependencyService, ITestOutputHelper testOutput)
    {
        Context = new MariaDbGrateTestContext(dependencyService.ServiceProvider, testContainer);
        TestOutput = testOutput;
    }

    protected override string ExpectedErrorMessageForInvalidSql => "Unknown column 'TOP' in 'field list'";
}
