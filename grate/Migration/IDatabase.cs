using System;
using System.Threading.Tasks;
using grate.Configuration;

namespace grate.Migration
{
    public interface IDatabase : IAsyncDisposable
    {
        string? ServerName { get; }
        string? DatabaseName { get; }
        bool SupportsDdlTransactions { get; }

        bool SplitBatchStatements { get; }
        string StatementSeparatorRegex { get; }
        public string ScriptsRunTable { get; }
        public string ScriptsRunErrorsTable { get; }
        public string VersionTable { get; }

        Task InitializeConnections(GrateConfiguration configuration);
        Task OpenConnection();
        Task CloseConnection();
        Task OpenAdminConnection();
        Task CloseAdminConnection();
        Task CreateDatabase();

        /// <summary>
        /// Drops the databse if it exists, and does nothing if it doesn't.
        /// </summary>
        /// <returns></returns>
        Task DropDatabase();

        Task<bool> DatabaseExists();
        Task RunSupportTasks();
        Task<string> GetCurrentVersion();
        Task<long> VersionTheDatabase(string newVersion);
        void Rollback();
        Task RunSql(string sql, ConnectionType connectionType);
        Task<string?> GetCurrentHash(string scriptName);
        Task<bool> HasRun(string scriptName);
        Task InsertScriptRun(string scriptName, string? sql, string hash, bool runOnce, object versionId);
        Task InsertScriptRunError(string scriptName, string? sql, string errorSql, string errorMessage, long versionId);
    }
}
