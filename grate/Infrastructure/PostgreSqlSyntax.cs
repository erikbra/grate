namespace grate.Infrastructure
{
    public class PostgreSqlSyntax : ISyntax
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

        public string CurrentDatabase => "SELECT current_database()";
        public string ListDatabases => "SELECT datname FROM pg_database";
        public string VarcharType => "varchar";
        public string TextType => "text";
        public string BooleanType => "boolean";
        public string Identity(string columnDefinition, string nullability) => $"{columnDefinition} GENERATED ALWAYS AS IDENTITY {nullability}";
        public string CreateSchema(string schemaName) => @$"CREATE SCHEMA ""{schemaName}"";";
        public string CreateDatabase(string databaseName) => @$"CREATE DATABASE ""{databaseName}""";
        public string DropDatabase(string databaseName) => @$"select pg_terminate_backend(pid) from pg_stat_activity where datname='{databaseName}';
                                                              DROP DATABASE IF EXISTS ""{databaseName}"";";
        public string TableWithSchema(string schemaName, string tableName) => $"{schemaName}.\"{tableName}\"";
        public string ReturnId => "RETURNING id;";
        public string TimestampType => "timestamp";
        public string Quote(string text) => $"\"{text}\"";
        public string PrimaryKey(string column) => $"PRIMARY KEY ({column})";
        public string LimitN(string sql, int n) => sql + "\nLIMIT 1";
    }
}
