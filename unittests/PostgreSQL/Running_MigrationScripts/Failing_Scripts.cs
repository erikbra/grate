using PostgreSQL.TestInfrastructure;
using TestCommon.TestInfrastructure;

namespace PostgreSQL.Running_MigrationScripts;

[Collection(nameof(PostgreSqlTestContainer))]
// ReSharper disable once InconsistentNaming
public class Failing_Scripts : TestCommon.Generic.Running_MigrationScripts.Failing_Scripts, IClassFixture<DependencyService>
{

    protected override IGrateTestContext Context { get; }
    protected override ITestOutputHelper TestOutput { get; }

    public Failing_Scripts(PostgreSqlTestContainer testContainer, DependencyService dependencyService, ITestOutputHelper testOutput)
    {
        Context = new PostgreSqlGrateTestContext(dependencyService.ServiceProvider, testContainer);
        TestOutput = testOutput;
    }

    protected override string ExpectedErrorMessageForInvalidSql =>
        @"42703: column ""top"" does not exist

POSITION: 8";
}
