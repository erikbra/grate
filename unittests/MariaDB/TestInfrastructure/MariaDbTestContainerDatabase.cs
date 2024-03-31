using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Configurations;
using DotNet.Testcontainers.Containers;
using Microsoft.Extensions.Logging;
using TestCommon.TestInfrastructure;
using Testcontainers.MariaDb;

namespace MariaDB.TestInfrastructure;

// ReSharper disable once ClassNeverInstantiated.Global
public class MariaDbTestContainerDatabase : TestContainerDatabase
{
    public override string DockerImage => "mariadb:10.10";
    protected override int InternalPort => 3306;
    
    public MariaDbTestContainerDatabase(ILogger<MariaDbTestContainerDatabase> logger) : base(logger)
    {
    }

    protected override IContainer InitializeTestContainer(ILogger logger)
    {
        return new MariaDbBuilder()
            .WithImage(DockerImage)
            .WithPassword(AdminPassword)
            .WithPortBinding(InternalPort, true)
            .WithCommand("--max_connections=10000")
            .WithLogger(logger)
            .Build();
    }
    
    private string Hostname => TestContainer.Hostname;
    
    public override string AdminConnectionString => $"Server={Hostname};Port={Port};Database=mysql;Uid=root;Pwd={AdminPassword}";
    public override string ConnectionString(string database) => $"Server={Hostname};Port={Port};Database={database};Uid=root;Pwd={AdminPassword}";
    public override string UserConnectionString(string database) => $"Server={Hostname};Port={Port};Database={database};Uid={database};Pwd=mooo1213";
    
}


