using SqlServer.TestInfrastructure;
using TestCommon.TestInfrastructure;

namespace SqlServer.Running_MigrationScripts;

[Collection(nameof(SqlServerTestContainer))]
// ReSharper disable once InconsistentNaming
public class Environment_scripts : TestCommon.Generic.Running_MigrationScripts.Environment_scripts, IClassFixture<SimpleService>
{
    protected override IGrateTestContext Context { get; }

    protected override ITestOutputHelper TestOutput { get; }

    public Environment_scripts(SqlServerTestContainer testContainer, SimpleService simpleService, ITestOutputHelper testOutput)
    {
        Context = new SqlServerGrateTestContext(simpleService.ServiceProvider, testContainer);
        TestOutput = testOutput;
    }
}
