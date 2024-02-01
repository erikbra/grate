using Basic_tests.Infrastructure.MariaDB;
using grate.Configuration;
using grate.Infrastructure;
using grate.MariaDb.Infrastructure;
using grate.MariaDb.Migration;
using grate.Migration;
using MariaDB.TestInfrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using TestCommon.TestInfrastructure;

namespace MariaDB;

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
            .AddSingleton<MariaDbTestContainer>()
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
            
            .AddTransient<IGrateTestContext, MariaDbGrateTestContext>()
            //.AddTransient<MariaDbGrateTestContext>()
            
            .AddTransient<IDatabase, MariaDbDatabase>()
            .AddSingleton<ISyntax, MariaDbSyntax>()
            .AddSingleton<IDatabaseConnectionFactory, MariaDbConnectionFactory>()
            .AddTransient<InspectableMariaDbDatabase>()
            ;

    }
}
