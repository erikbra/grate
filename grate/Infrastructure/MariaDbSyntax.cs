namespace grate.Infrastructure
{
    public class MariaDbSyntax : ISyntax
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

        public string CurrentDatabase => "SELECT DATABASE()";
        public string ListDatabases => "SHOW DATABASES";
        public string VarcharType => "varchar";
        public string TextType => "text";
        public string BooleanType => "boolean";
        public string Identity(string columnDefinition, string nullability) => $"{columnDefinition} {nullability} AUTO_INCREMENT";
        public string CreateSchema(string schemaName) => @$"CREATE SCHEMA {schemaName}";
        public string CreateDatabase(string schemaName) => @$"CREATE DATABASE {schemaName}";
        public string TableWithSchema(string schemaName, string tableName) => $"{schemaName}_{tableName}";
        public string ReturnId => ";SELECT LAST_INSERT_ID();";
        public string TimestampType => "timestamp";
        public string Quote(string text) => $"`{text}`";
        public string PrimaryKey(string column) => $"PRIMARY KEY ({column})";
        public string LimitN(string sql, int n) => sql + "\nLIMIT 1";
    }
}