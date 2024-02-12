using TestCommon.TestInfrastructure;

namespace SqlServer.DependencyInjection;

[Collection(nameof(SqlServerTestContainer))]
public class ServiceCollectionTest(IGrateTestContext context)
    : TestCommon.DependencyInjection.GrateServiceCollectionTest(context);
