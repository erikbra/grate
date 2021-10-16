using System;
using Microsoft.Extensions.Logging;

namespace grate.unittests.TestInfrastructure
{
    public static class TestConfig
    {
        private static readonly Random Random = new();

        public static string RandomDatabase() => Random.GetString(15);
        
        public static readonly ILoggerFactory LogFactory = LoggerFactory.Create(builder =>
        {
            builder.AddProvider(new NUnitLoggerProvider())
                .SetMinimumLevel(GetLogLevel());
        });
    
        private static LogLevel GetLogLevel()
        {
            if (!Enum.TryParse(Environment.GetEnvironmentVariable("LogLevel"), out LogLevel logLevel))
            {
                logLevel = LogLevel.Trace;
            }
            return logLevel;
        }
    }
}
