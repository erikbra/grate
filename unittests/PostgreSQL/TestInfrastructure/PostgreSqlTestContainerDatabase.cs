using DotNet.Testcontainers.Containers;
using Microsoft.Extensions.Logging;

namespace TestCommon.TestInfrastructure;
public class PostgreSqlTestContainerDatabase : TestContainerDatabase
{
    public override string DockerImage => "postgres:16";
    protected override int InternalPort => 5432;
    
    public PostgreSqlTestContainerDatabase(ILogger<PostgreSqlTestContainerDatabase> logger) : base(logger)
    {
    }

    protected override IContainer InitializeTestContainer(ILogger logger)
    {
        return new PostgreSqlBuilder()
            .WithImage(DockerImage)
            .WithPassword(AdminPassword)
            .WithPortBinding(InternalPort, true)
            .WithLogger(logger)
            .Build();
    }
    
    public override string AdminConnectionString =>
        $"Host={TestContainer.Hostname};Port={Port};Database=postgres;Username=postgres;Password={AdminPassword};Include Error Detail=true;Pooling=false";


    public override string ConnectionString(string database) =>
        $"Host={TestContainer.Hostname};Port={Port};Database={database};Username=postgres;Password={AdminPassword};Include Error Detail=true;Pooling=false";

    public override string UserConnectionString(string database) =>
        $"Host={TestContainer.Hostname};Port={Port};Database={database};Username=postgres;Password={AdminPassword};Include Error Detail=true;Pooling=false";

    
}

