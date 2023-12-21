using MariaDB.TestInfrastructure;
using TestCommon.TestInfrastructure;

namespace MariaDB.Running_MigrationScripts;

[Collection(nameof(MariaDbTestContainer))]
// ReSharper disable once InconsistentNaming
public class Versioning_The_Database : TestCommon.Generic.Running_MigrationScripts.Versioning_The_Database, IClassFixture<DependencyService>
{
    protected override IGrateTestContext Context { get; }

    protected override ITestOutputHelper TestOutput { get; }

    public Versioning_The_Database(MariaDbTestContainer testContainer, DependencyService dependencyService, ITestOutputHelper testOutput)
    {
        Context = new MariaDbGrateTestContext(dependencyService.ServiceProvider, testContainer);
        TestOutput = testOutput;
    }
}
