using grate.Migration;
using Microsoft.Extensions.Logging;

namespace Basic_tests.Infrastructure;

/// <summary>
/// Cannot Substitute ILogger&lt;GrateMigrator&gt;, since GrateMigrator is internal, so we need to create a mock logger.
/// </summary>
public record MockGrateLogger: ILogger<GrateMigrator>
{
    public virtual void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        var message = formatter(state, exception);
        LoggedMessages.Add(message);
    }

    public bool IsEnabled(LogLevel logLevel)
    {
        throw new NotImplementedException();
    }

    public IDisposable? BeginScope<TState>(TState state) where TState : notnull
    {
        throw new NotImplementedException();
    }
    
    public IList<string> LoggedMessages { get; } = new List<string>();
    
}
