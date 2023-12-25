using PostgreSQL.TestInfrastructure;
using TestCommon.TestInfrastructure;

namespace PostgreSQL.Running_MigrationScripts;

[Collection(nameof(PostgreSqlTestContainer))]
public class ScriptsRun_Table : TestCommon.Generic.Running_MigrationScripts.ScriptsRun_Table, IClassFixture<DependencyService>
{

    protected override IGrateTestContext Context { get; }
    protected override ITestOutputHelper TestOutput { get; }

    public ScriptsRun_Table(PostgreSqlTestContainer testContainer, DependencyService dependencyService, ITestOutputHelper testOutput)
    {
        Context = new PostgreSqlGrateTestContext(dependencyService.ServiceProvider, testContainer);
        TestOutput = testOutput;
    }

}
