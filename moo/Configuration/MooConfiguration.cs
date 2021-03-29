using System.IO;

namespace moo.Configuration
{
    public class MooConfiguration
    {
        public string? Database { get; init; } = null;
        public string? Server { get; init; } = null;
        
        //public KnownFolders KnownFolders { get; set; } = InCurrentDirectory();
        public KnownFolders? KnownFolders { get; set; }
        
        public DatabaseType DatabaseType { get; init; } = DatabaseType.sqlserver;
        //public DatabaseType DatabaseType { get; init; }
        
        public DirectoryInfo SqlFilesDirectory { get; init; } = CurrentDirectory;
        //public DirectoryInfo? SqlFilesDirectory { get; init; }
        
        public DirectoryInfo OutputPath { get; init; } = new(Path.Combine(CurrentDirectory.FullName, "output"));
        //public DirectoryInfo? OutputPath { get; set; }
        
        public string? ConnectionString { get; set; } = null;

        public static MooConfiguration Default => new();
        public bool CreateDatabase { get; init; } = true;
        public bool AlterDatabase { get; init; } = false;
        public bool RunInTransaction { get; init; } = true;
        public string Version { get; init; } = "0.0.0.1";
        
        //private static KnownFolders InCurrentDirectory() => KnownFolders.In(CurrentDirectory);
        private static DirectoryInfo CurrentDirectory => new(Directory.GetCurrentDirectory());
    }
}