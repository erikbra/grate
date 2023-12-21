using Sqlite.TestInfrastructure;
using TestCommon.TestInfrastructure;

namespace Sqlite.Running_MigrationScripts;

[Collection(nameof(SqliteTestContainer))]
public class ScriptsRun_Table : TestCommon.Generic.Running_MigrationScripts.ScriptsRun_Table, IClassFixture<DependencyService>
{

    protected override IGrateTestContext Context { get; }

    protected override ITestOutputHelper TestOutput { get; }

    public ScriptsRun_Table(SqliteTestContainer testContainer, DependencyService dependencyService, ITestOutputHelper testOutput)
    {
        Context = new SqliteGrateTestContext(dependencyService.ServiceProvider, testContainer);
        TestOutput = testOutput;
    }

}
