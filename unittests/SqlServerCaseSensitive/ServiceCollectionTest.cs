using grate.Configuration;
using grate.Infrastructure;
using grate.Migration;
using grate.SqlServer;
using grate.SqlServer.Infrastructure;
using grate.SqlServer.Migration;
using Microsoft.Extensions.DependencyInjection;
using SqlServerCaseSensitive.TestInfrastructure;
using TestCommon.Generic.Running_MigrationScripts;
using TestCommon.TestInfrastructure;
using static grate.Configuration.KnownFolderKeys;
namespace SqlServerCaseSensitiveCaseSensitive.DependencyInjection;

[Collection(nameof(SqlServerTestContainer))]
public class ServiceCollectionTest : TestCommon.DependencyInjection.GrateServiceCollectionTest
{
    private readonly SqlServerTestContainer _sqlServerTestContainer;

    public ServiceCollectionTest(SqlServerTestContainer sqlServerTestContainer)
    {
        _sqlServerTestContainer = sqlServerTestContainer;
    }
    protected override void ConfigureService(GrateConfigurationBuilder grateConfigurationBuilder)
    {
        var connectionString = $"Data Source={_sqlServerTestContainer.TestContainer!.Hostname},{_sqlServerTestContainer.TestContainer!.GetMappedPublicPort(_sqlServerTestContainer.Port)};Initial Catalog={TestConfig.RandomDatabase()};User Id=sa;Password={_sqlServerTestContainer.AdminPassword};Encrypt=false;Pooling=false";
        grateConfigurationBuilder.WithConnectionString(connectionString);
        grateConfigurationBuilder.UseSqlServer();
        grateConfigurationBuilder.ServiceCollection.AddSingleton<IDatabaseConnectionFactory, SqlServerConnectionFactory>();
    }

    protected override DirectoryInfo CreateMigrationFolder()
    {
        var sqlFolder = MigrationsScriptsBase.CreateRandomTempDirectory();
        var knownFolders = FoldersConfiguration.Default(null);
        var create_table = @"
                CREATE TABLE grate_test(
                    id int NOT NULL,
                    name varchar(255) NOT NULL,
                    PRIMARY KEY (id)
                )
            ";
        MigrationsScriptsBase.WriteSql(sqlFolder, knownFolders[Up]!.Path, "001_create_test_table.sql", create_table);
        var insert_test_data = @"
                INSERT INTO grate_test(id, name) VALUES(1, 'test');
            ";
        MigrationsScriptsBase.WriteSql(sqlFolder, knownFolders[RunFirstAfterUp]!.Path, "001_insert_test_data.sql", insert_test_data);
        return sqlFolder;

    }

    protected override void ValidateDatabaseService(IServiceCollection serviceCollection)
    {
        ValidateService(serviceCollection, typeof(IDatabase), ServiceLifetime.Transient, typeof(SqlServerDatabase));
        ValidateService(serviceCollection, typeof(ISyntax), ServiceLifetime.Transient, typeof(SqlServerSyntax));
    }
}
