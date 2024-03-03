using TestCommon.TestInfrastructure;

namespace TestCommon;

public abstract class Startup<
    TTestContainerDatabase,
    TExternalDatabase,
    TGrateTestContext>: Startup
    where TTestContainerDatabase : ITestDatabase
    where TExternalDatabase : ITestDatabase
    where TGrateTestContext : IGrateTestContext 
{
    protected override Type TestContainerDatabaseType => typeof(TTestContainerDatabase);
    protected override Type ExternalTestDatabaseType => typeof(TExternalDatabase);
    protected override Type TestContextType => typeof(TGrateTestContext);
}
