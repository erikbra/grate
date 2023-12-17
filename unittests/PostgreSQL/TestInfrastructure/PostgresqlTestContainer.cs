namespace TestCommon.TestInfrastructure;
public class PostgresqlTestContainer : ContainerFixture
{
    public string? DockerImage => "postgres:latest";
    public int Port = 5432;
    public PostgresqlTestContainer()
    {
        TestContainer = new PostgreSqlBuilder()
                            .WithImage(DockerImage)
                            .WithPassword(AdminPassword)
                            .WithPortBinding(Port, true)
                        .Build();
    }
}

[CollectionDefinition(nameof(PostgresqlTestContainer))]
public class PostgresqlTestCollection : ICollectionFixture<PostgresqlTestContainer>
{
}
