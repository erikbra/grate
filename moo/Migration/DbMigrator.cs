using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using moo.Configuration;
using moo.Infrastructure;

namespace moo.Migration
{
    public class DbMigrator: IDbMigrator
    {
        private readonly IFactory _factory;
        private readonly ILogger<DbMigrator> _logger;
        private readonly IHashGenerator _hashGenerator;

        public DbMigrator(IFactory factory, ILogger<DbMigrator> logger, IHashGenerator hashGenerator)
        {
            _factory = factory;
            _logger = logger;
            _hashGenerator = hashGenerator;
            Configuration = MooConfiguration.Default;
        }
        
        public async Task InitializeConnections()
        {
            await Database?.InitializeConnections(Configuration)!;
        }

        public IDatabase Database { get; set; } = null!;

        public void ApplyConfig(MooConfiguration config)
        {
            this.Configuration = config;
            Database = _factory.GetService<DatabaseType, IDatabase>(config.DatabaseType);
        }

        public async Task<bool> CreateDatabase()
        {
            if (Configuration.CreateDatabase)
            {
                await Database.CreateDatabase();
                return true;
            }
            return false;
        }

        public async Task OpenConnection() => await Database.OpenConnection();
        public async Task CloseConnection() => await Database.CloseConnection();

        public async Task RunSupportTasks() => await Database.RunSupportTasks();
        public Task<string> GetCurrentVersion() => Database.GetCurrentVersion();
        public Task<long> VersionTheDatabase(string newVersion) => Database.VersionTheDatabase(newVersion);
        public async Task OpenAdminConnection() => await Database.OpenAdminConnection();
        public async Task CloseAdminConnection() => await Database.CloseAdminConnection();

        public MooConfiguration Configuration { get; set; }

        public async Task<bool> RunSql(string sql, string scriptName, MigrationType migrationType, long versionId,
            object migratingEnvironmentSet, object repositoryVersion, object repositoryPath,
            ConnectionType connectionType)
        {
            var theSqlRun = false;

            if (await ThisScriptIsAlreadyRun(scriptName))
            {
                if (await ScriptChanged(scriptName, sql))
                {
                    switch (migrationType)
                    {
                        case MigrationType.Once:
                            OneTimeScriptChanged(sql, scriptName, versionId);
                            break;
                        case MigrationType.EveryTime:
                            await RunTheActualSql(sql, scriptName, migrationType, versionId, connectionType);
                            theSqlRun = true;
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
                await RunTheActualSql(sql, scriptName, migrationType, versionId, connectionType);
                theSqlRun = true;
            }

            return theSqlRun;
        }

        private async Task<bool> ScriptChanged(string scriptName, string sql)
        {
            var currentHash = await Database.GetCurrentHash(scriptName);
            var newHash = GetHash(sql);

            return currentHash != newHash;
        }

        private string GetHash(string sql)
        {
            return _hashGenerator.Hash(sql);
        }

        private Task<bool> ThisScriptIsAlreadyRun(string scriptName) => Database.HasRun(scriptName);

        private async Task RunTheActualSql(
            string sql, 
            string scriptName, 
            MigrationType migrationType, 
            long versionId,
            ConnectionType connectionType)
        {
            try
            {
                await Database.RunSql(sql, connectionType);
            }
            catch (Exception ex)
            {
                Database.Rollback();
                record_script_in_scripts_run_errors_table(scriptName, sql, sql, ex.Message, versionId);
                await Database.CloseConnection();
                throw;
            }

            await record_script_in_scripts_run_table(scriptName, sql, migrationType, versionId);
        }

        private void OneTimeScriptChanged(string sql, string scriptName, long versionId)
        {
            Database.Rollback();
            string errorMessage =
                $"{scriptName} has changed since the last time it was run. By default this is not allowed - scripts that run once should never change. To change this behavior to a warning, please set warnOnOneTimeScriptChanges to true and run again. Stopping execution.";
            record_script_in_scripts_run_errors_table(scriptName, sql, sql, errorMessage, versionId);
            Database.CloseConnection();
            throw new OneTimeScriptChanged(errorMessage);
        }

        private async Task record_script_in_scripts_run_table(string scriptName, string sql, MigrationType migrationType, long versionId)
        {
            var hash = _hashGenerator.Hash(sql);
            
            _logger.LogDebug("Recording {0} script ran on {1} - {2}.", scriptName, Database.ServerName, Database.DatabaseName);
            await Database.InsertScriptRun(scriptName, sql, hash, migrationType == MigrationType.Once, versionId);
        }

        private void record_script_in_scripts_run_errors_table(string scriptName, string sql, string errorSql, string errorMessage, long versionId)
        {
            Database.InsertScriptRunError(scriptName, sql, errorSql, errorMessage, versionId);
        }

        public async ValueTask DisposeAsync()
        {
            await Database.CloseConnection();
            await Database.CloseAdminConnection();
        }
    }
}