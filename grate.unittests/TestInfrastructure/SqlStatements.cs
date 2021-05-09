namespace grate.unittests.TestInfrastructure
{
    public record SqlStatements
    {
        public string SelectAllDatabases { get; init; } = default!;
        public string SelectVersion { get; init; } = default!;
        public string SelectCurrentDatabase { get; init; } = default!;
    }
}