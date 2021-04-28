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
        Task OpenConnection();
        Task CloseConnection();
        Task RunSupportTasks();
        Task<string> GetCurrentVersion();
        Task<long> VersionTheDatabase(string newVersion);
        Task OpenAdminConnection();
        Task CloseAdminConnection();
        Task<bool> RunSql(string sql, string scriptName, MigrationType migrationType, long versionId,
            IEnumerable<GrateEnvironment> environments,
            ConnectionType connectionType);
    }
}