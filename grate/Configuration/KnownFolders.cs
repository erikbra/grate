using System.Collections.Generic;
using System.IO;
using grate.Migration;
using static grate.Configuration.MigrationType;

namespace grate.Configuration;

public class KnownFolders: Dictionary<string, MigrationsFolder?>, IFoldersConfiguration
{
    public MigrationsFolder? AlterDatabase => this[nameof(KnownFolderNames.AlterDatabase)];
    public MigrationsFolder? RunAfterCreateDatabase => this[nameof(KnownFolderNames.RunAfterCreateDatabase)];
    public MigrationsFolder? RunBeforeUp => this[nameof(KnownFolderNames.RunBeforeUp)];
    public MigrationsFolder? Up => this[nameof(KnownFolderNames.Up)];
    public MigrationsFolder? RunFirstAfterUp => this[nameof(KnownFolderNames.RunFirstAfterUp)];
    public MigrationsFolder? Functions => this[nameof(KnownFolderNames.Functions)];
    public MigrationsFolder? Views => this[nameof(KnownFolderNames.Views)];
    public MigrationsFolder? Sprocs => this[nameof(KnownFolderNames.Sprocs)];
    public MigrationsFolder? Triggers => this[nameof(KnownFolderNames.Triggers)];
    public MigrationsFolder? Indexes => this[nameof(KnownFolderNames.Indexes)];
    public MigrationsFolder? RunAfterOtherAnyTimeScripts => this[nameof(KnownFolderNames.RunAfterOtherAnyTimeScripts)];
    public MigrationsFolder? Permissions => this[nameof(KnownFolderNames.Permissions)];
    public MigrationsFolder? BeforeMigration => this[nameof(KnownFolderNames.BeforeMigration)];
    public MigrationsFolder? AfterMigration => this[nameof(KnownFolderNames.AfterMigration)];
        
    public static KnownFolders In(DirectoryInfo parent, IKnownFolderNames? folderNames = null)
    {
        folderNames ??= KnownFolderNames.Default;
        
        DirectoryInfo Wrap(string folderName)
        {
            var folder = Path.Combine(parent.FullName, folderName);
            return new DirectoryInfo(folder);
        }
        
        return new KnownFolders()
        {
            { nameof(KnownFolderNames.BeforeMigration), new MigrationsFolder("BeforeMigration", Wrap(folderNames.BeforeMigration), EveryTime, TransactionHandling: TransactionHandling.Autonomous) },
            { nameof(KnownFolderNames.AlterDatabase) , new MigrationsFolder("AlterDatabase", Wrap(folderNames.AlterDatabase), AnyTime, ConnectionType.Admin, TransactionHandling.Autonomous) },
            { nameof(KnownFolderNames.RunAfterCreateDatabase), new MigrationsFolder("Run After Create Database", Wrap(folderNames.RunAfterCreateDatabase), AnyTime) },
            { nameof(KnownFolderNames.RunBeforeUp),  new MigrationsFolder("Run Before Update", Wrap(folderNames.RunBeforeUp), AnyTime) },
            { nameof(KnownFolderNames.Up), new MigrationsFolder("Update", Wrap(folderNames.Up), Once) },
            { nameof(KnownFolderNames.RunFirstAfterUp), new MigrationsFolder("Run First After Update", Wrap(folderNames.RunFirstAfterUp), Once) },
            { nameof(KnownFolderNames.Functions), new MigrationsFolder("Functions", Wrap(folderNames.Functions), AnyTime) },
            { nameof(KnownFolderNames.Views), new MigrationsFolder("Views", Wrap(folderNames.Views), AnyTime) },
            { nameof(KnownFolderNames.Sprocs), new MigrationsFolder("Stored Procedures", Wrap(folderNames.Sprocs), AnyTime) },
            { nameof(KnownFolderNames.Triggers), new MigrationsFolder("Triggers", Wrap(folderNames.Triggers), AnyTime) },
            { nameof(KnownFolderNames.Indexes), new MigrationsFolder("Indexes", Wrap(folderNames.Indexes), AnyTime) },
            { nameof(KnownFolderNames.RunAfterOtherAnyTimeScripts), new MigrationsFolder("Run after Other Anytime Scripts", Wrap(folderNames.RunAfterOtherAnyTimeScripts), AnyTime) },
            { nameof(KnownFolderNames.Permissions), new MigrationsFolder("Permissions", Wrap(folderNames.Permissions), EveryTime, TransactionHandling: TransactionHandling.Autonomous) },
            { nameof(KnownFolderNames.AfterMigration), new MigrationsFolder("AfterMigration", Wrap(folderNames.AfterMigration), EveryTime, TransactionHandling: TransactionHandling.Autonomous) }
        }
        ;

    }
    

}
