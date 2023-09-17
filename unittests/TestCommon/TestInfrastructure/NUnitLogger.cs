using System;
using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;
using NUnit.Framework;

namespace TestCommon.TestInfrastructure;

#pragma warning disable CS8603 // Possible null reference return.
/// <summary>
/// Helper logger to push all ILogger output to the NUnit TestContext so it's avail in the test logs
/// </summary>
public class NUnitLogger : ILogger
{
    private readonly string _name;
    private readonly LogLevel _minimumLogLevel;

    public NUnitLogger(string name, LogLevel minimumLogLevel)
    {
        _name = name;
        _minimumLogLevel = minimumLogLevel;
    }

    public IDisposable BeginScope<TState>(TState state) where TState : notnull => default; //We don't have scoping support

    public bool IsEnabled(LogLevel logLevel) => _minimumLogLevel >= logLevel  ;

    public void Log<TState>(
        LogLevel logLevel,
        EventId eventId,
        TState state,
        Exception? exception,
        Func<TState, Exception?, string> formatter)
    {
        if (!IsEnabled(logLevel))
        {
            return;
        }

        var shortName = $"[{Shorten(_name)}]";
        var ts = TimeOnly.FromDateTime(DateTime.Now).ToString("o");
        
        TestContext.Progress.WriteLine($"[{ts}] {shortName,-25} {formatter(state, exception)}");
    }

    private static string Shorten(string name) => name.Replace("grate.Migration.", "");
}
#pragma warning restore CS8603 // Possible null reference return.

public class NUnitLoggerProvider : ILoggerProvider
{
    private readonly LogLevel _minimumLogLevel;

    public NUnitLoggerProvider(LogLevel minimumLogLevel)
    {
        _minimumLogLevel = minimumLogLevel;
    }

    private ConcurrentDictionary<string, NUnitLogger> Loggers { get; } = new();

    public ILogger CreateLogger(string categoryName)
    {
        NUnitLogger customLogger = Loggers.GetOrAdd(categoryName, new NUnitLogger(categoryName, _minimumLogLevel));
        return customLogger;
    }

    public void Dispose() { GC.SuppressFinalize(this); }

    public event EventHandler OnCreateLogger = delegate { };
}
