using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace grate.Configuration;

public class CustomFoldersConfiguration: Dictionary<string, MigrationsFolder?>, IFoldersConfiguration
{
    public CustomFoldersConfiguration(DirectoryInfo? root, IEnumerable<MigrationsFolder> folders) :
        base(folders.ToDictionary(folder => folder.Name, folder => (MigrationsFolder?) Wrap(root, folder) ))
    {
        Root = root;
    }

    public CustomFoldersConfiguration(DirectoryInfo? root, params MigrationsFolder[] folders) :
        this(root, folders.AsEnumerable())
    { }
    
    public CustomFoldersConfiguration(IDictionary<string, MigrationsFolder> source) 
        : base(source.ToDictionary(item => item.Key, item => (MigrationsFolder?) item.Value))
    { }

    public CustomFoldersConfiguration(DirectoryInfo? root, IDictionary<string, MigrationsFolder> source)
        : base(source.ToDictionary(item => item.Key, item => (MigrationsFolder?)item.Value))
    {
        Root = root;
    }
    
    public CustomFoldersConfiguration() 
    { }

    public DirectoryInfo? Root { get; private set; }
    
    public void SetRoot(DirectoryInfo root)
    {
        Root = root;
        foreach (var value in Values)
        {
            value?.SetRoot(root);
        }
    }

    public static CustomFoldersConfiguration Empty => new(new DirectoryInfo("/dev/null"));

    private static MigrationsFolder Wrap(DirectoryInfo? root, MigrationsFolder folder) => folder with { Path = Wrap(root, folder.Path) };
   
    private static DirectoryInfo Wrap(DirectoryInfo? root, DirectoryInfo subFolder)
    {
        var folder = root is not null ? Path.Combine(root.FullName, subFolder.FullName): subFolder.FullName;
        return new DirectoryInfo(folder);
    }
    
}
