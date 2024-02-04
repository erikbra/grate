using TestCommon.TestInfrastructure;

namespace SqlServerCaseSensitive.DependencyInjection;

[Collection(nameof(SqlServerTestContainer))]
public class ServiceCollectionTest(IGrateTestContext context)
    : TestCommon.DependencyInjection.GrateServiceCollectionTest(context);
