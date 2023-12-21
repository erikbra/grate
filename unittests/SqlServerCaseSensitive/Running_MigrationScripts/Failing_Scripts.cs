using SqlServerCaseSensitive.TestInfrastructure;
using TestCommon.TestInfrastructure;

namespace SqlServerCaseSensitive.Running_MigrationScripts;
[Collection(nameof(SqlServerTestContainer))]
// ReSharper disable once InconsistentNaming
public class Failing_Scripts : TestCommon.Generic.Running_MigrationScripts.Failing_Scripts, IClassFixture<DependencyService>
{
    protected override IGrateTestContext Context { get; }

    protected override ITestOutputHelper TestOutput { get; }

    public Failing_Scripts(SqlServerTestContainer testContainer, DependencyService dependencyService, ITestOutputHelper testOutput)
    {
        Context = new SqlServerGrateTestContext(dependencyService.ServiceProvider, testContainer);
        TestOutput = testOutput;
    }
    protected override string ExpectedErrorMessageForInvalidSql => "Incorrect syntax near 'TOP'.";

}
