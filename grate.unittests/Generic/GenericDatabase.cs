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
    public async Task Adds_status_column_if_it_does_not_exist()
    {
        var db = "NEWDATABASE";

        // Setup the database (with status column)
        await using var migrator = GetMigrator(GetConfiguration(db, true));
        await migrator.Migrate();

        // Delete the status column (to simulate a database without it)
        string deleteSql = $"ALTER TABLE {Context.Syntax.TableWithSchema("grate", "Version")} DROP COLUMN status";
        string findAllColumnsSql = $"SELECT column_name FROM information_schema.columns where table_name = 'version'";

        var currentColumns = new List<string>();
        await using (var conn = Context.GetDbConnection(Context.ConnectionString(db)))
        {
            await conn.ExecuteAsync(deleteSql);
            currentColumns = (await conn.QueryAsync<string>(findAllColumnsSql)).ToList();
        }

        // Make sure it's gone
        currentColumns.Should().NotContain("status");

        // Migrate again so the column gets added back
        await migrator.Migrate();
        currentColumns = new List<string>();
        await using (var conn = Context.GetDbConnection(Context.ConnectionString(db)))
        {
            currentColumns = (await conn.QueryAsync<string>(findAllColumnsSql)).ToList();
        }

        currentColumns.Should().Contain("status");
    }

    [Test]
    public async Task Does_not_error_if_status_column_tries_to_create_twice()
    {
        var db = "NEWDATABASE";

        // Setup the database (with status column)
        await using var migrator = GetMigrator(GetConfiguration(db, true));
        await migrator.Migrate();

        string findAllColumnsSql = $"SELECT column_name FROM information_schema.columns where table_name = 'version'";

        var currentColumns = new List<string>();
        await using (var conn = Context.GetDbConnection(Context.ConnectionString(db)))
        {
            currentColumns = (await conn.QueryAsync<string>(findAllColumnsSql)).ToList();
        }

        // Make sure the 'status' column is there
        currentColumns.Should().Contain("status");

        // Migrate again to make sure this does not crash.
        await migrator.Migrate();
        currentColumns = new List<string>();
        await using (var conn = Context.GetDbConnection(Context.ConnectionString(db)))
        {
            currentColumns = (await conn.QueryAsync<string>(findAllColumnsSql)).ToList();
        }

        currentColumns.Should().Contain("status");
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
