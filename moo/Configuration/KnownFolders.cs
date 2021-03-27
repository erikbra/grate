using System.IO;
using System.Linq;
using static moo.Configuration.MigrationType;

namespace moo.Configuration
{
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

        public static KnownFolders In(DirectoryInfo? parent)
        {
            DirectoryInfo? Wrap(string folderName) => parent?.GetDirectories(folderName).FirstOrDefault();

            return new KnownFolders()
            {
                AlterDatabase = new MigrationsFolder("AlterDatabase", Wrap("alterDatabase"), EveryTime),
                RunAfterCreateDatabase =
                    new MigrationsFolder("Run After Create Database", Wrap("runAfterCreateDatabase"), EveryTime),
                RunBeforeUp = new MigrationsFolder("Run Before Update", Wrap("runBeforeUp"), EveryTime),
                Up = new MigrationsFolder("Update", Wrap("up"), Once),
                Down = new MigrationsFolder("Down Folder - Nothing to see here. Move along.", Wrap("down"), Once),
                RunFirstAfterUp = new MigrationsFolder("Run First After Update", Wrap("runFirstAfterUp"), Once),
                Functions = new MigrationsFolder("Functions", Wrap("functions"), EveryTime),
                Views = new MigrationsFolder("Views", Wrap("views"), EveryTime),
                Sprocs = new MigrationsFolder("Stored Procedures", Wrap("sprocs"), EveryTime),
                Triggers = new MigrationsFolder("Triggers", Wrap("triggers"), EveryTime),
                Indexes = new MigrationsFolder("Indexes", Wrap("indexes"), EveryTime),
                RunAfterOtherAnyTimeScripts = new MigrationsFolder("Run after Other Anytime Scripts",
                    Wrap("runAfterOtherAnyTimeScripts"), EveryTime),
                Permissions = new MigrationsFolder("Permissions", Wrap("permissions"), EveryTime),
                BeforeMigration = new MigrationsFolder("BeforeMigration", Wrap("beforeMigration"), EveryTime),
                AfterMigration = new MigrationsFolder("AfterMigration", Wrap("afterMigration"), EveryTime),
            };
        }
    }
}