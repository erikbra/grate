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

        protected override DbConnection GetSqlConnection(string? connectionString) => new SqlConnection(connectionString);
    }
}