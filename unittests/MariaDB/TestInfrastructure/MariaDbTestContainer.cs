using DotNet.Testcontainers.Configurations;
using Microsoft.Extensions.Logging;
using TestCommon.TestInfrastructure;
using Testcontainers.MariaDb;

namespace MariaDB.TestInfrastructure;

// ReSharper disable once ClassNeverInstantiated.Global
public class MariaDbTestContainer : ContainerFixture
{
    public string DockerImage => "mariadb:10.10";
    public readonly int Port = 3306;

    public MariaDbTestContainer(ILogger<MariaDbTestContainer> logger) : base()
    {
        TestcontainersSettings.Logger = logger;
        TestContainer = new MariaDbBuilder()
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
