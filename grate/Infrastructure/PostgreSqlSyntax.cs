namespace grate.Infrastructure
{
    internal class PostgreSqlSyntax : ISyntax
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
        public string Identity => "GENERATED ALWAYS AS IDENTITY";
        public string ReturnId => "RETURNING id;";
        public string TimestampType => "timestamp";
        public string PrimaryKey(string column) => $"PRIMARY KEY ({column})";
        public string LimitN(string sql, int n) => sql + "\nLIMIT 1";
    }
}