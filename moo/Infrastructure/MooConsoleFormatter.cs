using System;
using System.IO;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Logging.Console;
using Microsoft.Extensions.Options;

namespace moo.Infrastructure
{
    public class MooConsoleFormatter: ConsoleFormatter, IDisposable
    {
        public const string FormatterName = "moo-output";
        private readonly IDisposable? _optionsReloadToken;
        
        public MooConsoleFormatter(IOptionsMonitor<SimpleConsoleFormatterOptions>? options) : base(FormatterName)
        {
            if (options != null)
            {
                ReloadLoggerOptions(options.CurrentValue);
                _optionsReloadToken = options.OnChange(ReloadLoggerOptions);
            }
        }

        public override void Write<TState>(in LogEntry<TState> logEntry, IExternalScopeProvider scopeProvider, TextWriter textWriter)
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
        
        internal SimpleConsoleFormatterOptions? FormatterOptions { get; set; }

        public void Dispose()
        {
            _optionsReloadToken?.Dispose();
        }
        
        private void CreateDefaultLogMessage<TState>(TextWriter textWriter, in LogEntry<TState> logEntry, string? message)
        {
            Exception? exception = logEntry.Exception;
            
            LogLevel logLevel = logEntry.LogLevel;
            ConsoleColors logLevelColors = GetLogLevelConsoleColors(logLevel);
        
            textWriter.WriteColoredMessageLine(message, logLevelColors.Background, logLevelColors.Foreground);
        
            if (exception != null)
            {
                textWriter.WriteColoredMessageLine(exception.ToString(), logLevelColors.Background, logLevelColors.Foreground);
            }
            textWriter.Flush();
        }

        private ConsoleColors GetLogLevelConsoleColors(LogLevel logLevel)
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
                LogLevel.Trace => new ConsoleColors(MooConsoleColor.Foreground.Gray, MooConsoleColor.Background.Black),
                LogLevel.Debug => new ConsoleColors(MooConsoleColor.Foreground.Gray, MooConsoleColor.Background.Black),
                LogLevel.Information => new ConsoleColors(MooConsoleColor.Foreground.Green, MooConsoleColor.Background.Black),
                LogLevel.Warning => new ConsoleColors(MooConsoleColor.Foreground.Yellow, MooConsoleColor.Background.Black),
                LogLevel.Error => new ConsoleColors(MooConsoleColor.Foreground.Black, MooConsoleColor.Background.DarkRed),
                LogLevel.Critical => new ConsoleColors(MooConsoleColor.Foreground.White, MooConsoleColor.Background.DarkRed),
                _ => ConsoleColors.None
            };
        }
        
        
        private readonly struct ConsoleColors
        {
            public ConsoleColors(MooConsoleColor foreground, MooConsoleColor background)
            {
                Foreground = foreground;
                Background = background;
            }
        
            public MooConsoleColor Foreground { get; }
            public MooConsoleColor Background { get; }

            public static ConsoleColors None => new ConsoleColors();
        }
    }
}