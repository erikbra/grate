namespace Unit_tests.TestInfrastructure;

public record SqlStatements
{
    public string SelectVersion { get; init; } = default!;
    public string SleepTwoSeconds { get; init; } = default!;
    public string CreateUser { get; init; } = default!;
    public string GrantAccess { get; init; } = default!;
    public string LineComment { get; init; } = "--";
}
