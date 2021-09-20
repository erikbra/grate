using System;
using System.CommandLine;
using System.CommandLine.Builder;
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
        private static readonly ServiceProvider ServiceProvider = BuildServiceProvider();
        private static bool _verbose = true;

        public static async Task<int> Main(string[] args)
        {
            var rootCommand = Create<MigrateCommand>();
            rootCommand.Add(Verbose());

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

        private static void SetVerbose(bool verbose) => _verbose = verbose;

        private static ServiceProvider BuildServiceProvider()
        {
            var services = new ServiceCollection();

            services.AddCliCommands();

            services.AddLogging(logging => logging.AddConsole(
                    options =>
                    {
                        options.FormatterName = GrateConsoleFormatter.FormatterName;
                    }).SetMinimumLevel(_verbose ? LogLevel.Trace : LogLevel.Information)
                .AddConsoleFormatter<GrateConsoleFormatter, SimpleConsoleFormatterOptions>());

            // services.AddLogging(logging =>
            //     logging
            //         .AddConsole()
            //         .SetMinimumLevel(_verbose ? LogLevel.Trace : LogLevel.Information));

            services.AddTransient<IDbMigrator, DbMigrator>();
            services.AddTransient<IHashGenerator, HashGenerator>();

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

        private static Option<bool> Verbose() => new(new[] { "-v", "--verbose" }, "Verbose output");

        private static T Create<T>() where T : notnull => ServiceProvider.GetRequiredService<T>();
    }
}
