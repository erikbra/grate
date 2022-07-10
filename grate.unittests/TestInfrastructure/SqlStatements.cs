namespace grate.unittests.TestInfrastructure;

public record SqlStatements
{
    public string SelectVersion { get; init; } = default!;
    public string SleepTwoSeconds { get; init; } = default!;
}
