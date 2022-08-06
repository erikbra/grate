using System.Collections.Generic;
using System.IO;

namespace grate.Configuration;

public interface IFoldersConfiguration: IReadOnlyDictionary<string, MigrationsFolder?>
{
    DirectoryInfo? Root { get; }
    void SetRoot(DirectoryInfo root);
    
    
}
