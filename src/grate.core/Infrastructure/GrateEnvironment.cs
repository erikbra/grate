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
    
    // The names are so difficult to make sure there is a close to zero chance that we collide with any 
    // environment that an actual user might use (which would make things crash badly)
    public static GrateEnvironment Internal { get; } = new("GrateInternal-a01ce6e6-0038-4ebe-959e-7d039f6435bf");
    public static GrateEnvironment InternalBootstrap { get; } = new("GrateInternalBoostrap-a61456d0-e00a-4933-b692-c6a5d7d51539");
}
