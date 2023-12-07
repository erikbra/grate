using Testcontainers.Oracle;

namespace TestCommon.TestInfrastructure;
public class OracleTestContainer : ContainerFixture
{
    public string? DockerImage => "gvenzl/oracle-xe:21.3.0-slim-faststart";
    public int Port = 1521;
    public OracleTestContainer()
    {
        TestContainer = new OracleBuilder()
                            .WithImage(DockerImage)
                            .WithPassword(AdminPassword)
                            .WithPortBinding(Port, true)
                        .Build();
    }
}

[CollectionDefinition(nameof(OracleTestContainer))]
public class OracleTestCollection : ICollectionFixture<OracleTestContainer>
{
}
