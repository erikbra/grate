namespace grate.Migration;

// ReSharper disable once ClassNeverInstantiated.Global
internal record MigrationResult
{
    private readonly List<string> _scriptsRun = [];

    public IReadOnlyList<string> ScriptsRun => _scriptsRun.AsReadOnly();
    public bool IsUpToDate { get; set; } = true;

    public void AddScriptRun(string scriptName) => _scriptsRun.Add(scriptName);
}
