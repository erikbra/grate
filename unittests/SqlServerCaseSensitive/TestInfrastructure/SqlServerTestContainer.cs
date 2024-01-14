using Testcontainers.MsSql;

namespace TestCommon.TestInfrastructure;
public class SqlServerTestContainer : ContainerFixture
{
    // Run with linux/amd86 on ARM architectures too, the docker emulation is good enough
    public string? DockerImage => "mcr.microsoft.com/mssql/server:2019-latest";
    public int Port = 1433;
    public SqlServerTestContainer()
    {
        TestContainer = new MsSqlBuilder()
                            .WithImage(DockerImage)
                            .WithPassword(AdminPassword)
                            .WithPortBinding(Port, true)
                            .WithEnvironment("MSSQL_COLLATION", "Latin1_General_CS_AS") //CS == Case Sensitive
                        .Build();
    }
}

[CollectionDefinition(nameof(SqlServerTestContainer))]
public class SqlServerCITestCollection : ICollectionFixture<SqlServerTestContainer>
{
}
