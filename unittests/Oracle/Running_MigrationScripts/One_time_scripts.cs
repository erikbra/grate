using TestCommon.TestInfrastructure;

namespace Oracle.Running_MigrationScripts;


[Collection(nameof(OracleTestContainer))]
public class One_time_scripts(IGrateTestContext testContext, ITestOutputHelper testOutput)
    : TestCommon.Generic.Running_MigrationScripts.One_time_scripts(testContext, testOutput)
{
    protected override string CreateView1 => base.CreateView1 + " FROM DUAL";
    protected override string CreateView2 => base.CreateView2 + " FROM DUAL";
}

