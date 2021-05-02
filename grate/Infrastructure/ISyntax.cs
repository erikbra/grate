namespace grate.Infrastructure
{
    public interface ISyntax
    {
        string StatementSeparatorRegex { get; }
        string CurrentDatabase { get; }
        string ListDatabases { get; }
        string VarcharType { get; }
        string TextType { get; }
        string BooleanType { get; }
        string Identity { get;  }
        
        string PrimaryKey(string column);
        string LimitN(string sql, int n);
        string ReturnId { get; }
        string TimestampType { get; }
    }
}