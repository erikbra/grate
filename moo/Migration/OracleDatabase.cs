using System.Threading.Tasks;
using moo.Configuration;

namespace moo.Migration
{
    public class OracleDatabase : IDatabase
    {
        public string? ServerName { get; set; }
        public string? DatabaseName { get; set; }
        public bool SupportsDdlTransactions => false;

        public Task InitializeConnections(MooConfiguration? configuration)
        {
            throw new System.NotImplementedException();
        }

        public Task OpenConnection()
        {
            throw new System.NotImplementedException();
        }

        public void CloseConnection()
        {
            throw new System.NotImplementedException();
        }

        public Task OpenAdminConnection()
        {
            throw new System.NotImplementedException();
        }

        public Task CloseAdminConnection()
        {
            throw new System.NotImplementedException();
        }

        public Task CreateDatabase()
        {
            throw new System.NotImplementedException();
        }

        public Task RunSupportTasks()
        {
            throw new System.NotImplementedException();
        }

        public string GetCurrentVersion()
        {
            throw new System.NotImplementedException();
        }

        public string VersionTheDatabase(string newVersion)
        {
            throw new System.NotImplementedException();
        }

        public void Rollback()
        {
            throw new System.NotImplementedException();
        }

        public void RunSql(string sql, ConnectionType connectionType)
        {
            throw new System.NotImplementedException();
        }

        public string GetCurrentHash(string scriptName)
        {
            throw new System.NotImplementedException();
        }

        public bool HasRun(string scriptName)
        {
            throw new System.NotImplementedException();
        }

        public void InsertScriptRun(string scriptName, string sql, string hash, bool runOnce, object versionId)
        {
            throw new System.NotImplementedException();
        }

        public void InsertScriptRunError(string scriptName, string sql, string errorSql, string errorMessage, object versionId)
        {
            throw new System.NotImplementedException();
        }
    }
}