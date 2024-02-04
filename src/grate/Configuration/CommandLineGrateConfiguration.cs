namespace grate.Configuration;

internal record CommandLineGrateConfiguration : GrateConfiguration
{
    /// <summary>
    /// Database type to use.
    /// </summary>
    public DatabaseType DatabaseType { get; set; }
    
}
