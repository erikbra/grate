using System;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace grate.unittests.TestInfrastructure;

public abstract class TestContextBase : IDisposable
{
    public ILoggerFactory LogFactory = LoggerFactory.Create(builder =>
    {
        builder.AddProvider(new NUnitLoggerProvider())
               .SetMinimumLevel(LogLevel.None); // TODO: Make this level configurable somehow...
    });

    public void Dispose()
    {
        LogFactory.Dispose();
        GC.SuppressFinalize(this);
    }
}


