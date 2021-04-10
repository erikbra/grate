using System.Threading.Tasks;
using moo.Configuration;

namespace moo.Migration
{
    public class OracleDatabase : IDatabase
    {
        public string? ServerName { get; set; }
        public string? DatabaseName { get; set; }
        public bool SupportsDdlTransactions => false;
        
        public bool SplitBatchStatements => true;
        
        public string StatementSeparatorRegex =>  @"(?<KEEP1>^(?:.)*(?:-{2}).*$)|(?<KEEP1>/{1}\*{1}[\S\s]*?\*{1}/{1})|(?<KEEP1>^|\s)(?<BATCHSPLITTER>;)(?<KEEP2>\s|$)";

        public Task InitializeConnections(MooConfiguration? configuration)
        {
            throw new System.NotImplementedException();
        }

        public Task OpenConnection()
        {
            throw new System.NotImplementedException();
        }

        public Task CloseConnection()
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

        public Task<string> GetCurrentVersion()
        {
            throw new System.NotImplementedException();
        }

        public Task<long> VersionTheDatabase(string newVersion)
        {
            throw new System.NotImplementedException();
        }

        public void Rollback()
        {
            throw new System.NotImplementedException();
        }

        public Task RunSql(string sql, ConnectionType connectionType)
        {
            throw new System.NotImplementedException();
        }

        public Task<string?> GetCurrentHash(string scriptName)
        {
            throw new System.NotImplementedException();
        }

        public Task<bool> HasRun(string scriptName)
        {
            throw new System.NotImplementedException();
        }

        public Task InsertScriptRun(string scriptName, string sql, string hash, bool runOnce, object versionId)
        {
            throw new System.NotImplementedException();
        }

        public Task InsertScriptRunError(string scriptName, string sql, string errorSql, string errorMessage, long versionId)
        {
            throw new System.NotImplementedException();
        }
    }
}