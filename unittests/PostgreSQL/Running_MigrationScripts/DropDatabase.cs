using PostgreSQL.TestInfrastructure;
using TestCommon.TestInfrastructure;

namespace PostgreSQL.Running_MigrationScripts;

[Collection(nameof(PostgresqlTestContainer))]
public class DropDatabase : TestCommon.Generic.Running_MigrationScripts.DropDatabase, IClassFixture<DependencyService>
{

    protected override IGrateTestContext Context { get; }
    protected override ITestOutputHelper TestOutput { get; }

    public DropDatabase(PostgresqlTestContainer testContainer, DependencyService simpleService, ITestOutputHelper testOutput)
    {
        Context = new PostgreSqlGrateTestContext(simpleService.ServiceProvider, testContainer);
        TestOutput = testOutput;
    }

}
