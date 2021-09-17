using System.Data.Common;
using grate.Infrastructure;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;

namespace grate.Migration
{
    public class SqlServerDatabase : AnsiSqlDatabase
    {
        public SqlServerDatabase(ILogger<SqlServerDatabase> logger) 
            : base(logger, new SqlServerSyntax())
        { }

        public override bool SupportsDdlTransactions => true;
        public override bool SupportsSchemas => true;
        protected override DbConnection GetSqlConnection(string? connectionString) => new SqlConnection(connectionString);
    }
}
