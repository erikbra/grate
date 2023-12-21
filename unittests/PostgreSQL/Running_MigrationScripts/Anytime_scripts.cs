using PostgreSQL.TestInfrastructure;
using TestCommon.TestInfrastructure;

namespace PostgreSQL.Running_MigrationScripts;

[Collection(nameof(PostgresqlTestContainer))]
// ReSharper disable once InconsistentNaming
public class Anytime_scripts : TestCommon.Generic.Running_MigrationScripts.Anytime_scripts, IClassFixture<DependencyService>
{

    protected override IGrateTestContext Context { get; }

    protected override ITestOutputHelper TestOutput { get; }

    public Anytime_scripts(PostgresqlTestContainer testContainer, DependencyService simpleService, ITestOutputHelper testOutput)
    {
        Context = new PostgreSqlGrateTestContext(simpleService.ServiceProvider, testContainer);
        TestOutput = testOutput;
    }

}
