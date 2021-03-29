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
        bool CreateDatabase();
        void OpenConnection();
        void RunSupportTasks();
        string GetCurrentVersion();
        string VersionTheDatabase(string newVersion);
        void OpenAdminConnection();
        void CloseAdminConnection();
        bool RunSql(string sql, string scriptName, MigrationType migrationType, string versionId, object migratingEnvironmentSet, object repositoryVersion, object repositoryPath, ConnectionType connectionType);
    }
}