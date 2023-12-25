using PostgreSQL.TestInfrastructure;
using TestCommon.TestInfrastructure;

namespace PostgreSQL.Running_MigrationScripts;

[Collection(nameof(PostgreSqlTestContainer))]
public class TokenScripts : TestCommon.Generic.Running_MigrationScripts.TokenScripts, IClassFixture<DependencyService>
{

    protected override IGrateTestContext Context { get; }
    protected override ITestOutputHelper TestOutput { get; }

    public TokenScripts(PostgreSqlTestContainer testContainer, DependencyService dependencyService, ITestOutputHelper testOutput)
    {
        Context = new PostgreSqlGrateTestContext(dependencyService.ServiceProvider, testContainer);
        TestOutput = testOutput;
    }
}
