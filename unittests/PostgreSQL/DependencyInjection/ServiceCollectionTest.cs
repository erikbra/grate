using TestCommon.TestInfrastructure;

namespace PostgreSQL.DependencyInjection;

[Collection(nameof(PostgreSqlTestContainer))]
public class ServiceCollectionTest(IGrateTestContext context)
    : TestCommon.DependencyInjection.GrateServiceCollectionTest(context);
