using System;
using System.Collections.Generic;
using grate.Configuration;
using grate.Migration;
using static System.StringSplitOptions;

namespace grate.Infrastructure;

public class TokenProvider
{
    private readonly GrateConfiguration _config;
    private readonly IDatabase _db;

    public TokenProvider(GrateConfiguration config, IDatabase db)
    {
        _config = config;
        _db = db;
    }

    /// <summary>
    /// Gets a dictionary of tokens for replacement.  Keys == {tokens} and values are their replacement value.
    /// </summary>
    /// <returns></returns>
    public Dictionary<string, string?> GetTokens()
    {
        var tokens = new Dictionary<string, string?>(StringComparer.OrdinalIgnoreCase)
        {
            // we're missing many of the original roundhouse tokens atm, leaving
            // them commented out as a reminder for future us!

            // I'm not sure some of them make sense (eg Debug), but if they're avail ¯\_(ツ)_/¯

            ["AfterMigrationFolderName"] = _config.KnownFolders?[KnownFolderKeys.AfterMigration].ToToken(),
            ["AlterDatabaseFolderName"] = _config.KnownFolders?[KnownFolderKeys.AlterDatabase].ToToken(),
            ["Baseline"] = _config.Baseline.ToString(),
            ["BeforeMigrationFolderName"] = _config.KnownFolders?[KnownFolderKeys.BeforeMigration].ToToken(),
            ["CommandTimeout"] = _config.CommandTimeout.ToString(),
            ["CommandTimeoutAdmin"] = _config.AdminCommandTimeout.ToString(),
            //["ConfigurationFile"] = ConfigurationFile.to_string(),
            ["ConnectionString"] = _config.ConnectionString,
            ["ConnectionStringAdmin"] = _config.AdminConnectionString,
            //["CreateDatabaseCustomScript"] = CreateDatabaseCustomScript.to_string(),
            ["DatabaseName"] = _db.DatabaseName,
            ["DatabaseType"] = _config.DatabaseType.ToString(),
            //["Debug"] = Debug.to_string(),
            //["DisableOutput"] = DisableOutput.to_string(),
            ["DisableTokenReplacement"] = _config.DisableTokenReplacement.ToString(),
            ["DoNotAlterDatabase"] = (!_config.AlterDatabase).ToString(),
            ["DoNotCreateDatabase"] = (!_config.CreateDatabase).ToString(),
            ["DoNotStoreScriptsRunText"] = _config.DoNotStoreScriptsRunText.ToString(),
            //["DownFolderName"] = _config.KnownFolders?.Down.ToToken(),
            ["Drop"] = _config.Drop.ToString(),
            ["DryRun"] = _config.DryRun.ToString(),
            ["EnvironmentName"] = _config.Environment?.Current,
            ["EnvironmentNames"] = _config.Environment?.Current,  //string.Join(",", EnvironmentNames),
            ["FunctionsFolderName"] = _config.KnownFolders?[KnownFolderKeys.Functions].ToToken(),
            ["IndexesFolderName"] = _config.KnownFolders?[KnownFolderKeys.Indexes].ToToken(),
            //["Initialize"] = Initialize.to_string(),
            ["OutputPath"] = _config.OutputPath.FullName, //TODO: Does RH use name or full path?
            ["PermissionsFolderName"] = _config.KnownFolders?[KnownFolderKeys.Permissions].ToToken(),
            //["RecoveryMode"] = RecoveryMode.to_string(),
            //["RepositoryPath"] = RepositoryPath.to_string(),
            ["Restore"] = _config.Restore,
            //["RestoreTimeout"] = RestoreTimeout.to_string(),
            ["RunAfterCreateDatabaseFolderName"] = _config.KnownFolders?[KnownFolderKeys.RunAfterCreateDatabase].ToToken(),
            ["RunAfterOtherAnyTimeScriptsFolderName"] = _config.KnownFolders?[KnownFolderKeys.RunAfterOtherAnyTimeScripts].ToToken(),
            ["RunAllAnyTimeScripts"] = _config.RunAllAnyTimeScripts.ToString(),
            ["RunBeforeUpFolderName"] = _config.KnownFolders?[KnownFolderKeys.RunBeforeUp].ToToken(),
            ["RunFirstAfterUpFolderName"] = _config.KnownFolders?[KnownFolderKeys.RunFirstAfterUp].ToToken(),
            ["SchemaName"] = _config.SchemaName,
            ["ScriptsRunErrorsTableName"] = _db.ScriptsRunErrorsTable,
            ["ScriptsRunTableName"] = _db.ScriptsRunTable,
            //["SearchAllSubdirectoriesInsteadOfTraverse"] = SearchAllSubdirectoriesInsteadOfTraverse.to_string(),
            ["ServerName"] = _db.ServerName,
            ["Silent"] = _config.Silent.ToString(),
            ["SprocsFolderName"] = _config.KnownFolders?[KnownFolderKeys.Sprocs].ToToken(),
            ["SqlFilesDirectory"] = _config.SqlFilesDirectory.FullName, //TODO: Does RH do full path or just dir name?
            ["TriggersFolderName"] = _config.KnownFolders?[KnownFolderKeys.Triggers].ToToken(),
            ["UpFolderName"] = _config.KnownFolders?[KnownFolderKeys.Up].ToToken(),
            ["Version"] = _config.Version,
            //["VersionFile"] = VersionFile.to_string(),
            ["VersionTableName"] = _db.VersionTable,
            //["VersionXPath"] = VersionXPath.to_string(),
            ["ViewsFolderName"] = _config.KnownFolders?[KnownFolderKeys.Views].ToToken(),
            ["WarnAndIgnoreOnOneTimeScriptChanges"] = _config.WarnAndIgnoreOnOneTimeScriptChanges.ToString(),
            ["WarnOnOneTimeScriptChanges"] = _config.WarnOnOneTimeScriptChanges.ToString(),
            ["WithTransaction"] = _config.Transaction.ToString()
        };

        foreach (var s in _config.UserTokens.Safe()) // s is the original "key=value" value from the user
        {
            (string key, string value) = ParseUserToken(s);
            tokens[key] = value;
        }

        return tokens;
    }


    public static (string key, string value) ParseUserToken(string userToken)
    {
        // Splits the given "key=value" string into it's component parts.
        var parts = userToken.Split('=', RemoveEmptyEntries | TrimEntries);
        if (parts.Length != 2)
        {
            throw new ArgumentOutOfRangeException(nameof(userToken), $"Failed to parse provided user token '{userToken}'");
        }

        return (parts[0], parts[1]);
    }
}

