using SqlServerCaseSensitive.TestInfrastructure;
using TestCommon.Generic;
using TestCommon.TestInfrastructure;

namespace SqlServerCaseSensitive;
[Collection(nameof(SqlServerTestContainer))]
public class MigrationTables : GenericMigrationTables, IClassFixture<DependencyService>
{

    protected override IGrateTestContext Context { get; }

    protected ITestOutputHelper TestOutput { get; }

    public MigrationTables(SqlServerTestContainer testContainer, DependencyService dependencyService, ITestOutputHelper testOutput)
    {
        Context = new SqlServerGrateTestContext(dependencyService.ServiceProvider, testContainer);
        TestOutput = testOutput;
    }
}
