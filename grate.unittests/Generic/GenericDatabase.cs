using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using Dapper;
using FluentAssertions;
using grate.Configuration;
using grate.Migration;
using grate.unittests.TestInfrastructure;
using NUnit.Framework;

namespace grate.unittests.Generic;

[TestFixture]
public abstract class GenericDatabase
{
    protected abstract IGrateTestContext Context { get; }

    [Test]
    public async Task Is_created_if_confed_and_it_does_not_exist()
    {
        var db = "NEWDATABASE";
            
        await using var migrator = GetMigrator(GetConfiguration(db, true));
        await migrator.Migrate();

        IEnumerable<string> databases = await GetDatabases();
        databases.Should().Contain(db);
    }

    [Test]
    public async Task Is_not_created_if_not_confed()
    {
        var db = "SOMEOTHERDATABASE";
            
        IEnumerable<string> databasesBeforeMigration = await GetDatabases();
        databasesBeforeMigration.Should().NotContain(db);
            
        await using var migrator = GetMigrator(GetConfiguration(db, false));
            
        // The migration should throw an error, as the database does not exist.
        if (ThrowOnMissingDatabase)
        {
            Assert.ThrowsAsync(Context.DbExceptionType, () => migrator.Migrate());
        }

        // Ensure that the database was in fact not created 
        IEnumerable<string> databases = await GetDatabases();
        databases.Should().NotContain(db);
    }
        
    [Test]
    public async Task Does_not_error_if_confed_to_create_but_already_exists()
    {
        var db = "DAATAA";
            
        // Create the database manually before running the migration
        await CreateDatabase(db);
            
        // Check that the database has been created
        IEnumerable<string> databasesBeforeMigration = await GetDatabases();
        databasesBeforeMigration.Should().Contain(db);
            
        await using var migrator = GetMigrator(GetConfiguration(db, true));
            
        // There should be no errors running the migration
        Assert.DoesNotThrowAsync(() => migrator.Migrate());
    }
        
    [Test]
    public async Task Does_not_need_admin_connection_if_database_already_exists()
    {
        var db = "DATADATADATABASE";
            
        // Create the database manually before running the migration
        await CreateDatabase(db);
            
        // Check that the database has been created
        IEnumerable<string> databasesBeforeMigration = await GetDatabases();
        databasesBeforeMigration.Should().Contain(db);
            
        // Change the admin connection string to rubbish and run the migration
        await using var migrator = GetMigrator(GetConfiguration(db, true, "Invalid stuff"));
            
        // There should be no errors running the migration
        Assert.DoesNotThrowAsync(() => migrator.Migrate());
    }

    [Test]
    public async Task Does_not_needlessly_apply_case_sensitive_database_name_checks_Issue_167()
    {
        // There's a bug where if the database name specified by the user differs from the actual database only by case then
        // Grate currently attempts to create the database again, only for it to fail on the DBMS (Sql Server bug only).

        var db = "CASEDATABASE";
        await CreateDatabase(db);

        // Check that the database has been created
        IEnumerable<string> databasesBeforeMigration = await GetDatabases();
        databasesBeforeMigration.Should().Contain(db);

        await using var migrator = GetMigrator(GetConfiguration(db.ToLower(), true)); // ToLower is important here, this reproduces the bug in #167
        // There should be no errors running the migration
        Assert.DoesNotThrowAsync(() => migrator.Migrate());
    }

    protected virtual async Task CreateDatabase(string db)
    {
        using (new TransactionScope(TransactionScopeOption.Suppress, TransactionScopeAsyncFlowOption.Enabled))
        {
            for (var i = 0; i < 5; i++)
            {
                try
                {
                    await using var conn = Context.CreateAdminDbConnection();
                    await conn.OpenAsync();
                    await using var cmd = conn.CreateCommand();
                    cmd.CommandText = Context.Syntax.CreateDatabase(db, TestConfig.Password(Context.ConnectionString(db)));
                    await cmd.ExecuteNonQueryAsync();
                    break;
                }
                catch (DbException) { }
            }
        }
    }
        
    protected virtual async Task<IEnumerable<string>> GetDatabases()
    {
        IEnumerable<string> databases =Enumerable.Empty<string>();
        string sql = Context.Syntax.ListDatabases;

        using (new TransactionScope(TransactionScopeOption.Suppress, TransactionScopeAsyncFlowOption.Enabled))
        {
            for (var i = 0; i < 5; i++)
            {
                try
                {
                    await using var conn = Context.CreateAdminDbConnection();
                    databases = await conn.QueryAsync<string>(sql);
                    break;
                }
                catch (DbException) { }
            }
        }
        return databases;
    }

    protected virtual bool ThrowOnMissingDatabase => true;


    private GrateMigrator GetMigrator(GrateConfiguration config) => Context.GetMigrator(config);
    

    private GrateConfiguration GetConfiguration(string databaseName, bool createDatabase, string? adminConnectionString = null)
    {
        return new()
        {
            CreateDatabase = createDatabase, 
            ConnectionString = Context.ConnectionString(databaseName),
            AdminConnectionString = adminConnectionString ?? Context.AdminConnectionString,
            KnownFolders = KnownFolders.In(TestConfig.CreateRandomTempDirectory()),
            NonInteractive = true,
            DatabaseType = Context.DatabaseType
        };
    }
}
