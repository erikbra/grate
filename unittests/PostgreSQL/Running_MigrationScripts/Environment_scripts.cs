using PostgreSQL.TestInfrastructure;
using TestCommon.TestInfrastructure;

namespace PostgreSQL.Running_MigrationScripts;

[Collection(nameof(PostgreSqlTestContainer))]

// ReSharper disable once InconsistentNaming
public class Environment_scripts : TestCommon.Generic.Running_MigrationScripts.Environment_scripts, IClassFixture<DependencyService>
{

    protected override IGrateTestContext Context { get; }
    protected override ITestOutputHelper TestOutput { get; }

    public Environment_scripts(PostgreSqlTestContainer testContainer, DependencyService dependencyService, ITestOutputHelper testOutput)
    {
        Context = new PostgreSqlGrateTestContext(dependencyService.ServiceProvider, testContainer);
        TestOutput = testOutput;
    }

}
