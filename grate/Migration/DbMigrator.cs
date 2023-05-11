using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Transactions;
using grate.Configuration;
using grate.Infrastructure;
using Microsoft.Extensions.Logging;

namespace grate.Migration;

public class DbMigrator : IDbMigrator
{
    private readonly ILogger<DbMigrator> _logger;
    private readonly IHashGenerator _hashGenerator;

    public DbMigrator(IFactory factory, ILogger<DbMigrator> logger, IHashGenerator hashGenerator, GrateConfiguration? configuration = null)
    {
        _logger = logger;
        _hashGenerator = hashGenerator;
        Configuration = configuration ?? throw new ArgumentException("No configuration passed to DbMigrator.  Container setup error?", nameof(configuration));
        Database = factory.GetService<DatabaseType, IDatabase>(Configuration.DatabaseType);
        StatementSplitter = new StatementSplitter(Database.StatementSeparatorRegex);
    }

    public Task InitializeConnections() => Database.InitializeConnections(Configuration);

    public IDatabase Database { get; set; }
    private StatementSplitter StatementSplitter { get; }

    public async Task<bool> DatabaseExists() => await Database.DatabaseExists();

    public async Task<bool> CreateDatabase()
    {
        if (Configuration.CreateDatabase)
        {
            await Database.CreateDatabase();
            return true;
        }
        return false;
    }

    public Task DropDatabase()
    {
        Debug.Assert(Configuration.Drop, "How did we get into a drop command when not configured to do so?");
        return Database.DropDatabase();
    }

    public Task OpenConnection() => Database.OpenConnection();
    public Task CloseConnection() => Database.CloseConnection();

    public void SetDefaultConnectionActive() => Database.SetDefaultConnectionActive();
    public Task<IDisposable> OpenNewActiveConnection() => Database.OpenNewActiveConnection();
    public Task OpenActiveConnection() => Database.OpenActiveConnection();

    public Task RunSupportTasks() => Database.RunSupportTasks();
    public Task<string> GetCurrentVersion() => Database.GetCurrentVersion();
    public Task<long> VersionTheDatabase(string newVersion)
    {
        if (Configuration.DryRun)
        {
            _logger.LogDebug("Skipping writing database version row due to --dryrun");
            return Task.FromResult(-1L);
        }
        else
        {
            return Database.VersionTheDatabase(newVersion);
        }
    }

    public Task OpenAdminConnection() => Database.OpenAdminConnection();
    public Task CloseAdminConnection() => Database.CloseAdminConnection();

    public GrateConfiguration Configuration { get; set; }

    public async Task<bool> RunSql(string sql, string scriptName, MigrationType migrationType, long versionId,
        GrateEnvironment? environment,
        ConnectionType connectionType, TransactionHandling transactionHandling)
    {
        var theSqlWasRun = false;

        async Task<bool> LogAndRunSql()
        {
            _logger.LogInformation("  Running '{ScriptName}'.", scriptName);

            if (Configuration.DryRun)
            {
                return false;
            }
            else
            {
                await RunTheActualSql(sql, scriptName, migrationType, versionId, connectionType, transactionHandling);
                return true;
            }
        }

        if (!InCorrectEnvironment(scriptName, environment))
        {
            return false;
        }

        // the scripts are stored in the database with tokens replaced. This means we need to do any token work _before_ we start checking hashes etc
        if (TokenReplacementEnabled)
        {
            sql = ReplaceTokensIn(sql);
        }

        if (Configuration.Baseline)
        {
            await RecordScriptInScriptsRunTable(scriptName, sql, migrationType, versionId, transactionHandling);
            return false;
        }

        if (await ThisScriptIsAlreadyRun(scriptName, transactionHandling) && !IsEverytimeScript(scriptName, migrationType))
        {
            if (AnyTimeScriptForcedToRun(migrationType, Configuration) || await ScriptHasChanged(scriptName, sql))
            {
                var changeHandling = DetermineChangeHandling(Configuration);

                switch (migrationType)
                {
                    case MigrationType.Once when changeHandling == ChangedScriptHandling.Error:
                        await OneTimeScriptChanged(sql, scriptName, versionId);
                        break;

                    case MigrationType.Once when changeHandling == ChangedScriptHandling.WarnAndRun:
                        LogScriptChangedWarning(scriptName);
                        _logger.LogDebug("Running script anyway due to WarnOnOneTimeScriptChanges option being set.");
                        theSqlWasRun = await LogAndRunSql();
                        break;

                    case MigrationType.Once when changeHandling == ChangedScriptHandling.WarnAndIgnore:
                        LogScriptChangedWarning(scriptName);
                        _logger.LogDebug("Ignoring script but marking as run due to WarnAndIgnoreOnOneTimeScriptChanges option being set.");
                        await RecordScriptInScriptsRunTable(scriptName, sql, migrationType, versionId, transactionHandling);
                        break;

                    case MigrationType.AnyTime:
                    case MigrationType.EveryTime:
                        theSqlWasRun = await LogAndRunSql();
                        break;
                }
            }
            else
            {
                _logger.LogDebug(" Skipped {ScriptName} - {Reason}.", scriptName, "No changes were found to run");
            }
        }
        else
        {
            theSqlWasRun = await LogAndRunSql();
        }

        return theSqlWasRun;
    }
    
