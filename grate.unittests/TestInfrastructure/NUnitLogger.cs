using System;
using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;
using NUnit.Framework;

namespace grate.unittests.TestInfrastructure;

#pragma warning disable CS8603 // Possible null reference return.
/// <summary>
/// Helper logger to push all ILogger output to the NUnit TestContext so it's avail in the test logs
/// </summary>
public class NUnitLogger : ILogger
{
    private readonly string _name;

    public NUnitLogger(string name) => _name = name;

    public IDisposable BeginScope<TState>(TState state) => default; //We don't have scoping support

    public bool IsEnabled(LogLevel logLevel) => true;

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

        TestContext.Progress.WriteLine($"     {_name} - {formatter(state, exception)}");
    }
}
#pragma warning restore CS8603 // Possible null reference return.

public class NUnitLoggerProvider : ILoggerProvider
{
    private ConcurrentDictionary<string, NUnitLogger> Loggers { get; } = new();

    public ILogger CreateLogger(string categoryName)
    {
        NUnitLogger customLogger = Loggers.GetOrAdd(categoryName, new NUnitLogger(categoryName));
        return customLogger;
    }

    public void Dispose() { GC.SuppressFinalize(this); }

    public event EventHandler OnCreateLogger = delegate { };
}
