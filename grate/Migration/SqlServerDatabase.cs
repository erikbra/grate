using System;
using System.Data.Common;
using System.Threading.Tasks;
using System.Transactions;
using grate.Configuration;
using grate.Infrastructure;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;

namespace grate.Migration;

public class SqlServerDatabase : AnsiSqlDatabase
{
    public SqlServerDatabase(ILogger<SqlServerDatabase> logger) 
        : base(logger, new SqlServerSyntax())
    { }

    public override bool SupportsDdlTransactions => true;
    protected override bool SupportsSchemas => true;
    protected override DbConnection GetSqlConnection(string? connectionString)
    {
        // Define the retry logic parameters
        var options = new SqlRetryLogicOption()
        {
            // Tries 5 times before throwing an exception
            NumberOfTries = 15,
            
            // Preferred gap time to delay before retry
            DeltaTime = TimeSpan.FromMilliseconds(250),
            
            // Minimum gap time for each delay time before retry
            MinTimeInterval = TimeSpan.FromMilliseconds(150),
            
            // Maximum gap time for each delay time before retry
            MaxTimeInterval = TimeSpan.FromSeconds(5),
            
            TransientErrors = new []{ 1205, 3314, 3316, 3434, 3449 }
        };
        
        // Create a retry logic provider
        SqlRetryLogicBaseProvider provider = SqlConfigurableRetryFactory.CreateExponentialRetryProvider(options);
        
        var conn = new SqlConnection(connectionString);
        conn.AccessToken = AccessToken;
        
        // Set the retry logic provider on the connection instance
        conn.RetryLogicProvider = provider;

        return conn;
    }
    protected string? AccessToken { get; private set; }

    public override Task InitializeConnections(GrateConfiguration configuration)
    {
        AccessToken = configuration.AccessToken;
        return base.InitializeConnections(configuration);
    }

    public override async Task RestoreDatabase(string backupPath)
    {
        try
        {
            await OpenAdminConnection();
            Logger.LogInformation("Restoring {DbName} database on {Server} server from path {Path}.", DatabaseName, ServerName, backupPath);
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
            Logger.LogDebug(ex, "Got error: {ErrorMessage}", ex.Message);
            throw;
        }
        finally
        {
            await CloseAdminConnection();
        }

        await WaitUntilDatabaseIsReady();

        Logger.LogInformation("Database {DbName} successfully restored from path {Path}.", DatabaseName, backupPath);
    }

    
    protected override string HasRunSql =>
        $@"
SELECT 1 FROM  {ScriptsRunTable} WITH (NOLOCK)
WHERE script_name = @scriptName";
}
