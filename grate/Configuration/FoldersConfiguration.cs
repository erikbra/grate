using System.Collections.Generic;
using System.Linq;
using grate.Migration;
using static grate.Configuration.MigrationType;

namespace grate.Configuration;

public class FoldersConfiguration: Dictionary<string, MigrationsFolder?>, IFoldersConfiguration
{
    public FoldersConfiguration(IEnumerable<MigrationsFolder> folders) :
        base(folders.ToDictionary(folder => folder.Name, folder => (MigrationsFolder?) folder))
    {
    }

    public FoldersConfiguration(params MigrationsFolder[] folders) :
        this(folders.AsEnumerable())
    { }
    
    public FoldersConfiguration(IDictionary<string, MigrationsFolder> source) 
        : base(source.ToDictionary(item => item.Key, item => (MigrationsFolder?) item.Value))
    { }

    
    public FoldersConfiguration() 
    { }
    
    public MigrationsFolder? CreateDatabase { get; set; }
    public MigrationsFolder? DropDatabase { get; set; }

    public static FoldersConfiguration Empty => new();
    
    public static IFoldersConfiguration Default(IKnownFolderNames? folderNames = null)
    {
        folderNames ??= KnownFolderNames.Default;

        var foldersConfiguration = new FoldersConfiguration()
        {
            { KnownFolderKeys.BeforeMigration, new MigrationsFolder("BeforeMigration", folderNames.BeforeMigration, EveryTime, TransactionHandling: TransactionHandling.Autonomous) },
            { KnownFolderKeys.AlterDatabase , new MigrationsFolder("AlterDatabase", folderNames.AlterDatabase, AnyTime, ConnectionType.Admin, TransactionHandling.Autonomous) },
            { KnownFolderKeys.RunAfterCreateDatabase, new MigrationsFolder("Run After Create Database", folderNames.RunAfterCreateDatabase, AnyTime) },
            { KnownFolderKeys.RunBeforeUp,  new MigrationsFolder("Run Before Update", folderNames.RunBeforeUp, AnyTime) },
            { KnownFolderKeys.Up, new MigrationsFolder("Update", folderNames.Up, Once) },
            { KnownFolderKeys.RunFirstAfterUp, new MigrationsFolder("Run First After Update", folderNames.RunFirstAfterUp, Once) },
            { KnownFolderKeys.Functions, new MigrationsFolder("Functions", folderNames.Functions, AnyTime) },
            { KnownFolderKeys.Views, new MigrationsFolder("Views", folderNames.Views, AnyTime) },
            { KnownFolderKeys.Sprocs, new MigrationsFolder("Stored Procedures", folderNames.Sprocs, AnyTime) },
            { KnownFolderKeys.Triggers, new MigrationsFolder("Triggers", folderNames.Triggers, AnyTime) },
            { KnownFolderKeys.Indexes, new MigrationsFolder("Indexes", folderNames.Indexes, AnyTime) },
            { KnownFolderKeys.RunAfterOtherAnyTimeScripts, new MigrationsFolder("Run after Other Anytime Scripts", folderNames.RunAfterOtherAnyTimeScripts, AnyTime) },
            { KnownFolderKeys.Permissions, new MigrationsFolder("Permissions", folderNames.Permissions, EveryTime, TransactionHandling: TransactionHandling.Autonomous) },
            { KnownFolderKeys.AfterMigration, new MigrationsFolder("AfterMigration", folderNames.AfterMigration, EveryTime, TransactionHandling: TransactionHandling.Autonomous) },
        };
        foldersConfiguration.CreateDatabase = new MigrationsFolder("CreateDatabase", folderNames.CreateDatabase, AnyTime, ConnectionType.Admin, TransactionHandling.Autonomous);
        foldersConfiguration.DropDatabase = new MigrationsFolder("DropDatabase", folderNames.DropDatabase, AnyTime, ConnectionType.Admin, TransactionHandling.Autonomous);
        
        return foldersConfiguration;
    }

}
