using System;
using System.CommandLine;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using moo.Commands;
using moo.Configuration;
using moo.Infrastructure;
using moo.Migration;

namespace moo
{
    public static class Program
    {
        private static readonly ServiceProvider ServiceProvider = BuildServiceProvider();
        private static bool _verbose = true;

        public static async Task<int> Main(string[] args)
        {
            var rootCommand = Create<MigrateCommand>();
            rootCommand.Add(Verbose());
            
            rootCommand.Description = "The new moo - sql for the 20s";
            return await rootCommand.InvokeAsync(args);
        }
        
        private static void SetVerbose(bool verbose) => _verbose = verbose;
        
        private static ServiceProvider BuildServiceProvider()
        {
            var services = new ServiceCollection();
            
            services.AddCliCommands();
            
            services.AddLogging(logging => logging.AddConsole(
                    options =>
                    {
                        options.FormatterName = MooConsoleFormatter.FormatterName;
                    }).SetMinimumLevel(_verbose ? LogLevel.Trace: LogLevel.Information)
                .AddConsoleFormatter<MooConsoleFormatter, SimpleConsoleFormatterOptions>());
            
            // services.AddLogging(logging =>
            //     logging
            //         .AddConsole()
            //         .SetMinimumLevel(_verbose ? LogLevel.Trace : LogLevel.Information));
            
            services.AddTransient<IDbMigrator, DbMigrator>();

            services.AddTransient<SqlServerDatabase>();
            services.AddTransient<OracleDatabase>();

            services.AddTransient<IFactory>(serviceProvider =>
            {
                var fac = new Factory(serviceProvider);
                fac.AddService(DatabaseType.sqlserver, typeof(SqlServerDatabase));
                fac.AddService(DatabaseType.oracle, typeof(OracleDatabase));

                return fac;
            });
             
          
            return services.BuildServiceProvider();
        }
        
        private static Option<bool> Verbose() => new(new[] {"-v", "--verbose"}, "Verbose output");
        
        private static T Create<T>() where T: notnull => ServiceProvider.GetRequiredService<T>();
    }
}
