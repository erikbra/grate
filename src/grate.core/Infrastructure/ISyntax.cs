namespace grate.Infrastructure;

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
    string BigintType { get; }
    string BooleanType { get; }
    string CreateSchema(string schemaName);
    string CreateDatabase(string databaseName, string? password);
    /// <summary>
    /// Syntax to drop a database if it exists, and do nothing if not.
    /// </summary>
    /// <param name="databaseName"></param>
    /// <returns></returns>
    string DropDatabase(string databaseName);
    string TableWithSchema(string schemaName, string tableName);
    string LimitN(string sql, int n);
    string ReturnId { get; }
    string TimestampType { get; }
    string Quote(string text);
    string PrimaryKeyColumn(string columnName);
    string PrimaryKeyConstraint(string tableName, string column);
}
