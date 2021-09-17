using System;
using System.Collections.Generic;
using grate.Configuration;

namespace grate.Infrastructure
{
    public class TokenProvider
    {
        private readonly GrateConfiguration _config;

        public TokenProvider(GrateConfiguration config)
        {
            _config = config;
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

                //["AfterMigrationFolderName"] = AfterMigrationFolderName.to_string(),
                //["AlterDatabaseFolderName"] = AlterDatabaseFolderName.to_string(),
                //["Baseline"] = Baseline.to_string(),
                //["BeforeMigrationFolderName"] = BeforeMigrationFolderName.to_string(),
                ["CommandTimeout"] = _config.CommandTimeout.ToString(),
                ["CommandTimeoutAdmin"] = _config.AdminCommandTimeout.ToString(),
                //["ConfigurationFile"] = ConfigurationFile.to_string(),
                ["ConnectionString"] = _config.ConnectionString,
                ["ConnectionStringAdmin"] = _config.AdminConnectionString,
                //["CreateDatabaseCustomScript"] = CreateDatabaseCustomScript.to_string(),
                //["DatabaseName"] = _config.DatabaseName, //TODO: Find out where we get this, it's the only token I use!
                ["DatabaseType"] = _config.DatabaseType.ToString(),
                //["Debug"] = Debug.to_string(),
                //["DisableOutput"] = DisableOutput.to_string(),
                ["DisableTokenReplacement"] = _config.DisableTokenReplacement.ToString(),
                //["DoNotAlterDatabase"] = DoNotAlterDatabase.to_string(),
                //["DoNotCreateDatabase"] = DoNotCreateDatabase.to_string(),
                //["DoNotStoreScriptsRunText"] = DoNotStoreScriptsRunText.to_string(),
                //["DownFolderName"] = DownFolderName.to_string(),
                //["Drop"] = _config.Drop.ToString(),
                //["DryRun"] = DryRun.to_string(),
                ["EnvironmentName"] = _config.Environment?.Current,
                ["EnvironmentNames"] = _config.Environment?.Current,  //string.Join(",", EnvironmentNames),
                //["FunctionsFolderName"] = FunctionsFolderName.to_string(),
                //["IndexesFolderName"] = IndexesFolderName.to_string(),
                //["Initialize"] = Initialize.to_string(),
                ["OutputPath"] = _config.OutputPath.FullName, //TODO: Does RH use name or full path?
                //["PermissionsFolderName"] = PermissionsFolderName.to_string(),
                //["RecoveryMode"] = RecoveryMode.to_string(),
                //["RepositoryPath"] = RepositoryPath.to_string(),
                //["Restore"] = Restore.to_string(),
                //["RestoreCustomOptions"] = RestoreCustomOptions.to_string(),
                //["RestoreFromPath"] = RestoreFromPath.to_string(),
                //["RestoreTimeout"] = RestoreTimeout.to_string(),
                //["RunAfterCreateDatabaseFolderName"] = RunAfterCreateDatabaseFolderName.to_string(),
                //["RunAfterOtherAnyTimeScriptsFolderName"] = RunAfterOtherAnyTimeScriptsFolderName.to_string(),
                //["RunAllAnyTimeScripts"] = RunAllAnyTimeScripts.to_string(),
                //["RunBeforeUpFolderName"] = RunBeforeUpFolderName.to_string(),
                //["RunFirstAfterUpFolderName"] = RunFirstAfterUpFolderName.to_string(),
                ["SchemaName"] = _config.SchemaName,
                //["ScriptsRunErrorsTableName"] = ScriptsRunErrorsTableName.to_string(),
                //["ScriptsRunTableName"] = _config.ScriptsRunTableName.to_string(),
                //["SearchAllSubdirectoriesInsteadOfTraverse"] = SearchAllSubdirectoriesInsteadOfTraverse.to_string(),
                //["ServerName"] = ServerName.to_string(),
                ["Silent"] = _config.Silent.ToString(),
                //["SprocsFolderName"] = SprocsFolderName.to_string(),
                ["SqlFilesDirectory"] = _config.SqlFilesDirectory.FullName, //TODO: Does RH do full path or just dir name?
                //["TriggersFolderName"] = TriggersFolderName.to_string(),
                //["UpFolderName"] = UpFolderName.to_string(),
                ["Version"] = _config.Version,
                //["VersionFile"] = VersionFile.to_string(),
                //["VersionTableName"] = VersionTableName.to_string(),
                //["VersionXPath"] = VersionXPath.to_string(),
                //["ViewsFolderName"] = ViewsFolderName.to_string(),
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
