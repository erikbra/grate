namespace grate.Infrastructure
{
    public class SqlServerSyntax : ISyntax
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

        public string CurrentDatabase => "SELECT DB_NAME()";
        public string ListDatabases => "SELECT name FROM sys.databases";
        public string VarcharType => "nvarchar";
        public string TextType => "ntext";
        public string BooleanType => "bit";
        public string Identity(string columnDefinition, string nullability) => $"{columnDefinition} IDENTITY(1,1) {nullability}";
        public string CreateSchema(string schemaName) => @$"CREATE SCHEMA ""{schemaName}"";";
        public string CreateDatabase(string schemaName) => @$"CREATE DATABASE ""{schemaName}""";
        public string ReturnId => ";SELECT @@IDENTITY";
        public string TimestampType => "datetime";
        public string Quote(string text) => $"\"{text}\"";
        public string PrimaryKey(string column) => $"PRIMARY KEY CLUSTERED ({column})";
        public string LimitN(string sql, int n) => $"TOP {n}\n" + sql;
    }
}