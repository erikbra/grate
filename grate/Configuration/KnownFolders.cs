using System.Collections.Generic;
using grate.Migration;
using static grate.Configuration.MigrationType;

namespace grate.Configuration;

public class KnownFolders: Dictionary<string, MigrationsFolder?>, IFoldersConfiguration
{
    public MigrationsFolder? AlterDatabase => this[KnownFolderKeys.AlterDatabase];
    public MigrationsFolder? RunAfterCreateDatabase => this[KnownFolderKeys.RunAfterCreateDatabase];
    public MigrationsFolder? RunBeforeUp => this[KnownFolderKeys.RunBeforeUp];
    public MigrationsFolder? Up => this[KnownFolderKeys.Up];
    public MigrationsFolder? RunFirstAfterUp => this[KnownFolderKeys.RunFirstAfterUp];
    public MigrationsFolder? Functions => this[KnownFolderKeys.Functions];
    public MigrationsFolder? Views => this[KnownFolderKeys.Views];
    public MigrationsFolder? Sprocs => this[KnownFolderKeys.Sprocs];
    public MigrationsFolder? Triggers => this[KnownFolderKeys.Triggers];
    public MigrationsFolder? Indexes => this[KnownFolderKeys.Indexes];
    public MigrationsFolder? RunAfterOtherAnyTimeScripts => this[KnownFolderKeys.RunAfterOtherAnyTimeScripts];
    public MigrationsFolder? Permissions => this[KnownFolderKeys.Permissions];
    public MigrationsFolder? BeforeMigration => this[KnownFolderKeys.BeforeMigration];
    public MigrationsFolder? AfterMigration => this[KnownFolderKeys.AfterMigration];
        
    public static KnownFolders In(IKnownFolderNames? folderNames = null)
    {
        folderNames ??= KnownFolderNames.Default;
        
        return new KnownFolders()
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
            { KnownFolderKeys.AfterMigration, new MigrationsFolder("AfterMigration", folderNames.AfterMigration, EveryTime, TransactionHandling: TransactionHandling.Autonomous) }
        };
    }

    public static KnownFolders UnRooted(IKnownFolderNames? folderNames = null) => In(folderNames);


    private KnownFolders()
    {
    }
  
}
