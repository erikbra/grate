namespace grate.Infrastructure;

public class SqliteSyntax : ISyntax
{
    public string StatementSeparatorRegex
    {
        get
        {
            const string strings = @"(?<KEEP1>'[^']*')";
            const string dashComments = @"(?<KEEP1>--.*$)";
            const string starComments = @"(?<KEEP1>/\*[\S\s]*?\*/)";
            const string separator = @"(?<KEEP1>^|\s)(?<BATCHSPLITTER>GO)(?<KEEP2>\s|;|$)";
            return strings + "|" + dashComments + "|" + starComments + "|" + separator;
        }
    }

    public string CurrentDatabase => "SELECT name FROM pragma_database_list ORDER BY seq DESC LIMIT 1";
    public string ListDatabases => "select name from pragma_database_list";
    public string VarcharType => "nvarchar";
    public string TextType => "ntext";
    public string BigintType => "BIGINT";
    public string BooleanType => "bit";
    public string PrimaryKeyColumn(string columnName) => $"{columnName} INTEGER PRIMARY KEY NOT NULL";
    public string CreateSchema(string schemaName) => @$"CREATE SCHEMA ""{schemaName}"";";

    // The "Create database" is a no-op with Sqlite, so we just provide a dummy SQL that just selects current DB
    public string CreateDatabase(string databaseName, string? _) => CurrentDatabase;

    // The "Drop database" is done via file deletion in Sqlite, so this isn't used.
    public string DropDatabase(string databaseName) => CurrentDatabase;

    public string TableWithSchema(string schemaName, string tableName) => $"{schemaName}_{tableName}";
    public string ReturnId => "returning id;";
    public string TimestampType => "datetime";
    public string Quote(string text) => $"\"{text}\"";
    public string PrimaryKeyConstraint(string tableName, string column) => "";
    public string LimitN(string sql, int n) => sql + $"\nLIMIT {n}";
}
