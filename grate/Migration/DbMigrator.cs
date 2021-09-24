using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Transactions;
using grate.Configuration;
using grate.Infrastructure;
using Microsoft.Extensions.Logging;

namespace grate.Migration
{
    public class DbMigrator : IDbMigrator
    {
        private readonly ILogger<DbMigrator> _logger;
        private readonly IHashGenerator _hashGenerator;

        public DbMigrator(IFactory factory, ILogger<DbMigrator> logger, IHashGenerator hashGenerator, GrateConfiguration? configuration = null)
        {
            _logger = logger;
            _hashGenerator = hashGenerator;
            Configuration = configuration ?? GrateConfiguration.Default;
            Database = factory.GetService<DatabaseType, IDatabase>(Configuration.DatabaseType);
            StatementSplitter = new StatementSplitter(Database.StatementSeparatorRegex);
        }

        public Task InitializeConnections() => Database?.InitializeConnections(Configuration)!;

        public IDatabase Database { get; set; }
        public StatementSplitter StatementSplitter { get; }

        public async Task<bool> DatabaseExists() => await Database.DatabaseExists();

        public async Task<bool> CreateDatabase()
        {
            if (Configuration.CreateDatabase)
            {
                await Database.CreateDatabase();
                return true;
            }
            return false;
        }

        public Task DropDatabase()
        {
            Debug.Assert(Configuration.Drop, "How did we get into a drop command when not configured to do so?");
            return Database.DropDatabase();
        }

        public Task OpenConnection() => Database.OpenConnection();
        public Task CloseConnection() => Database.CloseConnection();

        public Task RunSupportTasks() => Database.RunSupportTasks();
        public Task<string> GetCurrentVersion() => Database.GetCurrentVersion();
        public Task<long> VersionTheDatabase(string newVersion) => Database.VersionTheDatabase(newVersion);
        public Task OpenAdminConnection() => Database.OpenAdminConnection();
        public Task CloseAdminConnection() => Database.CloseAdminConnection();

        public GrateConfiguration Configuration { get; set; }

        public async Task<bool> RunSql(string sql, string scriptName, MigrationType migrationType, long versionId,
            GrateEnvironment? environment,
            ConnectionType connectionType)
        {
            var theSqlRun = false;

            async Task LogAndRunSql()
            {
                _logger.LogInformation(" Running {scriptName} on {serverName} - {databaseName}.", scriptName, Database.ServerName, Database.DatabaseName);
                await RunTheActualSql(sql, scriptName, migrationType, versionId, connectionType);
                theSqlRun = true;
            }

            if (!InCorrectEnvironment(scriptName, environment))
            {
                return false;
            }

            // the scripts are stored in the database with tokens replaced. This means we need to do any token work _before_ we start checking hashes etc
            if (TokenReplacementEnabled)
            {
                sql = ReplaceTokensIn(sql);
            }

            if (await ThisScriptIsAlreadyRun(scriptName) && !IsEverytimeScript(scriptName, migrationType))
            {

                if (await ScriptChanged(scriptName, sql))
                {
                    // TODO: Add more options
                    var changeHandling = Configuration.WarnOnOneTimeScriptChanges ? ChangedScriptHandling.WarnAndRun : ChangedScriptHandling.Error;

                    switch (migrationType)
                    {
                        case MigrationType.Once when changeHandling == ChangedScriptHandling.Error:
                            await OneTimeScriptChangedInError(sql, scriptName, versionId);
                            break;

                        case MigrationType.Once when changeHandling == ChangedScriptHandling.WarnAndRun:
                            LogScriptChangedWarning(scriptName);
                            await LogAndRunSql();
                            break;

                        case MigrationType.AnyTime:
                        case MigrationType.EveryTime:
                            await LogAndRunSql();
                            break;
                    };
                }
                else
                {
                    _logger.LogInformation(" Skipped {scriptName} - {reason}.", scriptName, "No changes were found to run");
                }
            }
            else
            {
                await LogAndRunSql();
            }

            return theSqlRun;
        }

        enum ChangedScriptHandling
        {
            Error,
            WarnAndRun,
            WarnAndIgnore
        }

        private static bool InCorrectEnvironment(string scriptName, GrateEnvironment? env) => env?.ShouldRun(scriptName) ?? true;

