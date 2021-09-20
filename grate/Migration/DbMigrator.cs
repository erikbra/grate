﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using grate.Configuration;
using grate.Infrastructure;
using Microsoft.Extensions.Logging;

namespace grate.Migration
{
    public class DbMigrator : IDbMigrator
    {
        private readonly IFactory _factory;
        private readonly ILogger<DbMigrator> _logger;
        private readonly IHashGenerator _hashGenerator;

        public DbMigrator(IFactory factory, ILogger<DbMigrator> logger, IHashGenerator hashGenerator)
        {
            _factory = factory;
            _logger = logger;
            _hashGenerator = hashGenerator;
            Configuration = GrateConfiguration.Default;
        }

        public Task InitializeConnections() => Database?.InitializeConnections(Configuration)!;

        public IDatabase Database { get; set; } = null!;
        public StatementSplitter StatementSplitter { get; set; } = null!;

        public void ApplyConfig(GrateConfiguration config)
        {
            this.Configuration = config;
            Database = _factory.GetService<DatabaseType, IDatabase>(config.DatabaseType);
            StatementSplitter = new StatementSplitter(Database.StatementSeparatorRegex);
        }

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

            // the scripts are stored in the databse with tokens replaced. This means we need to do any token work _before_ we start checking hashes etc
            if (TokenReplacementEnabled)
            {
                sql = ReplaceTokensIn(sql);
            }

            if (await ThisScriptIsAlreadyRun(scriptName) && !IsEverytimeScript(scriptName, migrationType))
            {

                if (await ScriptChanged(scriptName, sql))
                {
                    switch (migrationType)
                    {
                        case MigrationType.Once:
                            await OneTimeScriptChanged(sql, scriptName, versionId);
                            break;
                        case MigrationType.AnyTime:
                        case MigrationType.EveryTime:
                            await LogAndRunSql();
                            break;
                    };
                }
                else
                {
                    _logger.LogInformation(" Skipped {0} - {1}.", scriptName, "No changes were found to run");
                }
            }
            else
            {
                await LogAndRunSql();
            }

            return theSqlRun;
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

        private async Task OneTimeScriptChanged(string sql, string scriptName, long versionId)
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

            _logger.LogDebug("Recording {0} script ran on {1} - {2}.", scriptName, Database.ServerName, Database.DatabaseName);
            return Database.InsertScriptRun(scriptName, sql, hash, migrationType == MigrationType.Once, versionId);
        }

        private Task RecordScriptInScriptsRunErrorsTable(string scriptName, string sql, string errorSql, string errorMessage, long versionId)
        {
            return Database.InsertScriptRunError(scriptName, sql, errorSql, errorMessage, versionId);
        }

        public async ValueTask DisposeAsync()
        {
            // Allow the dbase to clean itself up
            await Database.DisposeAsync();
            GC.SuppressFinalize(this);
        }
    }
}
