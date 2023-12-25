using PostgreSQL.TestInfrastructure;
using TestCommon.Generic;
using TestCommon.TestInfrastructure;

namespace PostgreSQL;

[Collection(nameof(PostgreSqlTestContainer))]
public class Database : GenericDatabase, IClassFixture<DependencyService>
{

    protected override IGrateTestContext Context { get; }

    protected ITestOutputHelper TestOutput { get; }

    public Database(PostgreSqlTestContainer testContainer, DependencyService dependencyService, ITestOutputHelper testOutput)
    {
        Context = new PostgreSqlGrateTestContext(dependencyService.ServiceProvider, testContainer);
        TestOutput = testOutput;
    }

}
