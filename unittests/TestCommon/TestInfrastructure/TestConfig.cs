using Microsoft.Extensions.Logging;
using static System.StringComparison;
using static System.StringSplitOptions;

#if NET6_0
using Dir = TestCommon.TestInfrastructure.Net6PolyFills.Directory;
#else
using Dir = System.IO.Directory;
#endif

namespace TestCommon.TestInfrastructure;

public static class TestConfig
{
    private static readonly Random Random = Random.Shared;

    public static string RandomDatabase() => Random.GetString(16);

    public static DirectoryInfo CreateRandomTempDirectory()
    {
        return Dir.CreateTempSubdirectory();
    }
    public static DirectoryInfo Wrap(DirectoryInfo root, string? subFolder) =>
        new(Path.Combine(root.ToString(), subFolder ?? ""));
    public static string? Username(string connectionString) => connectionString.Split(";", TrimEntries | RemoveEmptyEntries)
        .SingleOrDefault(entry => 
            entry.StartsWith("Uid", OrdinalIgnoreCase) || 
            entry.StartsWith("User Id", OrdinalIgnoreCase) || 
            entry.StartsWith("Username", OrdinalIgnoreCase))
        ?
        .Split("=", TrimEntries | RemoveEmptyEntries).Last();

    public static string? Password(string connectionString) => connectionString.Split(";", TrimEntries | RemoveEmptyEntries)
        .SingleOrDefault(entry => 
            entry.StartsWith("Password", OrdinalIgnoreCase) || 
            entry.StartsWith("Pwd", OrdinalIgnoreCase))?
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


    public static void WriteContent(DirectoryInfo? path, string filename, string? content)
    {
        ArgumentNullException.ThrowIfNull(path);
        if (!path.Exists)
        {
            path.Create();
        }

        File.WriteAllText(Path.Combine(path.ToString(), filename), content);
    }

    public static DirectoryInfo MakeSurePathExists(DirectoryInfo? path)
    {
        ArgumentNullException.ThrowIfNull(path);

        if (!path.Exists)
        {
            path.Create();
        }

        return path;
    }

}
