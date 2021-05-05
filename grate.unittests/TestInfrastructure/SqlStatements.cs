namespace grate.unittests.TestInfrastructure
{
    public record SqlStatements
    {
        public string SelectAllDatabases { get; init; }
        public string SelectVersion { get; init; }
        public string SelectCurrentDatabase { get; init; }
    }
}