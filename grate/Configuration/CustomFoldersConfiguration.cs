using System.Collections.Generic;
using System.Linq;

namespace grate.Configuration;

public class CustomFoldersConfiguration: Dictionary<string, MigrationsFolder?>, IFoldersConfiguration
{
    public CustomFoldersConfiguration(IEnumerable<MigrationsFolder> folders) :
        base(folders.ToDictionary(folder => folder.Name, folder => (MigrationsFolder?) folder)) {}

    public CustomFoldersConfiguration(params MigrationsFolder[] folders) :
        base(folders.ToDictionary(folder => folder.Name, folder => (MigrationsFolder?) folder)) {}
    
}
