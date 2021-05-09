using System.Data.Common;
using grate.Infrastructure;
using Microsoft.Extensions.Logging;
using Npgsql;

namespace grate.Migration
{
    public class PostgreSqlDatabase : AnsiSqlDatabase
    {
        public PostgreSqlDatabase(ILogger<PostgreSqlDatabase> logger) 
            : base(logger, new PostgreSqlSyntax())
        { }

        public override bool SupportsDdlTransactions => true;
        public override bool SupportsSchemas => true;
        protected override DbConnection GetSqlConnection(string? connectionString) => new NpgsqlConnection(connectionString);
    }
}