using grate.Infrastructure.FileSystem;
using Microsoft.Extensions.Logging;
using static System.StringComparison;
using static System.StringSplitOptions;

namespace TestCommon.TestInfrastructure;

public static class TestConfig
{
    private static readonly Random Random = Random.Shared;

    public static string RandomDatabase() => Random.GetString(15);

    public static IDirectoryInfo CreateRandomTempDirectory()
    {
        var dummyFile = Path.GetTempFileName();
        File.Delete(dummyFile);

        if (Directory.Exists(dummyFile))
        {
            Directory.Delete(dummyFile, true);
        }

        var scriptsDir = Directory.CreateDirectory(dummyFile);
        return new PhysicalDirectoryInfo(scriptsDir);
    }
    
    public static IDirectoryInfo Wrap(IDirectoryInfo root, string? subFolder) =>
        new PhysicalDirectoryInfo(Path.Combine(root.ToString(), subFolder ?? ""));
    public static string? Username(string connectionString) => connectionString.Split(";", TrimEntries | RemoveEmptyEntries)
        .SingleOrDefault(entry => entry.StartsWith("Uid", OrdinalIgnoreCase) || entry.StartsWith("User Id", OrdinalIgnoreCase))?
        .Split("=", TrimEntries | RemoveEmptyEntries).Last();

    public static string? Password(string connectionString) => connectionString.Split(";", TrimEntries | RemoveEmptyEntries)
        .SingleOrDefault(entry => entry.StartsWith("Password", OrdinalIgnoreCase) || entry.StartsWith("Pwd", OrdinalIgnoreCase))?
        .Split("=", TrimEntries | RemoveEmptyEntries).Last();

    public static LogLevel GetLogLevel() => LogLevelFromEnvironmentVariable();
    
    
    private static LogLevel LogLevelFromEnvironmentVariable()
    {
        if (!Enum.TryParse(Environment.GetEnvironmentVariable("LogLevel"), out LogLevel logLevel))
        {
            logLevel = LogLevel.Trace;
        }
        return logLevel;
    }
    
  

    public static IDirectoryInfo MakeSurePathExists(IDirectoryInfo? path)
    {
        ArgumentNullException.ThrowIfNull(path);

        if (!path.Exists)
        {
            path.Create();
        }

        return path;
    }

}
