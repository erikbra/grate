using Oracle.TestInfrastructure;
using TestCommon.TestInfrastructure;

namespace Oracle.Running_MigrationScripts;

[Collection(nameof(OracleTestContainer))]
// ReSharper disable once InconsistentNaming
public class Anytime_scripts : TestCommon.Generic.Running_MigrationScripts.Anytime_scripts, IClassFixture<DependencyService>
{
    protected override IGrateTestContext Context { get; }

    protected override ITestOutputHelper TestOutput { get; }

    public Anytime_scripts(OracleTestContainer testContainer, DependencyService dependencyService, ITestOutputHelper testOutput)
    {
        Context = new OracleGrateTestContext(dependencyService.ServiceProvider, testContainer);
        TestOutput = testOutput;
    }
}
