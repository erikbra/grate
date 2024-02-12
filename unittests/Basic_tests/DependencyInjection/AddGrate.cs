using FluentAssertions;
using grate.Configuration;
using grate.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace Basic_tests.DependencyInjection;

// ReSharper disable once InconsistentNaming
public class AddGrate
{
    [Fact]
    public void Registers_GrateConfiguration_in_the_ServiceCollection()
    {
        var serviceCollection = new ServiceCollection();
        var builder = GrateConfigurationBuilder.Create();
        var config = builder.Build();
        serviceCollection.AddGrate(config);
        serviceCollection.Should().ContainSingle(s => s.ServiceType == typeof(GrateConfiguration));
    }
    
}
