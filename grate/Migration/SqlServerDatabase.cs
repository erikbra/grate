using System;
using System.Data.Common;
using System.Threading.Tasks;
using System.Transactions;
using grate.Infrastructure;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;

namespace grate.Migration
{
    public class SqlServerDatabase : AnsiSqlDatabase
    {
        public SqlServerDatabase(ILogger<SqlServerDatabase> logger) 
            : base(logger, new SqlServerSyntax())
        { }

        public override bool SupportsDdlTransactions => true;
        public override bool SupportsSchemas => true;
        protected override DbConnection GetSqlConnection(string? connectionString) => new SqlConnection(connectionString);

        public override async Task RestoreDatabase(string backupPath)
        {
            try
            {
                await OpenAdminConnection();
                Logger.LogInformation("Restoring {dbName} database on {server} server from path {path}.", DatabaseName, ServerName, backupPath);
                using var s = new TransactionScope(TransactionScopeOption.Suppress, TransactionScopeAsyncFlowOption.Enabled);
                var cmd = AdminConnection.CreateCommand();
                cmd.CommandText =
                        $@"USE master
                        ALTER DATABASE [{DatabaseName}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
                        RESTORE DATABASE [{DatabaseName}]
                        FROM DISK = N'{backupPath}'
                        WITH NOUNLOAD
                        , STATS = 10
                        , RECOVERY
                        , REPLACE;

                        ALTER DATABASE [{DatabaseName}] SET MULTI_USER;";
                await cmd.ExecuteNonQueryAsync();
                s.Complete();
            }
            catch (Exception ex)
            {
                Logger.LogDebug(ex, "Got error: " + ex.Message);
                throw;
            }
            finally
            {
                await CloseAdminConnection();
            }

            await WaitUntilDatabaseIsReady();

            Logger.LogInformation("Database {dbName} successfully restored from path {path}.", DatabaseName, backupPath);
        }
    }
}
