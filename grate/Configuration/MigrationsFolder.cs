using System.IO;

namespace grate.Configuration;

public record MigrationsFolder(string Name, DirectoryInfo Path, MigrationType Type) : Folder(Name, Path);
