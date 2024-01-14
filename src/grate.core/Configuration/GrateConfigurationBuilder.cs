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
    /// <returns>GrateConfigurationBuilder</returns>
    public GrateConfigurationBuilder WithOutputFolder(DirectoryInfo outputFolder)
    {
        _grateConfiguration = _grateConfiguration with { OutputPath = outputFolder };
        return this;
    }
    /// <summary>
    /// Specify the bakups folder after migrated.
    /// </summary>
    /// <param name="outputFolder">Target folder to store all backups</param>
    /// <returns>GrateConfigurationBuilder</returns>
    public GrateConfigurationBuilder WithOutputFolder(string outputFolder)
    {
        WithOutputFolder(new DirectoryInfo(outputFolder));
        return this;
    }

    /// <summary>
    /// Specify the schema name to use for the migration.
    /// </summary>
    /// <param name="schemaName"></param>
    /// <returns>GrateConfigurationBuilder</returns>
    public GrateConfigurationBuilder WithSchema(string schemaName)
    {
        _grateConfiguration = _grateConfiguration with { SchemaName = schemaName };
        return this;
    }

    /// <summary>
    /// Specify the folder where the migration scripts are located.
    /// </summary>
    /// <param name="sqlFilesDirectory">Directory contains the grate subfolder.</param>
    /// <returns>GrateConfigurationBuilder</returns>
    public GrateConfigurationBuilder WithSqlFilesDirectory(DirectoryInfo sqlFilesDirectory)
    {
        _grateConfiguration = _grateConfiguration with { SqlFilesDirectory = sqlFilesDirectory };
        return this;
    }

    /// <summary>
    /// Specify the folder where the migration scripts are located.
    /// </summary>
    /// <param name="sqlFilesDirectory">Directory contains the grate subfolder.</param>
    /// <returns>GrateConfigurationBuilder</returns>
    public GrateConfigurationBuilder WithSqlFilesDirectory(string sqlFilesDirectory)
    {
        WithSqlFilesDirectory(new DirectoryInfo(sqlFilesDirectory));
        return this;
    }

    /// <summary>
    /// Connection string to use when connecting to the database.
    /// </summary>
    /// <param name="connectionString">Database connection string</param>
    /// <returns>GrateConfigurationBuilder</returns>
    public GrateConfigurationBuilder WithConnectionString(string connectionString)
    {
        _grateConfiguration = _grateConfiguration with { ConnectionString = connectionString };
        return this;
    }

    /// <summary>
    /// Connection string with admin privilege. Use to create new database if needed
    /// </summary>
    /// <param name="adminConnectionString">Admin connection string</param>
    /// <returns>GrateConfigurationBuilder</returns>
    public GrateConfigurationBuilder WithAdminConnectionString(string adminConnectionString)
    {
        _grateConfiguration = _grateConfiguration with { AdminConnectionString = adminConnectionString };
        return this;
    }

    /// <summary>
    /// Version of service to use. Grate will store the version in the database and use it to determine which scripts to run.
    /// </summary>
    /// <param name="version">service migration version</param>
    /// <returns>GrateConfigurationBuilder</returns>
    public GrateConfigurationBuilder WithVersion(string version)
    {
        _grateConfiguration = _grateConfiguration with { Version = version };
        return this;
    }

    /// <summary>
    /// Tell grate do not create database.
    /// </summary>
    /// <returns>GrateConfigurationBuilder</returns>
    public GrateConfigurationBuilder DoNotCreateDatabase()
    {
        _grateConfiguration = _grateConfiguration with { CreateDatabase = false };
        return this;
    }

    /// <summary>
    /// Database type to use.
    /// </summary>
    /// <param name="databaseType">Database type</param>
    /// <returns>GrateConfigurationBuilder</returns>
    public GrateConfigurationBuilder WithDatabaseType(string databaseType)
    {
        _grateConfiguration = _grateConfiguration with { DatabaseType = databaseType };
        return this;
    }

    /// <summary>
    /// Run the migration in a transaction.
    /// </summary>
    /// <returns>GrateConfigurationBuilder</returns>
    public GrateConfigurationBuilder WithTransaction()
    {
        _grateConfiguration = _grateConfiguration with { Transaction = true };
        return this;
    }
    /// <summary>
    /// Set migration environment.
    /// </summary>
    /// <param name="environmentName">Environment name to run</param>
    /// <returns>GrateConfigurationBuilder</returns>
    public GrateConfigurationBuilder WithEnvironment(string environmentName)
    {
        _grateConfiguration = _grateConfiguration with { Environment = new GrateEnvironment(environmentName) };
        return this;
    }
}
