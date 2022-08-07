using System.Collections.Generic;
using System.Linq;

namespace grate.Configuration;

public class CustomFoldersConfiguration: Dictionary<string, MigrationsFolder?>, IFoldersConfiguration
{
    public CustomFoldersConfiguration(IEnumerable<MigrationsFolder> folders) :
        base(folders.ToDictionary(folder => folder.Name, folder => (MigrationsFolder?) folder))
    {
    }

    public CustomFoldersConfiguration(params MigrationsFolder[] folders) :
        this(folders.AsEnumerable())
    { }
    
    public CustomFoldersConfiguration(IDictionary<string, MigrationsFolder> source) 
        : base(source.ToDictionary(item => item.Key, item => (MigrationsFolder?) item.Value))
    { }

    
    public CustomFoldersConfiguration() 
    { }


    public static CustomFoldersConfiguration Empty => new();

}