    public async Task<bool> RunSqlWithoutLogging(
        string sql, 
        string scriptName,
        GrateEnvironment? environment,
        ConnectionType connectionType, 
        TransactionHandling transactionHandling)
    {
        async Task<bool> PrintLogAndRunSql()
        {
            _logger.LogInformation("  Running '{ScriptName}'.", scriptName);

            if (Configuration.DryRun)
            {
                return false;
            }
            else
            {
                await RunTheActualSqlWithoutLogging(sql, scriptName, connectionType, transactionHandling);
                return true;
            }
        }

        if (!InCorrectEnvironment(scriptName, environment))
        {
            return false;
        }

        if (TokenReplacementEnabled)
        {
            sql = ReplaceTokensIn(sql);
        }
        
        return await PrintLogAndRunSql();;
    }


    public async Task RestoreDatabase(string backupPath)
    {
        await Database.RestoreDatabase(backupPath);
    }

    /// <summary>
    /// Returns true if we're looking at an AnyTime folder, but the RunAllAnyTimeScripts flag is forced on
    /// </summary>
    private static bool AnyTimeScriptForcedToRun(MigrationType migrationType, GrateConfiguration configuration)
    {
        return migrationType == MigrationType.AnyTime && configuration.RunAllAnyTimeScripts;
    }

    private static ChangedScriptHandling DetermineChangeHandling(GrateConfiguration configuration)
    {
        if (configuration.WarnOnOneTimeScriptChanges) return ChangedScriptHandling.WarnAndRun;
        if (configuration.WarnAndIgnoreOnOneTimeScriptChanges) return ChangedScriptHandling.WarnAndIgnore;
        return ChangedScriptHandling.Error;
    }

    private enum ChangedScriptHandling
    {
        Error,
        WarnAndRun,
        WarnAndIgnore
    }

    private static bool InCorrectEnvironment(string scriptName, GrateEnvironment? env)
    {
        return !GrateEnvironment.IsEnvironmentFile(scriptName) || // run non-env files all the time
               (env?.ShouldRun(scriptName) ?? false); // #101 - don't run .env scripts if no env specified.
    }

    private static bool IsEverytimeScript(string scriptName, MigrationType migrationType) =>
        migrationType == MigrationType.EveryTime ||
        FileName(scriptName).StartsWith("everytime.", StringComparison.InvariantCultureIgnoreCase) ||
        FileName(scriptName).Contains(".everytime.", StringComparison.InvariantCultureIgnoreCase);


    private async Task<bool> ScriptHasChanged(string scriptName, string sql)
    {
        var currentHash = await Database.GetCurrentHash(scriptName);
        var newHash = GetHash(sql);

        return currentHash != newHash;
    }

    private static string FileName(string path) => new FileInfo(path).Name;

    private string GetHash(string sql)
    {
        return _hashGenerator.Hash(sql);
    }

    private async Task<bool> ThisScriptIsAlreadyRun(string scriptName, TransactionHandling transactionHandling) => await Database.HasRun(scriptName, transactionHandling);


    /// <summary>
    /// Lazily initialised only if needed.
    /// </summary>
    private Dictionary<string, string?>? _tokens;
    private string ReplaceTokensIn(string sql)
    {
        _tokens ??= new TokenProvider(Configuration, Database).GetTokens();
        return TokenReplacer.ReplaceTokens(_tokens, sql);
    }

    private bool TokenReplacementEnabled => !Configuration.DisableTokenReplacement;


