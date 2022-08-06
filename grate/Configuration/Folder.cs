using System.IO;

namespace grate.Configuration;

public record Folder(string Name, DirectoryInfo? Path)
{
    public string? RelativePath { get; init; }
    public DirectoryInfo? Path { get; set; } = Path;

    public void SetRoot(DirectoryInfo value) => Path ??= new DirectoryInfo(System.IO.Path.Join(value.ToString(), RelativePath));

    public Folder(string name, string relativePath) : this(name, default(DirectoryInfo?))
    {
        RelativePath = relativePath;
    }

    public bool IsRooted() => Path != null;

}

