namespace TestCommon.TestInfrastructure;

public record GrateTestConfig
{
    public string? AdminConnectionString { get; set; } = null!;
    public string? DockerImage { get; set; } = null!;
    public bool ConnectToDockerInternal { get; set; }
}
