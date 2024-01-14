namespace TestCommon.TestInfrastructure;
public class PostgreSqlTestContainer : ContainerFixture
{
    public string? DockerImage => "postgres:16";
    public int Port = 5432;
    public PostgreSqlTestContainer()
    {
        TestContainer = new PostgreSqlBuilder()
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
