using SqlServer.TestInfrastructure;
using TestCommon.TestInfrastructure;

namespace SqlServer.DependencyInjection;

[Collection(nameof(SqlServerGrateTestContext))]
public class ServiceCollectionTest(SqlServerGrateTestContext context)
    : TestCommon.DependencyInjection.GrateServiceCollectionTest(context);
