using grate.Configuration;
using grate.Infrastructure;
using grate.Migration;
using grate.SqlServer.Infrastructure;
using grate.SqlServer.Migration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SqlServer.TestInfrastructure;
using TestCommon.TestInfrastructure;

namespace SqlServer;

// ReSharper disable once UnusedType.Global
public class Startup
{
    // ReSharper disable once UnusedMember.Global
    public void ConfigureServices(IServiceCollection services, HostBuilderContext context)
    {
        services
            .AddLogging(
                lb => lb
                    .AddXUnit()
                    .AddConsole()
                    .SetMinimumLevel(TestConfig.GetLogLevel())
            )
            .AddSingleton<SqlServerTestContainer>()
            .AddSingleton<IGrateMigrator, GrateMigrator>()
            .AddSingleton<Func<GrateConfiguration, GrateMigrator>>(provider =>
            config =>
            {
                IDatabase database = provider.GetRequiredService<IDatabase>();
                
                ILogger<DbMigrator> dbLogger = provider.GetRequiredService<ILogger<DbMigrator>>();
                ILogger<GrateMigrator> grateLogger = provider.GetRequiredService<ILogger<GrateMigrator>>();
                
                IHashGenerator hashGenerator = provider.GetRequiredService<IHashGenerator>();
                return 
                    new GrateMigrator(grateLogger,
                    new DbMigrator(database, dbLogger, hashGenerator, config));
            })
            .AddSingleton<IHashGenerator, HashGenerator>()
            
            .AddTransient<IGrateTestContext, SqlServerGrateTestContext>()
            
            .AddTransient<IDatabase, SqlServerDatabase>()
            .AddSingleton<ISyntax, SqlServerSyntax>()
            .AddSingleton<StatementSplitter>(provider => new StatementSplitter(
                provider.GetRequiredService<ISyntax>().StatementSeparatorRegex))
            .AddSingleton<BatchSplitterReplacer>(provider =>
            {
                var database = provider.GetService<IDatabase>()!;
                return new BatchSplitterReplacer(database.StatementSeparatorRegex, StatementSplitter.BatchTerminatorReplacementString);
            })
            .AddSingleton<IDatabaseConnectionFactory, SqlServerConnectionFactory>()
            .AddTransient<InspectableSqlServerDatabase>()
            ;

    }
}
