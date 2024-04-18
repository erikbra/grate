using grate.Infrastructure;
namespace grate.MariaDb.Infrastructure;

public readonly struct MariaDbSyntax : ISyntax
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
    public string CreateDatabase(string databaseName, string? _) => @$"CREATE DATABASE {databaseName}";
    public string DropDatabase(string databaseName) => @$"DROP DATABASE IF EXISTS `{databaseName}`;";
    public string TableWithSchema(string schemaName, string tableName) => $"{schemaName}_{tableName}";
    public string ReturnId => ";SELECT LAST_INSERT_ID();";
    public string LimitN(string sql, int n) => sql + "\nLIMIT 1";

    // any idea to reset identity without using value is welcome.
    public string ResetIdentity(string schemaName, string tableName, long value) => @$"ALTER TABLE {TableWithSchema(schemaName, tableName)} AUTO_INCREMENT = {value}";
}
