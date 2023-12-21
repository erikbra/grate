using Oracle.TestInfrastructure;
using TestCommon.TestInfrastructure;

namespace Oracle.Running_MigrationScripts;

[Collection(nameof(OracleTestContainer))]
// ReSharper disable once InconsistentNaming
public class Failing_Scripts : TestCommon.Generic.Running_MigrationScripts.Failing_Scripts, IClassFixture<DependencyService>
{
    protected override IGrateTestContext Context { get; }

    protected override ITestOutputHelper TestOutput { get; }

    public Failing_Scripts(OracleTestContainer testContainer, DependencyService dependencyService, ITestOutputHelper testOutput)
    {
        Context = new OracleGrateTestContext(dependencyService.ServiceProvider, testContainer);
        TestOutput = testOutput;
    }


    protected override string ExpectedErrorMessageForInvalidSql =>
        @"ORA-00923: FROM keyword not found where expected";
}
