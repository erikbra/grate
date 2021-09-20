using System;
using System.Collections.Generic;
using grate.Configuration;
using grate.Migration;

namespace grate.Infrastructure
{
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

                ["AfterMigrationFolderName"] = _config.KnownFolders?.AfterMigration.ToToken(),
                ["AlterDatabaseFolderName"] = _config.KnownFolders?.AlterDatabase.ToToken(),
                //["Baseline"] = Baseline.to_string(),
                ["BeforeMigrationFolderName"] = _config.KnownFolders?.BeforeMigration.ToToken(),
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
                //["DoNotAlterDatabase"] = DoNotAlterDatabase.to_string(),
                //["DoNotCreateDatabase"] = DoNotCreateDatabase.to_string(),
                //["DoNotStoreScriptsRunText"] = DoNotStoreScriptsRunText.to_string(),
                ["DownFolderName"] = _config.KnownFolders?.Down.ToToken(),
                //["Drop"] = _config.Drop.ToString(),
                //["DryRun"] = DryRun.to_string(),
                ["EnvironmentName"] = _config.Environment?.Current,
                ["EnvironmentNames"] = _config.Environment?.Current,  //string.Join(",", EnvironmentNames),
                ["FunctionsFolderName"] = _config.KnownFolders?.Functions.ToToken(),
                ["IndexesFolderName"] = _config.KnownFolders?.Indexes.ToToken(),
                //["Initialize"] = Initialize.to_string(),
                ["OutputPath"] = _config.OutputPath.FullName, //TODO: Does RH use name or full path?
                ["PermissionsFolderName"] = _config.KnownFolders?.Permissions.ToToken(),
                //["RecoveryMode"] = RecoveryMode.to_string(),
                //["RepositoryPath"] = RepositoryPath.to_string(),
                //["Restore"] = Restore.to_string(),
                //["RestoreCustomOptions"] = RestoreCustomOptions.to_string(),
                //["RestoreFromPath"] = RestoreFromPath.to_string(),
                //["RestoreTimeout"] = RestoreTimeout.to_string(),
                ["RunAfterCreateDatabaseFolderName"] = _config.KnownFolders?.RunAfterCreateDatabase.ToToken(),
                ["RunAfterOtherAnyTimeScriptsFolderName"] = _config.KnownFolders?.RunAfterOtherAnyTimeScripts.ToToken(),
                //["RunAllAnyTimeScripts"] = RunAllAnyTimeScripts.to_string(),
                ["RunBeforeUpFolderName"] = _config.KnownFolders?.RunBeforeUp.ToToken(),
                ["RunFirstAfterUpFolderName"] = _config.KnownFolders?.RunFirstAfterUp.ToToken(),
                ["SchemaName"] = _config.SchemaName,
                //["ScriptsRunErrorsTableName"] = ScriptsRunErrorsTableName.to_string(),
                //["ScriptsRunTableName"] = _config.ScriptsRunTableName.to_string(),
                //["SearchAllSubdirectoriesInsteadOfTraverse"] = SearchAllSubdirectoriesInsteadOfTraverse.to_string(),
                ["ServerName"] = _db.ServerName,
                ["Silent"] = _config.Silent.ToString(),
                ["SprocsFolderName"] = _config.KnownFolders?.Sprocs.ToToken(),
                ["SqlFilesDirectory"] = _config.SqlFilesDirectory.FullName, //TODO: Does RH do full path or just dir name?
                ["TriggersFolderName"] = _config.KnownFolders?.Triggers.ToToken(),
                ["UpFolderName"] = _config.KnownFolders?.Up.ToToken(),
                ["Version"] = _config.Version,
                //["VersionFile"] = VersionFile.to_string(),
                //["VersionTableName"] = VersionTableName.to_string(),
                //["VersionXPath"] = VersionXPath.to_string(),
                ["ViewsFolderName"] = _config.KnownFolders?.Views.ToToken(),
                //["WarnAndIgnoreOnOneTimeScriptChanges"] = WarnAndIgnoreOnOneTimeScriptChanges.to_string(),
                //["WarnOnOneTimeScriptChanges"] = WarnOnOneTimeScriptChanges.to_string(),
                ["WithTransaction"] = _config.Transaction.ToString()
            };

            /*
            if (UserTokens != null)
            {
                foreach (var t in UserTokens)
                {
                    tokens[t.Key] = t.Value;
                }
            }
            */
            return tokens;
        }
    }
}
