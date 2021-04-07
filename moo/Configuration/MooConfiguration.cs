using System.IO;
using System.Text.RegularExpressions;

namespace moo.Configuration
{
    public class MooConfiguration
    {
        private readonly string? _adminConnectionString = null;

        //public KnownFolders KnownFolders { get; set; } = InCurrentDirectory();
        public KnownFolders? KnownFolders { get; set; }
        
        public DatabaseType DatabaseType { get; init; } = DatabaseType.sqlserver;
        //public DatabaseType DatabaseType { get; init; }
        
        public DirectoryInfo SqlFilesDirectory { get; init; } = CurrentDirectory;
        //public DirectoryInfo? SqlFilesDirectory { get; init; }
        
        public DirectoryInfo OutputPath { get; init; } = new(Path.Combine(CurrentDirectory.FullName, "output"));
        //public DirectoryInfo? OutputPath { get; set; }
        
        public string? ConnectionString { get; init; } = null;

        public string? AdminConnectionString
        {
            get => _adminConnectionString ?? WithAdminDb(ConnectionString);
            init => _adminConnectionString = value;
        }

        private static string? WithAdminDb(string? connectionString)
        {
            if (string.IsNullOrEmpty(connectionString))
            {
                return connectionString;
            }
            var pattern = new Regex("(.*;Initial Catalog=)([^;]*)(.*)");
            var replaced = pattern.Replace(connectionString, "$1master$3");
            return replaced;
        }

        public static MooConfiguration Default => new();
        public bool CreateDatabase { get; init; } = true;
        public bool AlterDatabase { get; init; } = false;
        public bool RunInTransaction { get; init; } = true;
        public string Version { get; init; } = "0.0.0.1";
        
        public int CommandTimeout { get; set; }
        public int AdminCommandTimeout { get; set; }
        
        //private static KnownFolders InCurrentDirectory() => KnownFolders.In(CurrentDirectory);
        private static DirectoryInfo CurrentDirectory => new(Directory.GetCurrentDirectory());
    }
}