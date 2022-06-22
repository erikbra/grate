using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using grate.Migration;
using static grate.Configuration.MigrationType;

namespace grate.Configuration;

public class KnownFolders: Collection<MigrationsFolder>, IFoldersConfiguration
{
    public MigrationsFolder? AlterDatabase { get; init; }
    public MigrationsFolder? RunAfterCreateDatabase { get; init; }
    public MigrationsFolder? RunBeforeUp { get; init; }
    public MigrationsFolder? Up { get; init; }
    //public MigrationsFolder? Down { get; init; }
    public MigrationsFolder? RunFirstAfterUp { get; init; }
    public MigrationsFolder? Functions { get; init; }
    public MigrationsFolder? Views { get; init; }
    public MigrationsFolder? Sprocs { get; init; }
    public MigrationsFolder? Triggers { get; init; }
    public MigrationsFolder? Indexes { get; init; }
    public MigrationsFolder? RunAfterOtherAnyTimeScripts { get; init; }
    public MigrationsFolder? Permissions { get; init; }
    public MigrationsFolder? BeforeMigration { get; init; }
    public MigrationsFolder? AfterMigration { get; init; }
        
    public static KnownFolders In(DirectoryInfo parent)
    {
        DirectoryInfo Wrap(string folderName)
        {
            var folder = Path.Combine(parent.FullName, folderName);
            return new DirectoryInfo(folder);
        }

        return new KnownFolders(
            beforeMigration: new MigrationsFolder("BeforeMigration", Wrap("beforeMigration"), EveryTime),
            alterDatabase: new MigrationsFolder("AlterDatabase", Wrap("alterDatabase"), AnyTime, ConnectionType.Admin),
            runAfterCreateDatabase: new MigrationsFolder("Run After Create Database", Wrap("runAfterCreateDatabase"),
                AnyTime),
            runBeforeUp: new MigrationsFolder("Run Before Update", Wrap("runBeforeUp"), AnyTime),
            up: new MigrationsFolder("Update", Wrap("up"), Once),
            //down: new MigrationsFolder("Down Folder - Nothing to see here. Move along.", Wrap("down"), Once),
            runFirstAfterUp: new MigrationsFolder("Run First After Update", Wrap("runFirstAfterUp"), Once),
            functions: new MigrationsFolder("Functions", Wrap("functions"), AnyTime),
            views: new MigrationsFolder("Views", Wrap("views"), AnyTime),
            sprocs: new MigrationsFolder("Stored Procedures", Wrap("sprocs"), AnyTime),
            triggers: new MigrationsFolder("Triggers", Wrap("triggers"), AnyTime),
            indexes: new MigrationsFolder("Indexes", Wrap("indexes"), AnyTime),
            runAfterOtherAnyTimeScripts: new MigrationsFolder("Run after Other Anytime Scripts",
                Wrap("runAfterOtherAnyTimeScripts"), AnyTime),
            permissions: new MigrationsFolder("Permissions", Wrap("permissions"), EveryTime),
            afterMigration: new MigrationsFolder("AfterMigration", Wrap("afterMigration"), EveryTime)
        );
    }

    private KnownFolders(
        MigrationsFolder beforeMigration,
        MigrationsFolder alterDatabase,
        MigrationsFolder runAfterCreateDatabase,
        MigrationsFolder runBeforeUp,
        MigrationsFolder up,
        //MigrationsFolder down,
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
        : base(new List<MigrationsFolder>()
        {
            beforeMigration,
            alterDatabase,
            runAfterCreateDatabase,
            runBeforeUp,
            up,
            //down,
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
        //Down = down;
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
