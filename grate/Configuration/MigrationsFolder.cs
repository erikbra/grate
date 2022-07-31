using System.IO;
using grate.Migration;

namespace grate.Configuration;

public record MigrationsFolder(
        string Name,
        DirectoryInfo Path,
        MigrationType Type,
        ConnectionType ConnectionType = ConnectionType.Default,
        TransactionHandling TransactionHandling = TransactionHandling.Default)
    : Folder(Name, Path)
{

    public MigrationsFolder(
        DirectoryInfo root,
        string name,
        MigrationType type,
        ConnectionType connectionType = ConnectionType.Default,
        TransactionHandling transactionHandling = TransactionHandling.Default
    ) : this(name, Wrap(root, name), type, connectionType, transactionHandling) { }
    
    public MigrationsFolder(
        DirectoryInfo root,
        string name,
        string folderName,
        MigrationType type,
        ConnectionType connectionType = ConnectionType.Default,
        TransactionHandling transactionHandling = TransactionHandling.Default
    ) : this(name, Wrap(root, folderName), type, connectionType, transactionHandling) { }
    
    private static DirectoryInfo Wrap(DirectoryInfo root, string subFolder)
    {
        var folder = System.IO.Path.Combine(root.FullName, subFolder);
        return new DirectoryInfo(folder);
    }

}
