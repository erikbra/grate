using System;
using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;
using NUnit.Framework;

namespace grate.unittests.TestInfrastructure
{
    public abstract class TestContextBase : IDisposable
    {
        public ILoggerFactory LogFactory = LoggerFactory.Create(builder =>
        {
            builder.AddProvider(new NUnitLoggerProvider())
                   .SetMinimumLevel(LogLevel.Trace);
        });

        public void Dispose()
        {
            LogFactory.Dispose();
            GC.SuppressFinalize(this);
        }
    }

    public class NUnitLogger : ILogger
    {
        private readonly string _name;

        public NUnitLogger(string name) => _name = name;

        public IDisposable BeginScope<TState>(TState state) => default;

        public bool IsEnabled(LogLevel logLevel) => true;

        public void Log<TState>(
            LogLevel logLevel,
            EventId eventId,
            TState state,
            Exception exception,
            Func<TState, Exception, string> formatter)
        {
            if (!IsEnabled(logLevel))
            {
                return;
            }

            TestContext.Progress.WriteLine($"     {_name} - {formatter(state, exception)}");

        }
    }

    public class NUnitLoggerProvider : ILoggerProvider
    {
        public NUnitLoggerProvider() { }
        //public NUnitLoggerProvider(EventHandler onCreateLogger)
        //{
        //    OnCreateLogger = onCreateLogger;
        //}

        public ConcurrentDictionary<string, NUnitLogger> Loggers { get; set; } = new ConcurrentDictionary<string, NUnitLogger>();

        public ILogger CreateLogger(string categoryName)
        {
            NUnitLogger customLogger = Loggers.GetOrAdd(categoryName, new NUnitLogger(categoryName));
            //OnCreateLogger?.Invoke(this, new NUnitLoggerProvider());
            return customLogger;
        }

        public void Dispose() { GC.SuppressFinalize(this); }

        public event EventHandler OnCreateLogger = delegate { };
    }
}
