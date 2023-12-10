﻿using System.Data.Common;
using grate.Infrastructure;
using grate.Infrastructure.Npgsql;
using Microsoft.Extensions.Logging;
using Npgsql;
using grate.Migration;
namespace grate.Npgsql.Migration;

public class PostgreSqlDatabase : AnsiSqlDatabase
{
    public PostgreSqlDatabase(ILogger<PostgreSqlDatabase> logger)
        : base(logger, new PostgreSqlSyntax())
    { }

    public override bool SupportsDdlTransactions => true;
    public override bool SupportsSchemas => true;
    protected override DbConnection GetSqlConnection(string? connectionString) => new NpgsqlConnection(connectionString);

    public override Task RestoreDatabase(string backupPath)
    {
        throw new System.NotImplementedException("Restoring a database from file is not currently supported for  Postgresql.");
    }

    public override bool SplitBatchStatements => true;

    public override IEnumerable<string> GetStatements(string sql)
        => ReflectionNpgsqlQueryParser.Split(sql);
}
