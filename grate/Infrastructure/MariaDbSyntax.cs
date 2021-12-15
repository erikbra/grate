namespace grate.Infrastructure;

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
    public string BigintType => "BIGINT";
    public string BooleanType => "boolean";
    public string PrimaryKeyColumn(string columnName) => $"{columnName} bigint NOT NULL AUTO_INCREMENT";
    public string CreateSchema(string schemaName) => @$"CREATE SCHEMA {schemaName}";
    public string CreateDatabase(string databaseName, string? _) => @$"CREATE DATABASE {databaseName}";
    public string DropDatabase(string databaseName) => @$"DROP DATABASE IF EXISTS `{databaseName}`;";
    public string TableWithSchema(string schemaName, string tableName) => $"{schemaName}_{tableName}";
    public string ReturnId => ";SELECT LAST_INSERT_ID();";
    public string TimestampType => "timestamp";
    public string Quote(string text) => $"`{text}`";
    public string PrimaryKeyConstraint(string tableName, string column) => $",\nCONSTRAINT PK_{tableName}_{column} PRIMARY KEY ({column})";
    public string LimitN(string sql, int n) => sql + "\nLIMIT 1";
}