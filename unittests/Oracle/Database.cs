using Oracle.TestInfrastructure;
using TestCommon.Generic;
using TestCommon.TestInfrastructure;

namespace Oracle;

[Collection(nameof(OracleTestContainer))]
public class Database : GenericDatabase, IClassFixture<DependencyService>
{
    protected override IGrateTestContext Context { get; }

    protected ITestOutputHelper TestOutput { get; }

    public Database(OracleTestContainer testContainer, DependencyService dependencyService, ITestOutputHelper testOutput)
    {
        Context = new OracleGrateTestContext(dependencyService.ServiceProvider, testContainer);
        TestOutput = testOutput;
    }
}
