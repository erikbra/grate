using Sqlite.TestInfrastructure;
using TestCommon.TestInfrastructure;

namespace Sqlite.Running_MigrationScripts;

[Collection(nameof(SqliteTestContainer))]
// ReSharper disable once InconsistentNaming
public class Versioning_The_Database : TestCommon.Generic.Running_MigrationScripts.Versioning_The_Database, IClassFixture<DependencyService>
{

    protected override IGrateTestContext Context { get; }

    protected override ITestOutputHelper TestOutput { get; }

    public Versioning_The_Database(SqliteTestContainer testContainer, DependencyService dependencyService, ITestOutputHelper testOutput)
    {
        Context = new SqliteGrateTestContext(dependencyService.ServiceProvider, testContainer);
        TestOutput = testOutput;
    }

}
