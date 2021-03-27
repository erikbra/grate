using System.IO;

namespace moo.Configuration
{
    public record Folder(string? Name, DirectoryInfo? Path);
}