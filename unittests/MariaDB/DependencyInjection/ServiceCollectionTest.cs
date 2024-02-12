using MariaDB.TestInfrastructure;
using TestCommon.TestInfrastructure;
namespace MariaDB.DependencyInjection;

[Collection(nameof(MariaDbTestContainer))]
// ReSharper disable once UnusedType.Global
public class ServiceCollectionTest(IGrateTestContext context)
    : TestCommon.DependencyInjection.GrateServiceCollectionTest(context);
