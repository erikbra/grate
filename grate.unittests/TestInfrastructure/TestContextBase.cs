using System;
using Microsoft.Extensions.Logging;

namespace grate.unittests.TestInfrastructure;

public abstract class TestContextBase : IDisposable
{
    public ILoggerFactory LogFactory = LoggerFactory.Create(builder =>
    {
        builder.AddProvider(new NUnitLoggerProvider())
               .SetMinimumLevel(LogLevel.Trace); // TODO: Make this level configurable somehow...
    });

    public void Dispose()
    {
        LogFactory.Dispose();
        GC.SuppressFinalize(this);
    }
}


