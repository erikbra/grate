using PostgreSQL.TestInfrastructure;
using TestCommon.TestInfrastructure;

namespace PostgreSQL.Running_MigrationScripts;

[Collection(nameof(PostgreSqlTestContainer))]
public class DropDatabase : TestCommon.Generic.Running_MigrationScripts.DropDatabase, IClassFixture<DependencyService>
{

    protected override IGrateTestContext Context { get; }
    protected override ITestOutputHelper TestOutput { get; }

    public DropDatabase(PostgreSqlTestContainer testContainer, DependencyService dependencyService, ITestOutputHelper testOutput)
    {
        Context = new PostgreSqlGrateTestContext(dependencyService.ServiceProvider, testContainer);
        TestOutput = testOutput;
    }

}
