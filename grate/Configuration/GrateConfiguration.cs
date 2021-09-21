using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using grate.Infrastructure;

namespace grate.Configuration
{
    /// <summary>
    /// This is the 'schema' that System.CommandLine parses into for the MigrateCommand.
    /// This means that this object's properties should align with the options and arguments in `MigrateCommand` by convention.
    /// </summary>
    public class GrateConfiguration
    {
        private readonly string? _adminConnectionString = null;

        //public KnownFolders KnownFolders { get; set; } = InCurrentDirectory();
        public KnownFolders? KnownFolders { get; set; }

        public DatabaseType DatabaseType { get; init; } // = DatabaseType.sqlserver;

        public DirectoryInfo SqlFilesDirectory { get; init; } = CurrentDirectory;

        public DirectoryInfo OutputPath { get; init; } = new(Path.Combine(CurrentDirectory.FullName, "output"));

        public string? ConnectionString { get; init; } = null;

        public string SchemaName { get; init; } = "grate";

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
            var pattern = new Regex("(.*;\\s*(?:Initial Catalog|Database)=)([^;]*)(.*)");
            var replaced = pattern.Replace(connectionString, "$1master$3");
            return replaced;
        }

        public static GrateConfiguration Default => new();
        public bool CreateDatabase { get; init; } = true;
        public bool AlterDatabase { get; init; } = false;
        public bool Transaction { get; init; } = false;

        /// <summary>
        /// The environment the current migration is targeting for env-specific scripts.
        /// </summary>
        public GrateEnvironment? Environment { get; init; }

        /// <summary>
        /// The database version we're migrating to on this run.
        /// </summary>
        public string Version { get; init; } = "0.0.0.1";

        public int CommandTimeout { get; init; }
        public int AdminCommandTimeout { get; init; }
        public bool Silent => NonInteractive;
        public bool NonInteractive { get; init; }

        /// <summary>
        /// This instructs grate to not perform token replacement {{somename}}. Defaults to false.
        /// </summary>
        public bool DisableTokenReplacement { get; init; }

        /// <summary>
        /// Whether to drop the database prior to migration or not.
        /// </summary>
        public bool Drop { get; init; }

        /// <summary>
        /// The set of user-provided "key=value" pairs for use in token replacement.
        /// </summary>
        public IEnumerable<string>? UserTokens { get; init; }

        private static DirectoryInfo CurrentDirectory => new(Directory.GetCurrentDirectory());
    }
}
