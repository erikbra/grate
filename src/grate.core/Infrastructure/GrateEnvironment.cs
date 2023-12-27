﻿using static System.StringComparison;

namespace grate.Infrastructure;

public record GrateEnvironment
{
    /// <summary>
    /// The name of the Environment
    /// </summary>
    public string Current { get; }
    private const string EnvironmentMarker = ".ENV.";

    public GrateEnvironment(string current) => Current = current;

    public bool ShouldRun(string path) => !IsEnvironmentFile(path) || IsForCurrentEnvironment(path);

    private bool IsForCurrentEnvironment(string path) =>
        FileName(path).StartsWith($"{Current}.", InvariantCultureIgnoreCase) ||
        FileName(path).Contains($".{Current}.", InvariantCultureIgnoreCase);

    public static bool IsEnvironmentFile(string fileName) => fileName.Contains(EnvironmentMarker, InvariantCultureIgnoreCase);
    private static string FileName(string path) => new FileInfo(path).Name;

}
