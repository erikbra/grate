using System;

namespace grate.Infrastructure;

public class OracleSyntax : ISyntax
{
    public string StatementSeparatorRegex
    {
        get
        {
            const string strings = @"(?<KEEP1>'[^']*')";
            const string dashComments = @"(?<KEEP1>--.*$)";
            const string starComments = @"(?<KEEP1>/\*[\S\s]*?\*/)";
            const string separator = @"(?<KEEP1>^|\s)(?<BATCHSPLITTER>/)(?<KEEP2>\s|;|$)";
            return strings + "|" + dashComments + "|" + starComments + "|" + separator;
        }
    }

    public string CurrentDatabase => "select user from dual";
    public string ListDatabases => "SELECT * FROM all_users";
    public string VarcharType => "VARCHAR2";
    public string TextType => "CLOB";
    public string BigintType => "NUMBER(19)";
    public string BooleanType => "CHAR(1)";
    public string PrimaryKeyColumn(string columnName) => $"{columnName} NUMBER(19) PRIMARY KEY";

    public string CreateSchema(string schemaName) => throw new NotImplementedException("Create schema is not implemented for Oracle DB");

    public string CreateDatabase(string userName, string? password) => $@"
begin 
    execute immediate 'CREATE USER {userName} IDENTIFIED BY ""{password}""';
    execute immediate 'GRANT ALL PRIVILEGES TO {userName}';
end;
";
    public string DropDatabase(string databaseName) => $@"
begin 
    DECLARE
       usr VARCHAR2 (32) := '{databaseName}';
    BEGIN
       FOR ln_cur IN (SELECT sid, serial# FROM v$session WHERE username = usr)
       LOOP
          EXECUTE IMMEDIATE ('ALTER SYSTEM KILL SESSION ''' || ln_cur.sid || ',' || ln_cur.serial# || ''' IMMEDIATE');
       END LOOP;
    END;

    EXECUTE IMMEDIATE 'DROP USER {databaseName} CASCADE';
end;
";


    public string TableWithSchema(string schemaName, string tableName) => $"{schemaName}_{tableName}";
    public string ReturnId => "RETURNING id;";
    public string TimestampType => "timestamp";
    public string Quote(string text) => $"\"{text}\"";
    public string PrimaryKeyConstraint(string tableName, string column) => "";
    public string LimitN(string sql, int n) => sql + $"\nLIMIT {n}";
}
