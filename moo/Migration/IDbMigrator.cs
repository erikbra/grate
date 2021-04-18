using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using moo.Configuration;
using moo.Infrastructure;

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
        Task CloseConnection();
        Task RunSupportTasks();
        Task<string> GetCurrentVersion();
        Task<long> VersionTheDatabase(string newVersion);
        Task OpenAdminConnection();
        Task CloseAdminConnection();
        Task<bool> RunSql(string sql, string scriptName, MigrationType migrationType, long versionId,
            IEnumerable<MooEnvironment> environments, object repositoryVersion, object repositoryPath,
            ConnectionType connectionType);
    }
}