namespace grate.Infrastructure;

public class PostgreSqlSyntax : ISyntax
{
    public string StatementSeparatorRegex
    {
        get
        {
            const string strings = @"(?<KEEP1>'[^']*')";
            const string dashComments = @"(?<KEEP1>--.*$)";
            const string starComments = @"(?<KEEP1>/\*[\S\s]*?\*/)";
            const string separator = @"(?<KEEP1>.*)(?<BATCHSPLITTER>;)(?<KEEP2>.*)";
            return strings + "|" + dashComments + "|" + starComments + "|" + separator;
        }
    }

    public string CurrentDatabase => "SELECT current_database()";
    public string ListDatabases => "SELECT datname FROM pg_database";
    public string VarcharType => "varchar";
    public string TextType => "text";
    public string BigintType => "BIGINT";
    public string BooleanType => "boolean";
    public string PrimaryKeyColumn(string columnName) => $"{columnName} bigint GENERATED ALWAYS AS IDENTITY NOT NULL";
    public string CreateSchema(string schemaName) => @$"CREATE SCHEMA ""{schemaName}"";";
    public string CreateDatabase(string databaseName, string? _) => @$"CREATE DATABASE ""{databaseName}""";
    public string DropDatabase(string databaseName) => @$"select pg_terminate_backend(pid) from pg_stat_activity where datname='{databaseName}';
                                                              COMMIT;
                                                              DROP DATABASE IF EXISTS ""{databaseName}"";";
    public string TableWithSchema(string schemaName, string tableName) => $"{schemaName}.\"{tableName}\"";
    public string ReturnId => "RETURNING id;";
    public string TimestampType => "timestamp";
    public string Quote(string text) => $"\"{text}\"";
    public string PrimaryKeyConstraint(string tableName, string column) => $",\nCONSTRAINT PK_{tableName}_{column} PRIMARY KEY ({column})";
    public string LimitN(string sql, int n) => sql + $"\nLIMIT {n}";
}
