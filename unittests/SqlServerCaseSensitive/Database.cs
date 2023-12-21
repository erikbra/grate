using SqlServerCaseSensitive.TestInfrastructure;
using TestCommon.Generic;
using TestCommon.TestInfrastructure;

namespace SqlServerCaseSensitive;
[Collection(nameof(SqlServerTestContainer))]
public class Database : GenericDatabase, IClassFixture<DependencyService>
{

    protected override IGrateTestContext Context { get; }

    protected ITestOutputHelper TestOutput { get; }

    public Database(SqlServerTestContainer testContainer, DependencyService simpleService, ITestOutputHelper testOutput)
    {
        Context = new SqlServerGrateTestContext(simpleService.ServiceProvider, testContainer);
        TestOutput = testOutput;
    }
}
