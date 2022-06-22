using System.IO;
using grate.Migration;

namespace grate.Configuration;

public record MigrationsFolder(string Name, DirectoryInfo Path, MigrationType Type, ConnectionType ConnectionType = ConnectionType.Default) : Folder(Name, Path);
