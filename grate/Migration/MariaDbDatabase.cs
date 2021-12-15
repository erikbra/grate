using System.Data.Common;
using System.Threading.Tasks;
using grate.Infrastructure;
using Microsoft.Extensions.Logging;
using MySqlConnector;

namespace grate.Migration;

public class MariaDbDatabase : AnsiSqlDatabase
{
    public MariaDbDatabase(ILogger<MariaDbDatabase> logger) 
        : base(logger, new MariaDbSyntax())
    { }

    public override bool SupportsDdlTransactions => false;
    public override bool SupportsSchemas => false;
    protected override DbConnection GetSqlConnection(string? connectionString) => new MySqlConnection(connectionString);

    public override Task RestoreDatabase(string backupPath)
    {
        throw new System.NotImplementedException("Restoring a database from file is not currently supported for Maria DB.");
    }
}