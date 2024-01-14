using grate.Configuration;
using grate.Infrastructure;
using grate.Migration;
using grate.PostgreSql.Infrastructure;
using grate.PostgreSql.Migration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PostgreSQL.TestInfrastructure;
using TestCommon.TestInfrastructure;

namespace PostgreSQL;

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
            .AddSingleton<PostgreSqlTestContainer>()
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
            
            .AddTransient<IGrateTestContext, PostgreSqlGrateTestContext>()
            
            .AddTransient<IDatabase, PostgreSqlDatabase>()
            .AddSingleton<ISyntax, PostgreSqlSyntax>()
            .AddSingleton<IDatabaseConnectionFactory, PostgreSqlConnectionFactory>()
            .AddSingleton<StatementSplitter>()
            .AddSingleton(service =>
            {
                var database = service.GetService<IDatabase>()!;
                return new StatementSplitter(database.StatementSeparatorRegex);
            });
            ;

    }
}
