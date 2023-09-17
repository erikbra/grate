namespace Unit_tests.TestInfrastructure;

public interface IDockerTestContext
{
    string DockerCommand(string serverName, string adminPassword);

    /// <summary>
    /// The port in the container to find the HostPort to (e.g. 1433 for SQL server, 1521 for Oracle).
    /// If not set, we just assume the first port that is exposed from the container is the one.
    /// </summary>
    /// <returns></returns>
    int? ContainerPort { get; }
}
