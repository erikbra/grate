using System;
using System.Data;
using System.Data.Common;
using System.IO;
using System.Threading.Tasks;
using System.Transactions;
using Dapper;
using grate.Infrastructure;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Logging;

namespace grate.Migration
{
    public class SqliteDatabase : AnsiSqlDatabase
    {
        private static readonly SqliteSyntax Syntax = new();
            
        
        public SqliteDatabase(ILogger<SqliteDatabase> logger) 
            : base(logger, Syntax)
        { }

        public override bool SupportsDdlTransactions => false;
        public override bool SupportsSchemas => false;
        protected override DbConnection GetSqlConnection(string? connectionString) => new SqliteConnection(connectionString);

        protected override string ExistsSql(string tableSchema, string fullTableName) =>
            $@"
SELECT name FROM sqlite_master 
WHERE type ='table' AND 
name = '{fullTableName}';
";
        
        public override string DatabaseName => GetDatabaseName(Connection);

        /// <summary>
        /// Dropping a database in Sqlite is a bit different, it's just a matter of deleting a file on disk.
        /// </summary>
        /// <returns></returns>
        public override Task DropDatabase()
        {
            var db = _connection?.DataSource;
            
            if (File.Exists(db))
            {
                File.Delete(db);
            }
            
            return Task.CompletedTask;
        }

        // protected override async Task Open(DbConnection? conn)
        // {
        //     await base.Open(conn);
        //     if (conn != null)
        //     {
        //         var db = GetDatabaseName(conn);
        //         try
        //         {
        //             await conn.QueryAsync<string>(Syntax.AttachDatabase(db));
        //         }
        //         catch (SqliteException e) when(e.Message.Equals($"SQLite Error 1: 'database {db} is already in use'."))
        //         { }
        //     }
        // }

        public override Task<bool> DatabaseExists()
        {
            var file = Connection.DataSource;
            return Task.FromResult(File.Exists(file));
        }

        private static string GetDatabaseName(DbConnection conn) => Path.GetFileNameWithoutExtension(conn.DataSource);
    }
}
