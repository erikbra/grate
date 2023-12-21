using Oracle.TestInfrastructure;
using TestCommon.TestInfrastructure;

namespace Oracle.Running_MigrationScripts;

[Collection(nameof(OracleTestContainer))]
public class TokenScripts : TestCommon.Generic.Running_MigrationScripts.TokenScripts, IClassFixture<DependencyService>
{
    protected override IGrateTestContext Context { get; }

    protected override ITestOutputHelper TestOutput { get; }

    public TokenScripts(OracleTestContainer testContainer, DependencyService dependencyService, ITestOutputHelper testOutput)
    {
        Context = new OracleGrateTestContext(dependencyService.ServiceProvider, testContainer);
        TestOutput = testOutput;
    }

    protected override string CreateDatabaseName => base.CreateDatabaseName + " FROM DUAL";
    protected override string CreateViewMyCustomToken => base.CreateViewMyCustomToken + " FROM DUAL";
}
