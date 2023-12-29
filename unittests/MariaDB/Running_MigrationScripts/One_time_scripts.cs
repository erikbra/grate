using MariaDB.TestInfrastructure;
using TestCommon.TestInfrastructure;

namespace MariaDB.Running_MigrationScripts;

[Collection(nameof(MariaDbTestContainer))]
// ReSharper disable once InconsistentNaming
public class One_time_scripts : TestCommon.Generic.Running_MigrationScripts.One_time_scripts, IClassFixture<SimpleService>
{
    protected override IGrateTestContext Context { get; }

    protected override ITestOutputHelper TestOutput { get; }

    public One_time_scripts(MariaDbTestContainer testContainer, SimpleService simpleService, ITestOutputHelper testOutput)
    {
        Context = new MariaDbGrateTestContext(simpleService.ServiceProvider, testContainer);
        TestOutput = testOutput;
    }
}
