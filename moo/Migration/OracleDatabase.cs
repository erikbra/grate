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
    }
}