using grate.Configuration;
using Oracle.TestInfrastructure;
using TestCommon.TestInfrastructure;

namespace Oracle.Running_MigrationScripts;

[Collection(nameof(OracleTestContainer))]
// ReSharper disable once InconsistentNaming
public class Versioning_The_Database : TestCommon.Generic.Running_MigrationScripts.Versioning_The_Database, IClassFixture<SimpleService>
{
    protected override IGrateTestContext Context { get; }

    protected override ITestOutputHelper TestOutput { get; }
    protected OracleTestContainer _testContainer;

    public Versioning_The_Database(OracleTestContainer testContainer, SimpleService simpleService, ITestOutputHelper testOutput)
    {
        Context = new OracleGrateTestContext(simpleService.ServiceProvider, testContainer);
        TestOutput = testOutput;
        _testContainer = testContainer;
    }
    [Fact(Skip = "Skip due to Oracle doesn't support dynamic database creation in runtime")]
    public override Task Does_not_create_versions_when_no_script_existing()
    {
        return Task.CompletedTask;
    }
    protected override void ConfigureService(GrateConfigurationBuilder grateConfiguration) => grateConfiguration.ConfigureService(_testContainer);
}
