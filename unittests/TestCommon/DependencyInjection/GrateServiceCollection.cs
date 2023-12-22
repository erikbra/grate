using FluentAssertions;
using grate;
using grate.Configuration;
using grate.Infrastructure;
using grate.Migration;
using Microsoft.Extensions.DependencyInjection;
namespace TestCommon.DependencyInjection;
public abstract class GrateServiceCollectionTest
{
    protected abstract void ConfigureService(GrateConfiguration grateConfiguration);


    [Fact]
    public void Should_inject_all_nescessary_service_to_container()
    {
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddGrate(cfg =>
        {
            ConfigureService(cfg);
        });

        ValidateService(serviceCollection, typeof(IGrateMigrator), ServiceLifetime.Transient, typeof(GrateMigrator));
        ValidateService(serviceCollection, typeof(IDbMigrator), ServiceLifetime.Transient, typeof(DbMigrator));
        ValidateService(serviceCollection, typeof(IHashGenerator), ServiceLifetime.Transient, typeof(HashGenerator));
        ValidateService(serviceCollection, typeof(BatchSplitterReplacer), ServiceLifetime.Transient);
        ValidateService(serviceCollection, typeof(StatementSplitter), ServiceLifetime.Transient);
        ValidateDatabaseService(serviceCollection);
    }

    protected abstract void ValidateDatabaseService(ServiceCollection serviceCollection);

    protected void ValidateService(ServiceCollection serviceCollection, Type serviceType, ServiceLifetime lifetime, Type? expectedImplementationType = null)
    {
        var services = serviceCollection.Where(x => x.ServiceType == serviceType).ToArray();
        services.Should().HaveCount(1);
        var service = services.First();
        service.Lifetime.Should().Be(lifetime);
        if (expectedImplementationType is not null)
        {
            service.ImplementationType.Should().Be(expectedImplementationType);
        }
    }

    [Fact]
    public void Should_throw_invalid_operation_exception_when_no_database_is_configured()
    {
        var serviceCollection = new ServiceCollection()
                                    .AddGrate();
        var serviceProvider = serviceCollection.BuildServiceProvider();
        Action action = () => serviceProvider.GetService<IGrateMigrator>();
        action.Should().Throw<InvalidOperationException>("You forgot to configure the database. Please .UseXXX on the grate configuration.");
    }
}
