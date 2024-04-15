using grate.Infrastructure;
namespace grate.SqlServer.Infrastructure;

public readonly struct SqlServerSyntax : ISyntax
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
    public string CreateDatabase(string databaseName, string? _) => @$"CREATE DATABASE ""{databaseName}""";
    public string DropDatabase(string databaseName) => @$"USE master; 
                        IF EXISTS(SELECT * FROM sysdatabases WHERE [name] = '{databaseName}') 
                        BEGIN 
                            ALTER DATABASE [{databaseName}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE                            
                            DROP DATABASE [{databaseName}] 
                        END";
    public string TableWithSchema(string schemaName, string tableName) => $"{schemaName}.[{tableName}]";
    public string ReturnId => ";SELECT @@IDENTITY";
    public string LimitN(string sql, int n) => $"TOP {n}\n" + sql;
    public string ResetIdentity(string schemaName, string tableName, long _) => @$"DECLARE @max INT SELECT @max=ISNULL(max([id]),0) from [{schemaName}].[{tableName}]; DBCC CHECKIDENT ('[{schemaName}].[{tableName}]', RESEED, @max );";
}
