namespace grate.Infrastructure;

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

    public string CurrentDatabase => "Select Db_Name()";
    public string ListDatabases => "Select name From sys.databases Where database_id >= 5 And State = 0 And Is_In_Standby = 0 And Is_ReadOnly = 0;";
    public string VarcharType => "NVarChar";
    public string TextType => "NVarChar(Max)";
    public string BigintType => "BigInt";
    public string BooleanType => "Bit";
    public string PrimaryKeyColumn(string columnName) => $"{columnName} Int Identity(1, 1) Not Null";
    public string CreateSchema(string schemaName) => @$"Create Schema [{schemaName}];";
    public string CreateDatabase(string databaseName, string? _) => @$"Create Database [{databaseName}]";
    public string DropDatabase(string databaseName) => @$"Use master; 
If Db_Id('{databaseName}') Is Not Null; Begin 
    Alter Database [{databaseName}] Set Single_User With Rollback Immediate;                                
    Drop Database [{databaseName}]; 
End";
    public string TableWithSchema(string schemaName, string tableName) => $"[{schemaName}].{tableName}";
    public string ReturnId => ";Select Scope_Identity()";
    public string TimestampType => "DateTime2(0)";
    public string Quote(string text) => $"'{text}'";
    public string PrimaryKeyConstraint(string tableName, string column) => $",\nConstraint [PK_{tableName}_{column}] Primary Key Clustered ({column})";
    public string LimitN(string sql, int n) => $"Top ({n})\n" + sql;
}
