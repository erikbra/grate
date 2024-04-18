using grate.Infrastructure;
namespace grate.PostgreSql.Infrastructure;

public readonly struct PostgreSqlSyntax : ISyntax
{
    public string StatementSeparatorRegex
    {
        get
        {
            const string strings = @"(?<KEEP1>'([^']|\'\')*')";
            const string backslashEscapedStrings = @"(?<KEEP1>E(?<!\\)('[\S\s]*?(?<!\\)'))";
            const string dollarQuotedStrings = @"(?<KEEP1>\$(?'tag'\w*)\$[\S\s]*?\$\k'tag'\$)";
            const string dashComments = "(?<KEEP1>--.*$)";
            const string starComments = @"(?<KEEP1>/\*[\S\s]*?\*/)";
            const string separator = "(?<KEEP1>.*)(?<BATCHSPLITTER>(;)(?=(?:[^']|'[^']*')*$))(?<KEEP2>.*)";
            return strings + "|" + backslashEscapedStrings + "|" + dollarQuotedStrings + "|" + dashComments + "|" + starComments + "|" + separator;
        }
    }

    public string CurrentDatabase => "SELECT current_database()";
    public string ListDatabases => "SELECT datname FROM pg_database";
    public string CreateDatabase(string databaseName, string? _) => @$"CREATE DATABASE ""{databaseName}""";
    public string DropDatabase(string databaseName) => @$"select pg_terminate_backend(pid) from pg_stat_activity where datname='{databaseName}';
                                                              COMMIT;
                                                              DROP DATABASE IF EXISTS ""{databaseName}"";";
    public string TableWithSchema(string schemaName, string tableName) => $"\"{schemaName}\".\"{tableName}\"";
    public string ReturnId => "RETURNING id;";
    public string LimitN(string sql, int n) => sql + $"\nLIMIT {n}";
    public string ResetIdentity(string schemaName, string tableName, long _) => @$"SELECT setval(pg_get_serial_sequence('{TableWithSchema(schemaName, tableName)}', 'id'), coalesce(MAX(id), 1)) from {TableWithSchema(schemaName, tableName)};";
}