        private static bool IsEverytimeScript(string scriptName, MigrationType migrationType) =>
            migrationType == MigrationType.EveryTime ||
            FileName(scriptName).StartsWith("everytime.", StringComparison.InvariantCultureIgnoreCase) ||
            FileName(scriptName).Contains(".everytime.", StringComparison.InvariantCultureIgnoreCase);


        private async Task<bool> ScriptChanged(string scriptName, string sql)
        {
            var currentHash = await Database.GetCurrentHash(scriptName);
            var newHash = GetHash(sql);

            return currentHash != newHash;
        }

        private static string FileName(string path) => new FileInfo(path).Name;

        private string GetHash(string sql)
        {
            return _hashGenerator.Hash(sql);
        }

        private Task<bool> ThisScriptIsAlreadyRun(string scriptName) => Database.HasRun(scriptName);


        /// <summary>
        /// Lazily initialised only if needed.
        /// </summary>
        private Dictionary<string, string?>? _tokens;
        private string ReplaceTokensIn(string sql)
        {
            if (_tokens == null)
            {
                _tokens = new TokenProvider(Configuration, Database).GetTokens();
            }

            return TokenReplacer.ReplaceTokens(_tokens, sql);
        }

        private bool TokenReplacementEnabled => !Configuration.DisableTokenReplacement;



        private async Task RunTheActualSql(
            string sql,
            string scriptName,
            MigrationType migrationType,
            long versionId,
            ConnectionType connectionType)
        {
            foreach (var statement in GetStatements(sql))
            {
                try
                {
                    await Database.RunSql(statement, connectionType);
                }
                catch (Exception ex)
                {
                    Database.Rollback();
                    Transaction.Current?.Dispose();

                    await RecordScriptInScriptsRunErrorsTable(scriptName, sql, statement, ex.Message, versionId);

                    await Database.CloseConnection();
                    throw;
                }
            }
            await RecordScriptInScriptsRunTable(scriptName, sql, migrationType, versionId);
        }

        private IEnumerable<string> GetStatements(string sql) => StatementSplitter.Split(sql);

        private void LogScriptChangedWarning(string scriptName)
        {
            _logger.LogWarning("{scriptName} is a one time script that has changed since it was run.", scriptName);
            _logger.LogDebug("Running script anyway due to WarnOnOneTimeScriptChanges option being set.");
        }

        /// <summary>
        /// Throws an exception about this script having changed, and rolls back transactions.
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="scriptName"></param>
        /// <param name="versionId"></param>
        /// <exception cref="Migration.OneTimeScriptChanged"></exception>
        private async Task OneTimeScriptChangedInError(string sql, string scriptName, long versionId)
        {
            Database.Rollback();
            Transaction.Current?.Dispose();

            string errorMessage =
                    $"{scriptName} has changed since the last time it was run. By default this is not allowed - scripts that run once should never change. To change this behavior to a warning, please set warnOnOneTimeScriptChanges to true and run again. Stopping execution.";
            await RecordScriptInScriptsRunErrorsTable(scriptName, sql, sql, errorMessage, versionId);
            await Database.CloseConnection();
            throw new OneTimeScriptChanged(errorMessage);
        }

        private Task RecordScriptInScriptsRunTable(string scriptName, string sql, MigrationType migrationType, long versionId)
        {
            var hash = _hashGenerator.Hash(sql);
            var sqlToStore = Configuration.DoNotStoreScriptsRunText ? null : sql;
            
            _logger.LogDebug("Recording {scriptName} script ran on {serverName} - {databaseName}.", scriptName, Database.ServerName, Database.DatabaseName);
            return Database.InsertScriptRun(scriptName, sqlToStore, hash, migrationType == MigrationType.Once, versionId);
        }

        private Task RecordScriptInScriptsRunErrorsTable(string scriptName, string sql, string errorSql, string errorMessage, long versionId)
        {
            var sqlToStore = Configuration.DoNotStoreScriptsRunText ? null : sql;
            return Database.InsertScriptRunError(scriptName, sqlToStore, errorSql, errorMessage, versionId);
        }

        public async ValueTask DisposeAsync()
        {
            // Allow the dbase to clean itself up
            await Database.DisposeAsync();
            GC.SuppressFinalize(this);
        }
    }
}
