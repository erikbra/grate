using DotNet.Testcontainers.Configurations;
using DotNet.Testcontainers.Containers;
using DotNet.Testcontainers.Networks;
using Microsoft.Extensions.Logging;
using TestCommon.TestInfrastructure;
using Testcontainers.MySql;

namespace MySQL.TestInfrastructure;

// ReSharper disable once ClassNeverInstantiated.Global
public record MySqlTestContainerDatabase(
    GrateTestConfig GrateTestConfig,
    ILogger<MySqlTestContainerDatabase> Logger,
    INetwork Network
    ) : TestContainerDatabase(GrateTestConfig)
{
    public override string DockerImage => GrateTestConfig.DockerImage ??  "mysql/mysql-server:latest";
    protected override int InternalPort => MySqlBuilder.MySqlPort;
    protected override string NetworkAlias => "mysql-test-container";
    
    protected override string Hostname => IsInternal ? NetworkAlias : TestContainer.IpAddress;
    
    protected override IContainer InitializeTestContainer()
    {
        return new MySqlBuilder()
            //.WithCommand("--max_connections=10000")
            .WithImage(DockerImage)
            .WithDatabase(DefaultDatabase)
            .WithUsername(DefaultUser)
            .WithPassword(AdminPassword)
            //.WithHostname("%")
            //.WithHostname("127.0.0.1")
            //.WithExposedPort(InternalPort)
            .WithPortBinding(InternalPort, true)
            .WithNetworkAliases(NetworkAlias)
            .WithNetwork(Network)
            .WithLogger(Logger)
            .Build();
    }

    // public override async Task InitializeAsync()
    // {
    //     await Network.CreateAsync();
    //     //await base.InitializeAsync();
    //
    //     var container = TestContainer as MySqlContainer;
    //     await container!.StartAsync();
    //     //await TestContainer.StartAsync(WaitStrategy.WaitUntilAsync(this.TestContainer.Health.));
    // }
    
    private const string DefaultDatabase = "mysql";
    private const string DefaultUser = "mysqluser";
    
    public override string AdminConnectionString => $"Server={Hostname};Port={Port};Database={DefaultDatabase};Uid={DefaultUser};Pwd={AdminPassword}";
    public override string ConnectionString(string database) => $"Server={Hostname};Port={Port};Database={database};Uid={DefaultUser};Pwd={AdminPassword}";
    public override string UserConnectionString(string database) => $"Server={Hostname};Port={Port};Database={database};Uid={database};Pwd=mooo1213";
    
}


