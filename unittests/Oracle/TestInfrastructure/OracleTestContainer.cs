using DotNet.Testcontainers.Containers;
using Microsoft.Extensions.Logging;
using Testcontainers.Oracle;

namespace TestCommon.TestInfrastructure;
public class OracleTestContainer : ContainerFixture
{
    public override string DockerImage => "gvenzl/oracle-xe:21.3.0-slim-faststart";
    public override int Port => 1521;
    
    public OracleTestContainer(ILogger<OracleTestContainer> logger) : base(logger)
    {
    }

    protected override OracleContainer InitializeTestContainer()
    {
        return new OracleBuilder()
            .WithImage(DockerImage)
            .WithEnvironment("DOCKER_DEFAULT_PLATFORM", "linux/amd64")
            .WithPassword(AdminPassword)
            .WithPortBinding(Port, true)
            .Build();
    }
}

[CollectionDefinition(nameof(OracleTestContainer))]
public class OracleTestCollection : ICollectionFixture<OracleTestContainer>
{
}
