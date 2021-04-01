using System.Threading.Tasks;
using moo.Configuration;

namespace moo.Migration
{
    public interface IDbMigrator
    {
        MooConfiguration Configuration { get; set; }
        IDatabase Database { get; set; }
        Task InitializeConnections();
        void ApplyConfig(MooConfiguration config);
        Task<bool> CreateDatabase();
        Task OpenConnection();
        void RunSupportTasks();
        string GetCurrentVersion();
        string VersionTheDatabase(string newVersion);
        Task OpenAdminConnection();
        Task CloseAdminConnection();
        bool RunSql(string sql, string scriptName, MigrationType migrationType, string versionId, object migratingEnvironmentSet, object repositoryVersion, object repositoryPath, ConnectionType connectionType);
    }
}