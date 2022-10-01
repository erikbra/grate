using System.Data.Common;
using System.IO;
using System.Threading.Tasks;
using grate.Infrastructure;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Logging;

namespace grate.Migration;

public class SqliteDatabase : AnsiSqlDatabase
{
    private static readonly SqliteSyntax Syntax = new();
            
        
    public SqliteDatabase(ILogger<SqliteDatabase> logger) 
        : base(logger, Syntax)
    { }

    public override bool SupportsDdlTransactions => false;
    public override bool SupportsSchemas => false;
    protected override DbConnection GetSqlConnection(string? connectionString) => new SqliteConnection(connectionString);

    protected override string ExistsSql(string tableSchema, string fullTableName) =>
        $@"
SELECT name FROM sqlite_master 
WHERE type ='table' AND 
LOWER(name) = LOWER('{fullTableName}');
";

    protected override string ExistsSql(string tableSchema, string fullTableName, string columnName) =>
    $@"SELECT * FROM pragma_table_info('{fullTableName}')
WHERE name='{columnName}'";

    public override string DatabaseName => GetDatabaseName(Connection);

    /// <summary>
    /// Dropping a database in Sqlite is a bit different, it's just a matter of deleting a file on disk.
    /// </summary>
    /// <returns></returns>
    public override Task DropDatabase()
    {
        var db = Connection.DataSource;

        SqliteConnection.ClearAllPools();

        if (File.Exists(db))
        {
            File.Delete(db);
        }
            
        return Task.CompletedTask;
    }

    public override Task<bool> DatabaseExists()
    {
        var file = Connection.DataSource;
        return Task.FromResult(File.Exists(file));
    }

    private static string GetDatabaseName(DbConnection conn) => Path.GetFileNameWithoutExtension(conn.DataSource);

    public override Task RestoreDatabase(string backupPath)
    {
        throw new System.NotImplementedException("Restoring a database from file is not currently supported for  SqlLite.");
    }
}
