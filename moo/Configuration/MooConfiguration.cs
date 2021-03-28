using System.IO;

namespace moo.Configuration
{
    public class MooConfiguration
    {
        public string? Database { get; init; }
        public string? Server { get; init; }
        public KnownFolders? KnownFolders { get; set; }
        public DatabaseType DatabaseType { get; init; }
        public DirectoryInfo? SqlFilesDirectory { get; init; }
        public DirectoryInfo? OutputPath { get; set; }
        public string? ConnectionString { get; set; }

        public static MooConfiguration Default => new();
        public bool CreateDatabase { get; init; }
        public bool RunInTransaction { get; init; }
    }
}