    /// <summary>
    /// Actually, for real, executes the sql against the database
    /// </summary>
    /// <param name="sql"></param>
    /// <param name="scriptName"></param>
    /// <param name="migrationType"></param>
    /// <param name="versionId"></param>
    /// <param name="connectionType"></param>
    /// <param name="transactionHandling"></param>
    /// <returns></returns>
    private async Task RunTheActualSql(string sql,
        string scriptName,
        MigrationType migrationType,
        long versionId,
        ConnectionType connectionType, 
        TransactionHandling transactionHandling)
    {
        foreach (var statement in GetStatements(sql))
        {
            try
            {
                await Database.RunSql(statement, connectionType, transactionHandling);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error running script \"{ScriptName}\": {ErrorMessage}", scriptName, ex.Message);

                if (Transaction.Current is not null) {
                    Database.Rollback();
                }

                await Database.CloseConnection();
                Transaction.Current?.Dispose();

                using var s = new TransactionScope(TransactionScopeOption.Suppress, TransactionScopeAsyncFlowOption.Enabled);
                using (await OpenNewActiveConnection())
                {
                    await RecordScriptInScriptsRunErrorsTable(scriptName, sql, statement, ex.Message, versionId);
                }
                s.Complete();
                
                SetDefaultConnectionActive();
                throw;
            }
        }

        await RecordScriptInScriptsRunTable(scriptName, sql, migrationType, versionId, transactionHandling);
    }
    
    private async Task RunTheActualSqlWithoutLogging(
        string sql,
        string scriptName,
        ConnectionType connectionType, 
        TransactionHandling transactionHandling)
    {
        foreach (var statement in GetStatements(sql))
        {
            try
            {
                await Database.RunSql(statement, connectionType, transactionHandling);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error running script \"{ScriptName}\": {ErrorMessage}", scriptName, ex.Message);

                if (Transaction.Current is not null) {
                    Database.Rollback();
                }

                await Database.CloseConnection();
                Transaction.Current?.Dispose();
                throw;
            }
        }
    }


    private IEnumerable<string> GetStatements(string sql) => StatementSplitter.Split(sql);

    private void LogScriptChangedWarning(string scriptName)
    {
        _logger.LogWarning("{ScriptName} is a one time script that has changed since it was run.", scriptName);
    }

    /// <summary>
    /// Returns whether to execute the script even though it has changed.  
    /// Throws an exception if this script change is a failure scenario.
    /// </summary>
    /// <param name="sql"></param>
    /// <param name="scriptName"></param>
    /// <param name="versionId"></param>
    /// <returns></returns>
    /// <exception cref="Migration.OneTimeScriptChanged"></exception>
    private async Task OneTimeScriptChanged(string sql, string scriptName, long versionId)
    {
        Database.Rollback();
        await Database.CloseConnection();
        Transaction.Current?.Dispose();

        string errorMessage =
            $"{scriptName} has changed since the last time it was run. By default this is not allowed - scripts that run once should never change. To change this behavior to a warning, please set warnOnOneTimeScriptChanges to true and run again. Stopping execution.";

        using (var s = new TransactionScope(TransactionScopeOption.Suppress, TransactionScopeAsyncFlowOption.Enabled))
        {
            using (await OpenNewActiveConnection())
            {
                await RecordScriptInScriptsRunErrorsTable(scriptName, sql, sql, errorMessage, versionId);
            }
            s.Complete();
        }
        SetDefaultConnectionActive();
        
        throw new OneTimeScriptChanged(errorMessage);
    }

    private Task RecordScriptInScriptsRunTable(string scriptName, string sql, MigrationType migrationType,
        long versionId, TransactionHandling transactionHandling)
    {
        var hash = _hashGenerator.Hash(sql);
        var sqlToStore = Configuration.DoNotStoreScriptsRunText ? null : sql;

        if (Configuration.DryRun)
        {
            _logger.LogTrace("Skipping recording {ScriptName} script ran on {ServerName} - {DatabaseName}, --dryrun prevents sql writes", scriptName, Database.ServerName, Database.DatabaseName);
            return Task.CompletedTask;
        }
        else
        {
            _logger.LogTrace("Recording {ScriptName} script ran on {ServerName} - {DatabaseName}.", scriptName, Database.ServerName, Database.DatabaseName);
            return Database.InsertScriptRun(scriptName, sqlToStore, hash, migrationType == MigrationType.Once, versionId, transactionHandling);
        }
    }

    private async Task RecordScriptInScriptsRunErrorsTable(string scriptName, string sql, string errorSql, string errorMessage, long versionId)
    {
        var sqlToStore = Configuration.DoNotStoreScriptsRunText ? null : sql;
        await Database.InsertScriptRunError(scriptName, sqlToStore, errorSql, errorMessage, versionId);
        await Database.ChangeVersionStatus(MigrationStatus.Error, versionId);
    }

    public async ValueTask DisposeAsync()
    {
        // Allow the dbase to clean itself up
        await Database.DisposeAsync();
        GC.SuppressFinalize(this);
    }
}
