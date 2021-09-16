namespace grate.Infrastructure
{
    /// <summary>
    /// Interface for the various SQL dialects that grate supports.
    /// </summary>
    public interface ISyntax
    {
        string StatementSeparatorRegex { get; }
        string CurrentDatabase { get; }
        string ListDatabases { get; }
        string VarcharType { get; }
        string TextType { get; }
        string BooleanType { get; }
        string Identity(string columnDefinition, string nullability);
        string CreateSchema(string schemaName);
        string CreateDatabase(string databaseName);
        string DropDatabase(string databaseName);
        string TableWithSchema(string schemaName, string tableName);
        string PrimaryKey(string column);
        string LimitN(string sql, int n);
        string ReturnId { get; }
        string TimestampType { get; }
        string Quote(string text);
    }
}
