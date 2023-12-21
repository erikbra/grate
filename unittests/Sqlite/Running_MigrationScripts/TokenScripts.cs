using Sqlite.TestInfrastructure;
using TestCommon.TestInfrastructure;

namespace Sqlite.Running_MigrationScripts;

[Collection(nameof(SqliteTestContainer))]
public class TokenScripts : TestCommon.Generic.Running_MigrationScripts.TokenScripts, IClassFixture<DependencyService>
{

    protected override IGrateTestContext Context { get; }

    protected override ITestOutputHelper TestOutput { get; }

    public TokenScripts(SqliteTestContainer testContainer, DependencyService simpleService, ITestOutputHelper testOutput)
    {
        Context = new SqliteGrateTestContext(simpleService.ServiceProvider, testContainer);
        TestOutput = testOutput;
    }

}
