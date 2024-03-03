using SqlServerCaseSensitive.TestInfrastructure;
using TestCommon.TestInfrastructure;

namespace SqlServerCaseSensitive.DependencyInjection;

[Collection(nameof(SqlServerGrateTestContext))]
public class ServiceCollectionTest(IGrateTestContext context)
    : TestCommon.DependencyInjection.GrateServiceCollectionTest(context);
