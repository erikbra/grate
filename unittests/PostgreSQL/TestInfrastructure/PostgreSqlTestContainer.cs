using Microsoft.Extensions.Logging;

namespace TestCommon.TestInfrastructure;
public class PostgreSqlTestContainer : ContainerFixture
{
    public override string DockerImage => "postgres:16";
    public override int Port => 5432;
    
    public PostgreSqlTestContainer(ILogger<PostgreSqlTestContainer> logger) : base(logger)
    {
    }

    protected override PostgreSqlContainer InitializeTestContainer()
    {
        return new PostgreSqlBuilder()
            .WithImage(DockerImage)
            .WithPassword(AdminPassword)
            .WithPortBinding(Port, true)
            .Build();
    }
}

[CollectionDefinition(nameof(PostgreSqlTestContainer))]
public class PostgresqlTestCollection : ICollectionFixture<PostgreSqlTestContainer>
{
}
