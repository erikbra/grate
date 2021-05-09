using System.Data.Common;
using grate.Infrastructure;
using Microsoft.Extensions.Logging;
using MySqlConnector;

namespace grate.Migration
{
    public class MariaDbDatabase : AnsiSqlDatabase
    {
        public MariaDbDatabase(ILogger<MariaDbDatabase> logger) 
            : base(logger, new MariaDbSyntax())
        { }

        public override bool SupportsDdlTransactions => false;
        public override bool SupportsSchemas => false;
        protected override DbConnection GetSqlConnection(string? connectionString) => new MySqlConnection(connectionString);
    }
}