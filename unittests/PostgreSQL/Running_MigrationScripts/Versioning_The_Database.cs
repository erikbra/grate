using PostgreSQL.TestInfrastructure;
using TestCommon.TestInfrastructure;
using Xunit.Abstractions;

namespace PostgreSQL.Running_MigrationScripts;

[Collection(nameof(PostgresqlTestContainer))]
// ReSharper disable once InconsistentNaming
public class Versioning_The_Database : TestCommon.Generic.Running_MigrationScripts.Versioning_The_Database, IClassFixture<SimpleService>
{

    protected override IGrateTestContext Context { get; }
    protected override ITestOutputHelper TestOutput { get; }

    public Versioning_The_Database(PostgresqlTestContainer testContainer, SimpleService simpleService, ITestOutputHelper testOutput)
    {
        Context = new PostgreSqlGrateTestContext(simpleService.ServiceProvider, testContainer);
        TestOutput = testOutput;
    }

}
