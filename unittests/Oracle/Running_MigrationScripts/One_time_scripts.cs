using Oracle.TestInfrastructure;
using TestCommon.TestInfrastructure;

namespace Oracle.Running_MigrationScripts;


[Collection(nameof(OracleGrateTestContext))]
public class One_time_scripts(OracleGrateTestContext testContext, ITestOutputHelper testOutput)
    : TestCommon.Generic.Running_MigrationScripts.One_time_scripts(testContext, testOutput)
{
    protected override string CreateView1 => base.CreateView1 + " FROM DUAL";
    protected override string CreateView2 => base.CreateView2 + " FROM DUAL";
}

