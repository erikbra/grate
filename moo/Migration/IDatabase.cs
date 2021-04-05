using System.Threading.Tasks;
using moo.Configuration;

namespace moo.Migration
{
    public interface IDatabase
    {
        string? ServerName { get; }
        string? DatabaseName { get; }
        bool SupportsDdlTransactions { get; }
        Task InitializeConnections(MooConfiguration configuration);
        Task OpenConnection();
        Task CloseConnection();
        Task OpenAdminConnection();
        Task CloseAdminConnection();
        Task CreateDatabase();
        Task RunSupportTasks();
        Task<string> GetCurrentVersion();
        Task<long> VersionTheDatabase(string newVersion);
        void Rollback();
        Task RunSql(string sql, ConnectionType connectionType);
        Task<string?> GetCurrentHash(string scriptName);
        Task<bool> HasRun(string scriptName);
        Task InsertScriptRun(string scriptName, string sql, string hash, bool runOnce, object versionId);
        void InsertScriptRunError(string scriptName, string sql, string errorSql, string errorMessage, object versionId);
    }
}