using System.Data.Common;
using System.Threading.Tasks;
using grate.Infrastructure;
using Microsoft.Extensions.Logging;
using Npgsql;

namespace grate.Migration;

public class PostgreSqlDatabase : AnsiSqlDatabase
{
    public PostgreSqlDatabase(ILogger<PostgreSqlDatabase> logger)
        : base(logger, new PostgreSqlSyntax())
    { }

    public override bool SupportsDdlTransactions => true;
    protected override bool SupportsSchemas => true;
    protected override DbConnection GetSqlConnection(string? connectionString) => new NpgsqlConnection(connectionString);

    public override Task RestoreDatabase(string backupPath)
    {
        throw new System.NotImplementedException("Restoring a database from file is not currently supported for  Postgresql.");
    }

    protected override string ExistsSql(string tableSchema, string fullTableName)
    {
        // For #230.  Postgres tables are lowercase by default unless you quote them when created, which we do.  We _don't_ quote the schema though, so it will always be lowercase
        // Ensure the table check uses the lowercase version of anything we're passed, as that's what we would have created.
        return base.ExistsSql(tableSchema.ToLower(), fullTableName);
    }

    protected override string ExistsSql(string tableSchema, string fullTableName, string columnName)
    {
        // For #230.  Postgres tables are lowercase by default unless you quote them when created, which we do.  We _don't_ quote the schema though, so it will always be lowercase
        // Ensure the table check uses the lowercase version of anything we're passed, as that's what we would have created.
        return base.ExistsSql(tableSchema.ToLower(), fullTableName, columnName);
    }
}
