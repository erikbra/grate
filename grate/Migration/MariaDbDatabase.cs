using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
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
    protected override bool SupportsSchemas => false;
    protected override DbConnection GetSqlConnection(string? connectionString) => new MySqlConnection(connectionString);

    public override Task RestoreDatabase(string backupPath)
    {
        throw new System.NotImplementedException("Restoring a database from file is not currently supported for Maria DB.");
    }

    public override async Task<bool> DatabaseExists()
    {
        var sql = $"SELECT SCHEMA_NAME FROM INFORMATION_SCHEMA.SCHEMATA WHERE SCHEMA_NAME = '{DatabaseName}'";
        try
        {
            await OpenConnection();
            var results = await Connection.QueryAsync<string>(sql, commandType: CommandType.Text);
            return results.Any();
        }
        catch (DbException ex)
        {
            Logger.LogDebug(ex, "An unexpected error occurred performing the CheckDatabaseExists check: {ErrorMessage}", ex.Message);
            return false; // base method also returns false on any DbException
        }
        finally
        {
            await CloseConnection();
        }
    }
}
