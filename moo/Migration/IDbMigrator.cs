using System;
using System.Threading.Tasks;
using moo.Configuration;

namespace moo.Migration
{
    public interface IDbMigrator: IAsyncDisposable
    {
        MooConfiguration Configuration { get; set; }
        IDatabase Database { get; set; }
        Task InitializeConnections();
        void ApplyConfig(MooConfiguration config);
        Task<bool> CreateDatabase();
        Task OpenConnection();
        Task RunSupportTasks();
        Task<string> GetCurrentVersion();
        Task<long> VersionTheDatabase(string newVersion);
        Task OpenAdminConnection();
        Task CloseAdminConnection();
        Task<bool> RunSql(string sql, string scriptName, MigrationType migrationType, long versionId,
            object migratingEnvironmentSet, object repositoryVersion, object repositoryPath,
            ConnectionType connectionType);
    }
}