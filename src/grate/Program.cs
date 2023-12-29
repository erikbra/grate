﻿using System.CommandLine;
using System.CommandLine.Builder;
using System.CommandLine.Invocation;
using System.CommandLine.NamingConventionBinder;
using System.CommandLine.Parsing;
using System.Reflection;
using grate.Commands;
using grate.Configuration;
using grate.Exceptions;
using grate.Infrastructure;
using grate.MariaDb;
using grate.MariaDb.Migration;
using grate.Migration;
using grate.Oracle;
using grate.Oracle.Migration;
using grate.PostgreSql;
using grate.PostgreSql.Migration;
using grate.Sqlite;
using grate.Sqlite.Migration;
using grate.SqlServer;
using grate.SqlServer.Migration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
namespace grate;

public static class Program
{
    private static ServiceProvider _serviceProvider = default!;

    public static async Task<int> Main(string[] args)
    {
        // Temporarily parse the configuration, to get the verbosity level
        var cfg = await ParseGrateConfiguration(args);
        _serviceProvider = BuildServiceProvider(cfg);

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
            .UseExceptionHandler()
            .CancelOnProcessTermination()
            .Build();

        int result = -1;

        try
        {
            result = await parser.InvokeAsync(args);
        }
        catch (MigrationFailed ex)
        {
            var logger = _serviceProvider.GetRequiredService<ILogger<GrateMigrator>>();
            logger.LogError(ex, "{ErrorMessage}", ex.Message);
        }

        await WaitForLoggerToFinish();

        return result;
    }

    private static string GetVersion() => Assembly.GetEntryAssembly()?.GetName().Version?.ToString() ?? "1.0.0.1";

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

    private static ServiceProvider BuildServiceProvider(GrateConfiguration config)
    {
        var services = new ServiceCollection();

        services.AddCliCommands();


        services.AddLogging(logging => logging.AddConsole(options =>
            {
                options.FormatterName = GrateConsoleFormatter.FormatterName;
                options.LogToStandardErrorThreshold = LogLevel.Warning;
            })
            .SetMinimumLevel(config.Verbosity)
            .AddConsoleFormatter<GrateConsoleFormatter, SimpleConsoleFormatterOptions>());
        services.AddGrate(config, builder =>
        {
            switch (config.DatabaseType)
            {
                case MariaDbDatabase.Type:
                    builder.UseMariaDb();
                    break;
                case OracleDatabase.Type:
                    builder.UseOracle();
                    break;
                case PostgreSqlDatabase.Type:
                    builder.UsePostgreSql();
                    break;
                case SqlServerDatabase.Type:
                    builder.UseSqlServer();
                    break;
                case SqliteDatabase.Type:
                    builder.UseSqlite();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(config), config.DatabaseType, "Unknown the target database type");
            }
        });
        //services.AddSingleton(config);

        // services.AddTransient<IDbMigrator, DbMigrator>();
        // services.AddTransient<IHashGenerator, HashGenerator>();

        // services.AddTransient<IGrateMigrator, GrateMigrator>();

        // services.AddTransient<grate.MariaDb.Migration.MariaDbDatabase>();
        // services.AddTransient<grate.Oracle.Migration.OracleDatabase>();
        // services.AddTransient<grate.PostgreSql.Migration.PostgreSqlDatabase>();
        // services.AddTransient<grate.SqlServer.Migration.SqlServerDatabase>();
        // services.AddTransient<grate.Sqlite.Migration.SqliteDatabase>();

        // services.AddTransient<IFactory>(serviceProvider =>
        // {
        //     var fac = new Factory(serviceProvider);

        //     fac.AddService(grate.MariaDb.Migration.MariaDbDatabase.Type, typeof(grate.MariaDb.Migration.MariaDbDatabase));
        //     fac.AddService(grate.Oracle.Migration.OracleDatabase.Type, typeof(grate.Oracle.Migration.OracleDatabase));
        //     fac.AddService(grate.PostgreSql.Migration.PostgreSqlDatabase.Type, typeof(grate.PostgreSql.Migration.PostgreSqlDatabase));
        //     fac.AddService(grate.SqlServer.Migration.SqlServerDatabase.Type, typeof(grate.SqlServer.Migration.SqlServerDatabase));
        //     fac.AddService(grate.Sqlite.Migration.SqliteDatabase.Type, typeof(grate.Sqlite.Migration.SqliteDatabase));

        //     return fac;
        // });


        return services.BuildServiceProvider();
    }

    private static Option<LogLevel> Verbosity() => new(
        new[] { "-v", "--verbosity" },
        "Verbosity level (as defined here: https://docs.microsoft.com/dotnet/api/Microsoft.Extensions.Logging.LogLevel)");

    private static T Create<T>() where T : notnull => _serviceProvider.GetRequiredService<T>();
}
