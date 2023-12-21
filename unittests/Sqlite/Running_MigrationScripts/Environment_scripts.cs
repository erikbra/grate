using Sqlite.TestInfrastructure;
using TestCommon.TestInfrastructure;

namespace Sqlite.Running_MigrationScripts;

[Collection(nameof(SqliteTestContainer))]
// ReSharper disable once InconsistentNaming
public class Environment_scripts : TestCommon.Generic.Running_MigrationScripts.Environment_scripts, IClassFixture<DependencyService>
{

    protected override IGrateTestContext Context { get; }

    protected override ITestOutputHelper TestOutput { get; }

    public Environment_scripts(SqliteTestContainer testContainer, DependencyService dependencyService, ITestOutputHelper testOutput)
    {
        Context = new SqliteGrateTestContext(dependencyService.ServiceProvider, testContainer);
        TestOutput = testOutput;
    }

}
