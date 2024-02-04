using Microsoft.Extensions.Logging;
using Testcontainers.MsSql;
namespace TestCommon.TestInfrastructure;
public class SqlServerTestContainer : ContainerFixture
{
    // Run with linux/amd86 on ARM architectures too, the docker emulation is good enough
    public override string DockerImage => "mcr.microsoft.com/mssql/server:2019-latest";
    public override int Port => 1433;
    
    public SqlServerTestContainer(ILogger<SqlServerTestContainer> logger) : base(logger)
    {
    }

    protected override MsSqlContainer InitializeTestContainer()
    {
        return new MsSqlBuilder()
            .WithImage(DockerImage)
            .WithEnvironment("DOCKER_DEFAULT_PLATFORM", "linux/amd64")
            .WithPassword(AdminPassword)
            .WithPortBinding(Port, true)
            .WithEnvironment("MSSQL_COLLATION", "Danish_Norwegian_CI_AS")
            .Build();
    }
}

[CollectionDefinition(nameof(SqlServerTestContainer))]
public class SqlServerCITestCollection : ICollectionFixture<SqlServerTestContainer>
{
}
