using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using Dapper;
using grate.Configuration;
using grate.Infrastructure;
using Microsoft.Extensions.Logging;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using static System.StringSplitOptions;

namespace grate.Migration;

public class OracleDatabase : AnsiSqlDatabase
{
    public OracleDatabase(ILogger<OracleDatabase> logger)
        : base(logger, new OracleSyntax())
    {
    }

    public override bool SupportsDdlTransactions => false;
    protected override bool SupportsSchemas => false;

    protected override DbConnection GetSqlConnection(string? connectionString) => new OracleConnection(connectionString);

    protected override string ExistsSql(string tableSchema, string fullTableName) =>
        $@"
SELECT * FROM user_tables
WHERE 
lower(table_name) = '{fullTableName.ToLowerInvariant()}'
";

    protected override string ExistsSql(string tableSchema, string fullTableName, string columnName) =>
$@"
SELECT * FROM user_tab_columns
WHERE 
lower(table_name) = '{fullTableName.ToLowerInvariant()}' AND
lower(column_name) = '{columnName.ToLowerInvariant()}'
";

    protected override string CurrentVersionSql => $@"
SELECT version
FROM 
    (SELECT version,
            ROW_NUMBER() OVER (ORDER BY version DESC) AS version_row_number 
    FROM {VersionTable})
WHERE  version_row_number <= 1
";

    protected override async Task CreateScriptsRunTable()
    {
        if (!await ScriptsRunTableExists())
        {
            await base.CreateScriptsRunTable();
            await CreateIdSequence(ScriptsRunTable);
            await CreateIdInsertTrigger(ScriptsRunTable);
        }
    }

    protected override async Task CreateScriptsRunErrorsTable()
    {
        if (!await ScriptsRunErrorsTableExists())
        {
            await base.CreateScriptsRunErrorsTable();
            await CreateIdSequence(ScriptsRunErrorsTable);
            await CreateIdInsertTrigger(ScriptsRunErrorsTable);
        }
    }

    public override Task RestoreDatabase(string backupPath)
    {
        throw new System.NotImplementedException("Restoring a database from file is not currently supported for Oracle.");
    }

    protected override async Task CreateVersionTable()
    {
        if (!await VersionTableExists())
        {
            await base.CreateVersionTable();
            await CreateIdSequence(VersionTable);
            await CreateIdInsertTrigger(VersionTable);
        }
    }

    protected override string Parameterize(string sql) => sql.Replace("@", ":");
    protected override object Bool(bool source) => source ? '1' : '0';
    protected override object? Text(string? source) => source == null ? null : new OracleClobParameter(source);

    public override async Task<long> VersionTheDatabase(string newVersion)
    {
        var sql = (string)$@"
INSERT INTO {VersionTable}
(version, entry_date, modified_date, entered_by, status)
VALUES(:newVersion, :entryDate, :modifiedDate, :enteredBy, :status)
RETURNING id into :id
";
        var parameters = new
        {
            newVersion,
            entryDate = DateTime.UtcNow,
            modifiedDate = DateTime.UtcNow,
            enteredBy = ClaimsPrincipal.Current?.Identity?.Name ?? Environment.UserName,
            status = MigrationStatus.InProgress
        };
        var dynParams = new DynamicParameters(parameters);
        dynParams.Add(":id", dbType: DbType.Int64, direction: ParameterDirection.Output);

        await ActiveConnection.ExecuteAsync(
            sql,
            dynParams);

        var res = dynParams.Get<long>(":id");

        Logger.LogInformation(" Versioning {dbName} database with version {version}.", DatabaseName, newVersion);

        return res;
    }

    public override string DatabaseName
    {
        get
        {
            var tokens = Tokenize(Connection.ConnectionString);
            return GetValue(tokens, "Proxy User Id") ?? GetValue(tokens, "User ID") ?? base.DatabaseName;
        }
    }

    public override async Task ChangeVersionStatus(string status, long versionId)
    {
        var sql = (string)$@"
            UPDATE {VersionTable}
            SET status = :status
            WHERE id = :versionId";

        var parameters = new
        {
            status,
            versionId,
        };

        await Connection.ExecuteAsync(
            sql,
            parameters);
    }

    private static IDictionary<string, string?> Tokenize(string? connectionString)
    {
        var tokens = connectionString?.Split(";", RemoveEmptyEntries | TrimEntries) ?? Enumerable.Empty<string>();
        var keyPairs = tokens.Select(t => t.Split("=", TrimEntries));
        return keyPairs.ToDictionary(pair => pair[0], pair => (string?)pair[1]);
    }

    private static string? GetValue(IDictionary<string, string?> dictionary, string key) =>
        dictionary.TryGetValue(key, out string? value) ? value : null;

    private async Task CreateIdSequence(string table)
    {
        var sql = $"CREATE SEQUENCE {table}_seq";
        await ExecuteNonQuery(ActiveConnection, sql, Config?.CommandTimeout);
    }

    private async Task CreateIdInsertTrigger(string table)
    {
        var sql = $@"
CREATE OR REPLACE TRIGGER {table}_ins
BEFORE INSERT ON {table}
FOR EACH ROW
BEGIN
  SELECT {table}_seq.nextval INTO :new.id FROM dual;
END;";
        await ExecuteNonQuery(ActiveConnection, sql, Config?.CommandTimeout);
    }
    
    private class OracleClobParameter : SqlMapper.ICustomQueryParameter
    {
        private readonly string _value;

        public OracleClobParameter(string value)
        {
            _value = value;
        }

        public void AddParameter(IDbCommand command, string name)
        {
            var clob = new OracleClob(command.Connection as OracleConnection);

            // It should be Unicode Oracle throws an exception when
            // the length is not even.
            var bytes = Encoding.Unicode.GetBytes(_value);
            var length = Encoding.Unicode.GetByteCount(_value);

            int pos = 0;
            int chunkSize = 32768;

            while (pos < length)
            {
                chunkSize = chunkSize > (length - pos) ? length - pos : chunkSize;
                clob.Write(bytes, pos, chunkSize);
                pos += chunkSize;
            }

            var param = new OracleParameter(name, OracleDbType.Clob);
            param.Value = clob;

            command.Parameters.Add(param);
        }
    }
}
