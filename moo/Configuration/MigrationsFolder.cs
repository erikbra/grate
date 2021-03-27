using System.IO;

namespace moo.Configuration
{
    public record MigrationsFolder(string Name, DirectoryInfo? Path, MigrationType Type) : Folder(Name, Path);
}