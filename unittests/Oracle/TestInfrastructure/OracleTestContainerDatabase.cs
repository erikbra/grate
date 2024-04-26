using DotNet.Testcontainers.Containers;
using Microsoft.Extensions.Logging;
using Testcontainers.Oracle;

namespace TestCommon.TestInfrastructure;
public class OracleTestContainerDatabase(
    GrateTestConfig grateTestConfig,
    ILogger<OracleTestContainerDatabase> logger) : TestContainerDatabase(logger)
{
    //public override string DockerImage => "gvenzl/oracle-xe:21.3.0-slim-faststart";
    public override string DockerImage => grateTestConfig.DockerImage ?? "gvenzl/oracle-free:latest-faststart";
    protected override int InternalPort => 1521;

    protected override IContainer InitializeTestContainer(ILogger logger)
    {
        return new OracleBuilder()
            .WithImage(DockerImage)
            .WithEnvironment("DOCKER_DEFAULT_PLATFORM", "linux/amd64")
            .WithEnvironment("ORACLE_PDB", "FREEPDB1")
            .WithPassword(AdminPassword)
            .WithPortBinding(InternalPort, true)
            .WithLogger(logger)
            .Build();
    }
    
    public override string AdminConnectionString => $@"Data Source={TestContainer.Hostname}:{Port}/FREEPDB1;User ID=system;Password={AdminPassword};Pooling=False";
    public override string ConnectionString(string database) => $@"Data Source={TestContainer.Hostname}:{Port}/FREEPDB1;User ID={database.ToUpper()};Password={AdminPassword};Pooling=False";
    public override string UserConnectionString(string database) => $@"Data Source={TestContainer.Hostname}:{Port}/FREEPDB1;User ID={database.ToUpper()};Password={AdminPassword};Pooling=False";

}
