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
        public string CreateDatabase(string databaseName) => @$"CREATE DATABASE ""{databaseName}""";
        public string DropDatabase(string databaseName) => @"USE master; 
                        IF EXISTS(SELECT * FROM sysdatabases WHERE [name] = '{databaseName}') 
                        BEGIN 
                            ALTER DATABASE [{databaseName}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE                            
                            DROP DATABASE [{databaseName}] 
                        END";
        public string TableWithSchema(string schemaName, string tableName) => $"{schemaName}.\"{tableName}\"";
        public string ReturnId => ";SELECT @@IDENTITY";
        public string TimestampType => "datetime";
        public string Quote(string text) => $"\"{text}\"";
        public string PrimaryKey(string column) => $"PRIMARY KEY CLUSTERED ({column})";
        public string LimitN(string sql, int n) => $"TOP {n}\n" + sql;
    }
}
