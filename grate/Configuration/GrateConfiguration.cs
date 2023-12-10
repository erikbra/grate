using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using grate.Infrastructure;
using Microsoft.Extensions.Logging;

namespace grate.Configuration;

/// <summary>
/// This is the 'schema' that System.CommandLine parses into for the MigrateCommand.
/// This means that this object's properties should align with the options and arguments in `MigrateCommand` by convention.
/// </summary>
public record GrateConfiguration
{
    private readonly string? _adminConnectionString;

    public IFoldersConfiguration? Folders { get; init; } = FoldersConfiguration.Default();

    public DatabaseType DatabaseType { get; init; } // = DatabaseType.sqlserver;

    public DirectoryInfo SqlFilesDirectory { get; init; } = CurrentDirectory;

    public DirectoryInfo OutputPath { get; init; } = new(Path.Combine(CurrentDirectory.FullName, "output"));

    public string? ConnectionString { get; init; }

    public string SchemaName { get; init; } = "grate";

    public string ScriptsRunTableName { get; set; } = "ScriptsRun";
    public string ScriptsRunErrorsTableName { get; set; } = "ScriptsRunErrors";
    public string VersionTableName { get; set; } = "Version";

    public string? AdminConnectionString
    {
        get => _adminConnectionString ?? WithAdminDb(ConnectionString);
        init => _adminConnectionString = value;
    }

    public string? AccessToken { get; set; }

    private string? WithAdminDb(string? connectionString)
    {
        if (string.IsNullOrEmpty(connectionString))
        {
            return connectionString;
        }
        var pattern = new Regex("(.*;\\s*(?:Initial Catalog|Database)=)([^;]*)(.*)");
        var replacement = $"$1{GetMasterDbName(DatabaseType)}$3";
        var replaced = pattern.Replace(connectionString, replacement);
        return replaced;
    }

    public static GrateConfiguration Default => new();
    public bool CreateDatabase { get; init; } = true;
    public bool AlterDatabase { get; init; }
    public bool Transaction { get; init; }

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
    /// If true grate will issue a warning and RUN any one time scripts that have changed.
    /// </summary>
    public bool WarnOnOneTimeScriptChanges { get; init; }

    /// <summary>
    /// If true grate will issue a warning, update the dbase hash but NOT RUN any one time scripts that have changed.
    /// </summary>
    public bool WarnAndIgnoreOnOneTimeScriptChanges { get; init; }

    /// <summary>
    /// The set of user-provided "key=value" pairs for use in token replacement.
    /// </summary>
    public IEnumerable<string>? UserTokens { get; init; }

    private static DirectoryInfo CurrentDirectory => new(Directory.GetCurrentDirectory());

    public LogLevel Verbosity { get; init; } = LogLevel.Information;

    /// <summary>
    /// If true grate will not store script text in the database to save space in small/embedded databases.
    /// </summary>
    public bool DoNotStoreScriptsRunText { get; init; }

    /// <summary>
    /// If true we mark scripts as run but don't actually run them.
    /// </summary>
    public bool Baseline { get; init; }

    /// <summary>
    /// If true we need to log what we would have done, but NOT run any SQL
    /// including writing to the grate versioning schema
    /// </summary>
    public bool DryRun { get; init; }

    /// <summary>
    /// If true runs all AnyTime scripts even if they haven't changed.
    /// </summary>
    public bool RunAllAnyTimeScripts { get; init; }

    /// <summary>
    /// If specified, location of the backup file to use when restoring
    /// </summary>
    public string? Restore { get; init; }

    /// <summary>
    /// By default, scripts are ordered by relative path including subdirectories. This option searches subdirectories, but order is based on filename alone.
    /// </summary>
    public bool IgnoreDirectoryNames { get; set; }

    private static string GetMasterDbName(DatabaseType databaseType) => databaseType switch
    {
        DatabaseType.mariadb => "mysql",
        DatabaseType.oracle => "oracle",
        DatabaseType.postgresql => "postgres",
        DatabaseType.sqlite => "master",
        DatabaseType.sqlserver => "master",
        _ => throw new ArgumentOutOfRangeException(nameof(databaseType), databaseType.ToString())
    };

}
