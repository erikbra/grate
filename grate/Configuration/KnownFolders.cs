using System.IO;
using grate.Migration;
using static grate.Configuration.MigrationType;

namespace grate.Configuration;

public class KnownFolders
{
    public MigrationsFolder? AlterDatabase { get; init; }
    public MigrationsFolder? RunAfterCreateDatabase { get; init; }
    public MigrationsFolder? RunBeforeUp { get; init; }
    public MigrationsFolder? Up { get; init; }
    public MigrationsFolder? Down { get; init; }
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

        return new KnownFolders()
        {
            AlterDatabase = new MigrationsFolder("AlterDatabase", Wrap("alterDatabase"), AnyTime, ConnectionType.Admin),
            RunAfterCreateDatabase = new MigrationsFolder("Run After Create Database", Wrap("runAfterCreateDatabase"), AnyTime),
            RunBeforeUp = new MigrationsFolder("Run Before Update", Wrap("runBeforeUp"), AnyTime),
            Up = new MigrationsFolder("Update", Wrap("up"), Once),
            Down = new MigrationsFolder("Down Folder - Nothing to see here. Move along.", Wrap("down"), Once),
            RunFirstAfterUp = new MigrationsFolder("Run First After Update", Wrap("runFirstAfterUp"), Once),
            Functions = new MigrationsFolder("Functions", Wrap("functions"), AnyTime),
            Views = new MigrationsFolder("Views", Wrap("views"), AnyTime),
            Sprocs = new MigrationsFolder("Stored Procedures", Wrap("sprocs"), AnyTime),
            Triggers = new MigrationsFolder("Triggers", Wrap("triggers"), AnyTime),
            Indexes = new MigrationsFolder("Indexes", Wrap("indexes"), AnyTime),
            RunAfterOtherAnyTimeScripts = new MigrationsFolder("Run after Other Anytime Scripts", Wrap("runAfterOtherAnyTimeScripts"), AnyTime),
            Permissions = new MigrationsFolder("Permissions", Wrap("permissions"), EveryTime),
            BeforeMigration = new MigrationsFolder("BeforeMigration", Wrap("beforeMigration"), EveryTime),
            AfterMigration = new MigrationsFolder("AfterMigration", Wrap("afterMigration"), EveryTime),
        };
    }
}
