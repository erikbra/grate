using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Logging.Console;
using Microsoft.Extensions.Options;

namespace grate.Infrastructure;

internal class GrateConsoleFormatter : ConsoleFormatter, IDisposable
{
    public const string FormatterName = "grate-output";
    private readonly IDisposable? _optionsReloadToken;

    public GrateConsoleFormatter(IOptionsMonitor<SimpleConsoleFormatterOptions>? options) : base(FormatterName)
    {
        if (options != null)
        {
            ReloadLoggerOptions(options.CurrentValue);
            _optionsReloadToken = options.OnChange(ReloadLoggerOptions);
        }
    }

    public override void Write<TState>(in LogEntry<TState> logEntry, IExternalScopeProvider? scopeProvider, TextWriter textWriter)
    {
        string? message = logEntry.Formatter?.Invoke(logEntry.State, logEntry.Exception);
        if (logEntry.Exception == null && message == null)
        {
            return;
        }

        CreateDefaultLogMessage(textWriter, logEntry, message);
    }

    private void ReloadLoggerOptions(SimpleConsoleFormatterOptions options)
    {
        FormatterOptions = options;
    }

    private SimpleConsoleFormatterOptions? FormatterOptions { get; set; }

    public void Dispose()
    {
        _optionsReloadToken?.Dispose();
        GC.SuppressFinalize(this);
    }

    private static void CreateDefaultLogMessage<TState>(TextWriter textWriter, in LogEntry<TState> logEntry, string? message)
    {
        Exception? exception = logEntry.Exception;

        LogLevel logLevel = logEntry.LogLevel;
        ConsoleColors logLevelColors = GetLogLevelConsoleColors(logLevel);

        textWriter.WriteColoredMessageLine(message, logLevelColors.Foreground, logLevelColors.Background);

        if (exception != null)
        {
            textWriter.WriteColoredMessageLine(exception.ToString(), logLevelColors.Foreground, logLevelColors.Background);
        }
        textWriter.Flush();
    }

    private static ConsoleColors GetLogLevelConsoleColors(LogLevel logLevel)
    {
        bool disableColors = Console.IsOutputRedirected;

        if (disableColors)
        {
            return ConsoleColors.None;
        }

        // We must explicitly set the background color if we are setting the foreground color,
        // since just setting one can look bad on the users console.
        return logLevel switch
        {
            LogLevel.Trace => new ConsoleColors(GrateConsoleColor.Foreground.DarkYellow),
            LogLevel.Debug => new ConsoleColors(GrateConsoleColor.Foreground.DarkGray),
            LogLevel.Information => new ConsoleColors(GrateConsoleColor.Foreground.Green),
            LogLevel.Warning => new ConsoleColors(GrateConsoleColor.Foreground.Yellow),
            LogLevel.Error => new ConsoleColors(GrateConsoleColor.Foreground.DarkRed),
            LogLevel.Critical => new ConsoleColors(GrateConsoleColor.Foreground.Cyan),
            _ => ConsoleColors.None
        };
    }


    private readonly struct ConsoleColors(GrateConsoleColor foreground, GrateConsoleColor? background = null)
    {
        public GrateConsoleColor Foreground { get; } = foreground;
        public GrateConsoleColor? Background { get; } = background;

        public static ConsoleColors None => new();
    }
}
