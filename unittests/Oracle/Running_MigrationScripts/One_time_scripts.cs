using Oracle.TestInfrastructure;
using TestCommon.TestInfrastructure;

namespace Oracle.Running_MigrationScripts;

[Collection(nameof(OracleTestContainer))]
// ReSharper disable once InconsistentNaming
public class One_time_scripts : TestCommon.Generic.Running_MigrationScripts.One_time_scripts, IClassFixture<DependencyService>
{
    protected override IGrateTestContext Context { get; }

    protected override ITestOutputHelper TestOutput { get; }

    public One_time_scripts(OracleTestContainer testContainer, DependencyService dependencyService, ITestOutputHelper testOutput)
    {
        Context = new OracleGrateTestContext(dependencyService.ServiceProvider, testContainer);
        TestOutput = testOutput;
    }



    protected override string CreateView1 => base.CreateView1 + " FROM DUAL";
    protected override string CreateView2 => base.CreateView2 + " FROM DUAL";
}
