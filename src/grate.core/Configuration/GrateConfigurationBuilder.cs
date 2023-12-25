using Microsoft.Extensions.DependencyInjection;

namespace grate.Configuration;

public class GrateConfigurationBuilder
{
    public IServiceCollection ServiceCollection => _grateConfiguration.ServiceCollection!;
    public GrateConfigurationBuilder(IServiceCollection serviceCollection, GrateConfiguration presetGrateConfiguration)
    {
        _grateConfiguration = presetGrateConfiguration with { ServiceCollection = serviceCollection };
    }
    private GrateConfiguration _grateConfiguration { get; set; }
    public GrateConfiguration Build()
    {
        return _grateConfiguration;
    }
    public void WithFolder(IFoldersConfiguration folders)
    {
        _grateConfiguration = _grateConfiguration with { Folders = folders };
    }
    public void WithSqlFilesDirectory(DirectoryInfo sqlFilesDirectory)
    {
        _grateConfiguration = _grateConfiguration with { SqlFilesDirectory = sqlFilesDirectory };
    }
    public void WithConnectionString(string connectionString)
    {
        _grateConfiguration = _grateConfiguration with { ConnectionString = connectionString };
    }
    public void WithAdminConnectionString(string adminConnectionString)
    {
        _grateConfiguration = _grateConfiguration with { AdminConnectionString = adminConnectionString };
    }
    public void WithVersion(string version)
    {
        _grateConfiguration = _grateConfiguration with { Version = version };
    }
    public void CreateDatabase(bool createDatabase)
    {
        _grateConfiguration = _grateConfiguration with { CreateDatabase = createDatabase };
    }
    public void WithDatabaseType(string databaseType)
    {
        _grateConfiguration = _grateConfiguration with { DatabaseType = databaseType };
    }
    public void Interactive()
    {
        _grateConfiguration = _grateConfiguration with { NonInteractive = false };
    }
}
