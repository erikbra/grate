using Microsoft.Extensions.Logging;
using Testcontainers.Oracle;

namespace TestCommon.TestInfrastructure;
public class OracleTestContainerDatabase : TestContainerDatabase
{
    public override string DockerImage => "gvenzl/oracle-xe:21.3.0-slim-faststart";
    protected override int InternalPort => 1521;
    
    public OracleTestContainerDatabase(ILogger<OracleTestContainerDatabase> logger) : base(logger)
    {
    }

    protected override OracleContainer InitializeTestContainer()
    {
        return new OracleBuilder()
            .WithImage(DockerImage)
            .WithEnvironment("DOCKER_DEFAULT_PLATFORM", "linux/amd64")
            .WithPassword(AdminPassword)
            .WithPortBinding(InternalPort, true)
            .Build();
    }
    
    public override string AdminConnectionString => $@"Data Source={TestContainer.Hostname}:{Port}/XEPDB1;User ID=system;Password={AdminPassword};Pooling=False";
    public override string ConnectionString(string database) => $@"Data Source={TestContainer.Hostname}:{Port}/XEPDB1;User ID={database.ToUpper()};Password={AdminPassword};Pooling=False";
    public override string UserConnectionString(string database) => $@"Data Source={TestContainer.Hostname}:{Port}/XEPDB1;User ID={database.ToUpper()};Password={AdminPassword};Pooling=False";

}
