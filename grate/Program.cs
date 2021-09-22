using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Builder;
using System.CommandLine.Invocation;
using System.CommandLine.Parsing;
using System.Threading;
using System.Threading.Tasks;
using grate.Commands;
using grate.Configuration;
using grate.Infrastructure;
using grate.Migration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;

namespace grate
{
    public static class Program
    {
        private static ServiceProvider _serviceProvider = default!;

        public static async Task<int> Main(string[] args)
        {
            // Temporarily parse the configuration, to get the verbosity level
            var cfg = await ParseGrateConfiguration(args);
            _serviceProvider = BuildServiceProvider(cfg.Verbosity);
            
            var rootCommand = Create<MigrateCommand>();
            rootCommand.Add(Verbosity());

            rootCommand.Description = "The new grate - sql for the 20s";

            var parser = new CommandLineBuilder(rootCommand)
                // These are all the CommandLine features enabled by default
                // .UseVersionOption()  //but we don't want version (as we use the --version option ourselves)
                .UseHelp()
                .UseEnvironmentVariableDirective()
                .UseParseDirective()
                .UseDebugDirective()
                .UseSuggestDirective()
                .RegisterWithDotnetSuggest()
                .UseTypoCorrections()
                .UseParseErrorReporting()
                .UseExceptionHandler()
                .CancelOnProcessTermination()
                .Build();
            
            var result = await parser.InvokeAsync(args);

            await WaitForLoggerToFinish();

            return result;
        }

        private static async Task<GrateConfiguration> ParseGrateConfiguration(IReadOnlyList<string> commandline)
        {
            GrateConfiguration cfg = GrateConfiguration.Default;
            var handler = CommandHandler.Create((GrateConfiguration config) => cfg = config);

            var cmd = new MigrateCommand(null!) { Verbosity() };

            ParseResult p =
                new Parser(cmd).Parse(commandline);
            await handler.InvokeAsync(new InvocationContext(p));
            return cfg;
        }


        /// <summary>
        /// Wait for logger to be finished - it logs on a different thread, and we
        /// don't want to exit before everything is written to console.
        /// </summary>
        private static async Task WaitForLoggerToFinish()
        {
            var maxWaitTime = 2000;
            var waitedTime = 0;
            var delay = 100;

            await Task.Delay(1);
            while (ThreadPool.PendingWorkItemCount > 0 && waitedTime < maxWaitTime)
            {
                await Task.Delay(delay);
                waitedTime += delay;
            }
        }

        private static ServiceProvider BuildServiceProvider(LogLevel logLevel)
        {
            var services = new ServiceCollection();

            services.AddCliCommands();

            services.AddLogging(logging => logging.AddConsole(options =>
                {
                    options.FormatterName = GrateConsoleFormatter.FormatterName;
                    options.LogToStandardErrorThreshold = LogLevel.Warning;
                })
                .SetMinimumLevel(logLevel)
                .AddConsoleFormatter<GrateConsoleFormatter, SimpleConsoleFormatterOptions>());
         
            services.AddTransient<IDbMigrator, DbMigrator>();
            services.AddTransient<IHashGenerator, HashGenerator>();
            
            services.AddTransient<GrateMigrator, GrateMigrator>();

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

        private static Option<LogLevel> Verbosity() => new(
            new[] { "-v", "--verbosity" }, 
            "Verbosity level (as defined here: https://docs.microsoft.com/dotnet/api/Microsoft.Extensions.Logging.LogLevel)");

        private static T Create<T>() where T : notnull => _serviceProvider.GetRequiredService<T>();
    }
}
