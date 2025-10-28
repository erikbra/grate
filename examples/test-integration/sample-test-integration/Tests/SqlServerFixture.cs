using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using grate.DependencyInjection;
using grate.Migration;
using grate.sqlserver.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Testcontainers.MsSql;
using Xunit;

namespace Tests;

/// <summary>
/// Helper class to run a SqlServer as part of the tests for migrations etc.  One 'server' per run instance.
/// We run the grate migration as part of standing up the server so we have a fully populated database to test against.
/// </summary>
public sealed class SqlServerFixture : IAsyncLifetime
{
    private static MsSqlContainer SqlServer { get; }

    /// <summary>
    /// The fully privileged sa connection string to our test database
    /// </summary>
    internal static string SysAdminConnString => SqlServer.GetConnectionString().Replace("Database=master", "Database=MyApp_IntegrationTests");
    // Note: When testing real apps we often also have connection strings with application credentials in order to test permissions as part of the tests

    static SqlServerFixture()
    {
        SqlServer = new MsSqlBuilder()
            .WithImage("mcr.microsoft.com/mssql/server:2022-CU12-ubuntu-22.04") // TestContainers recommend running a pinned image for repeatability

            // See https://github.com/testcontainers/testcontainers-dotnet/issues/1220#issuecomment-2248455959
            // uncomment if you have the issue above
            //.WithWaitStrategy(Wait.ForUnixContainer().UntilCommandIsCompleted("/opt/mssql-tools18/bin/sqlcmd", "-C", "-Q", "SELECT 1;"))
            .Build();
    }

    public async Task InitializeAsync()
    {
        await SqlServer.StartAsync();
        Assert.Equal(TestcontainersStates.Running, SqlServer.State);
        await RunDatabaseMigration();
    }

    public async Task DisposeAsync()
    {
        if (SqlServer.State == TestcontainersStates.Running)
        {
            await SqlServer.StopAsync();
        }

        await SqlServer.DisposeAsync();
    }

    private async Task RunDatabaseMigration()
    {
        // migrate database
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddLogging(logBuilder =>
        {
            logBuilder.SetMinimumLevel(LogLevel.Information);
            logBuilder.AddConsole();
        });
        serviceCollection.AddGrate(builder =>
        {
            builder.WithSqlFilesDirectory("./db")
                    .WithConnectionString(SysAdminConnString);
        }).UseSqlServer();
        var serviceProvider = serviceCollection.BuildServiceProvider();
        var logger = serviceProvider.GetRequiredService<ILogger<SqlServerFixture>>();
        logger.LogInformation("Starting migration");
        var grateMigrator = serviceProvider.GetRequiredService<IGrateMigrator>();
        await grateMigrator.Migrate();
    }
}


[CollectionDefinition(nameof(SqlServerCollection))]
public class SqlServerCollection : ICollectionFixture<SqlServerFixture>
{
    // This class has no code, and is never created. Its purpose is simply
    // to be the place to apply [CollectionDefinition] and all the
    // ICollectionFixture<> interfaces so the unit tests can opt in to the database infrastructure.
}
