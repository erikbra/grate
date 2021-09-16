using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using grate.Configuration;
using grate.Infrastructure;

namespace grate.Migration
{
    public interface IDbMigrator: IAsyncDisposable
    {
        GrateConfiguration Configuration { get; set; }
        IDatabase Database { get; set; }
        Task InitializeConnections();
        void ApplyConfig(GrateConfiguration config);
        Task<bool> CreateDatabase();

        /// <summary>
        /// Requests a database drop if configuration allows it.
        /// </summary>
        /// <returns>Returns whether the database was actually dropped or not.</returns>
        Task<bool> DropDatabase();
        Task<bool> DatabaseExists();
        Task OpenConnection();
        Task CloseConnection();
        Task RunSupportTasks();
        Task<string> GetCurrentVersion();
        Task<long> VersionTheDatabase(string newVersion);
        Task OpenAdminConnection();
        Task CloseAdminConnection();
        Task<bool> RunSql(string sql, string scriptName, MigrationType migrationType, long versionId,
            GrateEnvironment? environment,
            ConnectionType connectionType);
    }
}
