using Testcontainers.MariaDb;

namespace TestCommon.TestInfrastructure;
public class MariaDbTestContainer : ContainerFixture
{
    public string? DockerImage => "mariadb:10.10";
    public int Port = 3306;
    public MariaDbTestContainer() : base()
    {
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
