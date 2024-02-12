using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Configurations;
using DotNet.Testcontainers.Containers;
using Microsoft.Extensions.Logging;
using TestCommon.TestInfrastructure;
using Testcontainers.MariaDb;

namespace MariaDB.TestInfrastructure;

// ReSharper disable once ClassNeverInstantiated.Global
public class MariaDbTestContainer : ContainerFixture
{
    public override string DockerImage => "mariadb:10.10";
    public override int Port => 3306;
    
    public MariaDbTestContainer(ILogger<MariaDbTestContainer> logger) : base(logger)
    {
    }

    protected override MariaDbContainer InitializeTestContainer()
    {
        return new MariaDbBuilder()
            .WithImage(DockerImage)
            .WithPassword(AdminPassword)
            .WithPortBinding(Port, true)
            .Build();
    }
}

[CollectionDefinition(nameof(MariaDbTestContainer))]
public class MariaDbTestCollection : ICollectionFixture<MariaDbTestContainer>
{
}
