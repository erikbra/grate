using static System.StringComparison;

namespace grate.Infrastructure;

public record GrateEnvironment(string Current)
{
    /// <summary>
    /// The name of the Environment
    /// </summary>
    public string Current { get; } = Current;

    private const string EnvironmentMarker = ".ENV.";

    public bool ShouldRun(string path) => !IsEnvironmentFile(path) || IsForCurrentEnvironment(path);

    private bool IsForCurrentEnvironment(string path) =>
        FileName(path).StartsWith($"{Current}.", InvariantCultureIgnoreCase) ||
        FileName(path).Contains($".{Current}.", InvariantCultureIgnoreCase);

    public static bool IsEnvironmentFile(string fileName) => fileName.Contains(EnvironmentMarker, InvariantCultureIgnoreCase);
    private static string FileName(string path) => new FileInfo(path).Name;

    public override string ToString() => Current;
    
    public static GrateEnvironment Internal { get; } = new("GrateInternal-a01ce6e6-0038-4ebe-959e-7d039f6435bf");
}
