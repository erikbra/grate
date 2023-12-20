using System;
using System.Data.Common;
using System.Diagnostics;
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

    protected override DbConnection GetSqlConnection(string? connectionString)
    {
        //if we actually get a null here we're in trouble, but the cascading changes of making the param
        //non-null are significant.
        connectionString ??= "";

        // If pipelining is not explicitly mentioned in the connection string, turn it off, as enabling it
        // might lead to problems in more scenarios than it (potentially) solves, in the most
        // common grate scenarios.
        if (!connectionString.Contains("Pipelining", StringComparison.InvariantCultureIgnoreCase))
        {
            var builder = new MySqlConnectionStringBuilder(connectionString) { Pipelining = false };
            connectionString = builder.ConnectionString;
        }

        var conn = new MySqlConnection(connectionString);
        return conn;
    }

    public override Task RestoreDatabase(string backupPath)
    {
        throw new System.NotImplementedException("Restoring a database from file is not currently supported for Maria DB.");
    }

    public override async Task DropDatabase()
    {
        // Drop the database in normal fashion
        await base.DropDatabase();

        // We need to kill any active connections to get MariaDB to actually delete the database,
        // and stop accepting new connections to it. So we create a list of the
        // active sessions against our database, and create 'KILL X' statements (where X is session id).
        // Then we execute the kill statements.
        var sql = $@"
SELECT GROUP_CONCAT(CONCAT('KILL ',id,';') SEPARATOR ' ')
FROM information_schema.processlist WHERE DB = '{DatabaseName}'";

        var killStatements = await ExecuteScalarAsync<object>(AdminConnection, sql);
        if (killStatements != null && !DBNull.Value.Equals(killStatements))
        {
            string killSql = killStatements.ToString() ?? ""; // Just to keep warnings happy
            await ExecuteNonQuery(AdminConnection, killSql, null);
        }

        var databaseExists = await DatabaseExists();
        Debug.Assert(!databaseExists, "Database still exists after it is dropped");
    }
}
