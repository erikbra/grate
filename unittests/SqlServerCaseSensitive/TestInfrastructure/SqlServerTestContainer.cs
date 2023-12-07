using System.Runtime.InteropServices;
using Testcontainers.MsSql;
using static System.Runtime.InteropServices.Architecture;
namespace TestCommon.TestInfrastructure;
public class SqlServerTestContainer : ContainerFixture
{
    // on arm64 (M1), the standard mssql/server image is not available
    public string? DockerImage => RuntimeInformation.ProcessArchitecture switch
    {
        Arm64 => "mcr.microsoft.com/azure-sql-edge:latest",
        X64 => "mcr.microsoft.com/mssql/server:2019-latest",
        var other => throw new PlatformNotSupportedException("Unsupported platform for running tests: " + other)
    };
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
