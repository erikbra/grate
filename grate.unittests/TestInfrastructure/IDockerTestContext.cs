namespace grate.unittests.TestInfrastructure;

public interface IDockerTestContext
{
    string DockerCommand(string serverName, string adminPassword);
}