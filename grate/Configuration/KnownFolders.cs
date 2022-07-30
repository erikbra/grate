using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using grate.Migration;
using static grate.Configuration.MigrationType;

namespace grate.Configuration;

public class KnownFolders: Collection<MigrationsFolder?>, IFoldersConfiguration
{
    public MigrationsFolder? AlterDatabase { get; }
    public MigrationsFolder? RunAfterCreateDatabase { get; }
    public MigrationsFolder? RunBeforeUp { get; }
    public MigrationsFolder? Up { get; }
    public MigrationsFolder? RunFirstAfterUp { get; }
    public MigrationsFolder? Functions { get; }
    public MigrationsFolder? Views { get; }
    public MigrationsFolder? Sprocs { get; }
    public MigrationsFolder? Triggers { get; }
    public MigrationsFolder? Indexes { get; }
    public MigrationsFolder? RunAfterOtherAnyTimeScripts { get; }
    public MigrationsFolder? Permissions { get; }
    public MigrationsFolder? BeforeMigration { get; }
    public MigrationsFolder? AfterMigration { get; }
        
    public static KnownFolders In(DirectoryInfo parent, IKnownFolderNames? folderNames = null)
    {
        folderNames ??= KnownFolderNames.Default;
        
        DirectoryInfo Wrap(string folderName)
        {
            var folder = Path.Combine(parent.FullName, folderName);
            return new DirectoryInfo(folder);
        }

        return new KnownFolders(
            beforeMigration: new MigrationsFolder("BeforeMigration", Wrap(folderNames.BeforeMigration), EveryTime, TransactionHandling: TransactionHandling.Autonomous),
            alterDatabase: new MigrationsFolder("AlterDatabase", Wrap(folderNames.AlterDatabase), AnyTime, ConnectionType.Admin, TransactionHandling.Autonomous),
            runAfterCreateDatabase: new MigrationsFolder("Run After Create Database", Wrap(folderNames.RunAfterCreateDatabase),
                AnyTime),
            runBeforeUp: new MigrationsFolder("Run Before Update", Wrap(folderNames.RunBeforeUp), AnyTime),
            up: new MigrationsFolder("Update", Wrap(folderNames.Up), Once),
            runFirstAfterUp: new MigrationsFolder("Run First After Update", Wrap(folderNames.RunFirstAfterUp), Once),
            functions: new MigrationsFolder("Functions", Wrap(folderNames.Functions), AnyTime),
            views: new MigrationsFolder("Views", Wrap(folderNames.Views), AnyTime),
            sprocs: new MigrationsFolder("Stored Procedures", Wrap(folderNames.Sprocs), AnyTime),
            triggers: new MigrationsFolder("Triggers", Wrap(folderNames.Triggers), AnyTime),
            indexes: new MigrationsFolder("Indexes", Wrap(folderNames.Indexes), AnyTime),
            runAfterOtherAnyTimeScripts: new MigrationsFolder("Run after Other Anytime Scripts",
                Wrap(folderNames.RunAfterOtherAnyTimeScripts), AnyTime),
            permissions: new MigrationsFolder("Permissions", Wrap(folderNames.Permissions), EveryTime, TransactionHandling: TransactionHandling.Autonomous),
            afterMigration: new MigrationsFolder("AfterMigration", Wrap(folderNames.AfterMigration), EveryTime, TransactionHandling: TransactionHandling.Autonomous)
        );
    }

    private KnownFolders(
        MigrationsFolder beforeMigration,
        MigrationsFolder alterDatabase,
        MigrationsFolder runAfterCreateDatabase,
        MigrationsFolder runBeforeUp,
        MigrationsFolder up,
        MigrationsFolder runFirstAfterUp,
        MigrationsFolder functions,
        MigrationsFolder views,
        MigrationsFolder sprocs,
        MigrationsFolder triggers,
        MigrationsFolder indexes,
        MigrationsFolder runAfterOtherAnyTimeScripts,
        MigrationsFolder permissions,
        MigrationsFolder afterMigration
    )
        : base(new List<MigrationsFolder?>()
        {
            beforeMigration,
            alterDatabase,
            runAfterCreateDatabase,
            runBeforeUp,
            up,
            runFirstAfterUp,
            functions,
            views,
            sprocs,
            triggers,
            indexes,
            runAfterOtherAnyTimeScripts,
            permissions,
            afterMigration
        })
    {
        AlterDatabase = alterDatabase;
        RunAfterCreateDatabase = runAfterCreateDatabase;
        RunBeforeUp = runBeforeUp;
        Up = up;
        RunFirstAfterUp = runFirstAfterUp;
        Functions = functions;
        Views = views;
        Sprocs = sprocs;
        Triggers = triggers;
        Indexes = indexes;
        RunAfterOtherAnyTimeScripts = runAfterOtherAnyTimeScripts;
        Permissions = permissions;
        BeforeMigration = beforeMigration;
        AfterMigration = afterMigration;
    }
}
