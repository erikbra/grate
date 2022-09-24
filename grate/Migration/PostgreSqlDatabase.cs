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
    public override bool SupportCreateCustomScript => true;
    protected override DbConnection GetSqlConnection(string? connectionString) => new NpgsqlConnection(connectionString);

    public override Task RestoreDatabase(string backupPath)
    {
        throw new System.NotImplementedException("Restoring a database from file is not currently supported for  Postgresql.");
    }
}
