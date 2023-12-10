using Oracle.TestInfrastructure;
using TestCommon.TestInfrastructure;

namespace Oracle.Running_MigrationScripts;

[Collection(nameof(OracleTestContainer))]
public class DropDatabase : TestCommon.Generic.Running_MigrationScripts.DropDatabase, IClassFixture<SimpleService>
{
    protected override IGrateTestContext Context { get; }

    protected override ITestOutputHelper TestOutput { get; }

    public DropDatabase(OracleTestContainer testContainer, SimpleService simpleService, ITestOutputHelper testOutput)
    {
        Context = new OracleGrateTestContext(simpleService.ServiceProvider, testContainer);
        TestOutput = testOutput;
    }
}
