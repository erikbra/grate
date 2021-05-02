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

        protected override DbConnection GetSqlConnection(string? connectionString) => new NpgsqlConnection(connectionString);
    }
}