using System.CommandLine;
using System.CommandLine.Builder;
using System.CommandLine.Invocation;
using System.CommandLine.IO;
using System.CommandLine.NamingConventionBinder;
using System.CommandLine.Parsing;
using System.Reflection;
using grate.Commands;
using grate.Configuration;
using grate.Infrastructure;
using grate.mariadb.DependencyInjection;
using grate.Migration;
using grate.oracle.DependencyInjection;
using grate.postgresql.DependencyInjection;
using grate.sqlite.DependencyInjection;
using grate.sqlserver.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;

namespace grate;

public static class Program
{
    private static IServiceProvider _serviceProvider = default!;

    public static async Task<int> Main(string[] args)
    {
        // Temporarily parse the configuration, to get the verbosity level, and potentially set parameters
        // to support the "IsUpToDate" check.
        var cfg = await ParseGrateConfiguration(args);
        if (cfg.UpToDateCheck)
        {
            cfg = cfg with { Verbosity = LogLevel.Critical, DryRun = true };
        }

        _serviceProvider = BuildServiceProvider(cfg).CreateAsyncScope().ServiceProvider;

        var rootCommand = Create<MigrateCommand>();
        rootCommand.Add(Verbosity());

        rootCommand.Description = $"grate v{GetVersion()} - sql for the 20s";

        var parser = new CommandLineBuilder(rootCommand)
            // These are all the CommandLine features enabled by default
            // .UseVersionOption()  //but we don't want version (as we use the --version option ourselves)
            .UseHelp()
            .UseEnvironmentVariableDirective()
            .UseParseDirective()
            .UseSuggestDirective()
            .RegisterWithDotnetSuggest()
            .UseTypoCorrections()
            .UseParseErrorReporting()
            .UseExceptionHandler(ExceptionHandler)
            .CancelOnProcessTermination()
            .Build();

        var result = await parser.InvokeAsync(args);

        await WaitForLoggerToFinish();

        return result;
    }
    
    private static void ExceptionHandler(Exception ex, InvocationContext context)
    {
        // Log the error message at the highest level, and the exception at debug level.
        // Avoids logging the exception stack trace to the end user, if logging level is not set to debug.
        
        var logger = _serviceProvider.GetRequiredService<ILogger<GrateMigrator>>();
        
        context.Console.Error.CreateTextWriter().WriteColoredMessage("An error occurred: ", GrateConsoleColor.Foreground.Red);
        
        logger.LogDebug(ex, "{ErrorMessage}", ex.Message);
        logger.LogError("{ErrorMessage}", ex.Message);
        
        context.ExitCode = 1;
    }
    

    private static string GetVersion() => Assembly.GetEntryAssembly()?.GetName().Version?.ToString() ?? "1.0.0.1";

    private static async Task<CommandLineGrateConfiguration> ParseGrateConfiguration(IReadOnlyList<string> commandline)
    {
        CommandLineGrateConfiguration cfg = new CommandLineGrateConfiguration();
        var handler = CommandHandler.Create((CommandLineGrateConfiguration config) => cfg = config);

        var cmd = new MigrateCommand(null!)
        {
            Verbosity(),
        };

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
        try
        {
            while (ThreadPool.PendingWorkItemCount > 0 && waitedTime < maxWaitTime)
            {
                await Task.Delay(delay);
                waitedTime += delay;
            }
        }
        catch (Exception)
        {
            // We don't want to fail on exit. Nevermind, just exit, and get on with it.
        }
    }

    private static ServiceProvider BuildServiceProvider(CommandLineGrateConfiguration config)
    {
        IServiceCollection services = new ServiceCollection();

        services.AddCliCommands();

        services.AddLogging(logging => logging.AddConsole(options =>
            {
                options.FormatterName = GrateConsoleFormatter.FormatterName;
                options.LogToStandardErrorThreshold = LogLevel.Warning;
            })
            .AddFilter("Grate.Migration.Internal", LogLevel.Critical)
            .AddFilter("Grate.Migration.IsUpToDate", LogLevel.Information)
            .SetMinimumLevel(config.Verbosity)
            .AddConsoleFormatter<GrateConsoleFormatter, SimpleConsoleFormatterOptions>());
        
        services = config.DatabaseType switch
        {
            DatabaseType.MariaDB => services.AddGrateWithMariaDb(config),
            DatabaseType.Oracle => services.AddGrateWithOracle(config),
            DatabaseType.PostgreSQL => services.AddGrateWithPostgreSQL(config),
            DatabaseType.SQLServer => services.AddGrateWithSqlServer(config),
            DatabaseType.SQLite => services.AddGrateWithSqlite(config),
            _ => throw new ArgumentOutOfRangeException(nameof(config), 
                config.DatabaseType, 
                "Unknown target database type: " + config.DatabaseType)
        };

        return services.BuildServiceProvider();
    }

    internal static Option<LogLevel> Verbosity() => new(
        ["-v", "--verbosity"],
        "Verbosity level (as defined here: https://docs.microsoft.com/dotnet/api/Microsoft.Extensions.Logging.LogLevel)");

    private static T Create<T>() where T : notnull => _serviceProvider.GetRequiredService<T>();
}
