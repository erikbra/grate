using grate.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace grate.Configuration;

public sealed class GrateConfigurationBuilder
{
    public IServiceCollection ServiceCollection => _grateConfiguration.ServiceCollection!;
    public GrateConfigurationBuilder(IServiceCollection serviceCollection, GrateConfiguration presetGrateConfiguration)
    {
        _grateConfiguration = presetGrateConfiguration with { ServiceCollection = serviceCollection };
    }
    private GrateConfiguration _grateConfiguration { get; set; }

    /// <summary>
    /// Build the grate configuration.
    /// </summary>
    /// <returns>GrateConfiguration</returns>
    public GrateConfiguration Build()
    {
        return _grateConfiguration;
    }

    /// <summary>
    /// Specify the bakups folder after migrated.
    /// </summary>
    /// <param name="outputFolder">Target folder to store all backups</param>
    public void WithOutputFolder(DirectoryInfo outputFolder)
    {
        _grateConfiguration = _grateConfiguration with { OutputPath = outputFolder };
    }
    /// <summary>
    /// Specify the bakups folder after migrated.
    /// </summary>
    /// <param name="outputFolder">Target folder to store all backups</param>
    public void WithOutputFolder(string outputFolder)
    {
        WithOutputFolder(new DirectoryInfo(outputFolder));
    }

    /// <summary>
    /// Specify the schema name to use for the migration.
    /// </summary>
    /// <param name="schemaName"></param>
    public void WithSchema(string schemaName)
    {
        _grateConfiguration = _grateConfiguration with { SchemaName = schemaName };
    }

    /// <summary>
    /// Specify the folder where the migration scripts are located.
    /// </summary>
    /// <param name="sqlFilesDirectory">Directory contains the grate subfolder.</param>
    public void WithSqlFilesDirectory(DirectoryInfo sqlFilesDirectory)
    {
        _grateConfiguration = _grateConfiguration with { SqlFilesDirectory = sqlFilesDirectory };
    }

    /// <summary>
    /// Specify the folder where the migration scripts are located.
    /// </summary>
    /// <param name="sqlFilesDirectory">Directory contains the grate subfolder.</param>
    public void WithSqlFilesDirectory(string sqlFilesDirectory)
    {
        WithSqlFilesDirectory(new DirectoryInfo(sqlFilesDirectory));
    }

    /// <summary>
    /// Connection string to use when connecting to the database.
    /// </summary>
    /// <param name="connectionString">Database connection string</param>
    public void WithConnectionString(string connectionString)
    {
        _grateConfiguration = _grateConfiguration with { ConnectionString = connectionString };
    }

    /// <summary>
    /// Connection string with admin privilege. Use to create new database if needed
    /// </summary>
    /// <param name="adminConnectionString">Admin connection string</param>
    public void WithAdminConnectionString(string adminConnectionString)
    {
        _grateConfiguration = _grateConfiguration with { AdminConnectionString = adminConnectionString };
    }

    /// <summary>
    /// Version of service to use. Grate will store the version in the database and use it to determine which scripts to run.
    /// </summary>
    /// <param name="version">service migration version</param>
    public void WithVersion(string version)
    {
        _grateConfiguration = _grateConfiguration with { Version = version };
    }

    /// <summary>
    /// Tell grate do not create database.
    /// </summary>
    public void DoNotCreateDatabase()
    {
        _grateConfiguration = _grateConfiguration with { CreateDatabase = false };
    }

    /// <summary>
    /// Database type to use.
    /// </summary>
    /// <param name="databaseType">Database type</param>
    public void WithDatabaseType(string databaseType)
    {
        _grateConfiguration = _grateConfiguration with { DatabaseType = databaseType };
    }

    /// <summary>
    /// Run the migration in a transaction.
    /// </summary>
    public void WithTransaction()
    {
        _grateConfiguration = _grateConfiguration with { Transaction = true };
    }

    /// <summary>
    /// Set migration environment.
    /// </summary>
    /// <param name="environmentName">Environment name to run</param>
    public void WithEnvironment(string environmentName)
    {
        _grateConfiguration = _grateConfiguration with { Environment = new GrateEnvironment(environmentName) };
    }
}
