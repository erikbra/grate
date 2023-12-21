using MariaDB.TestInfrastructure;
using TestCommon.Generic;
using TestCommon.TestInfrastructure;

namespace MariaDB;

[Collection(nameof(MariaDbTestContainer))]
public class MigrationTables : GenericMigrationTables, IClassFixture<DependencyService>
{
    protected override IGrateTestContext Context { get; }

    protected ITestOutputHelper TestOutput { get; }

    public MigrationTables(MariaDbTestContainer testContainer, DependencyService dependencyService, ITestOutputHelper testOutput)
    {
        Context = new MariaDbGrateTestContext(dependencyService.ServiceProvider, testContainer);
        TestOutput = testOutput;
    }
}
