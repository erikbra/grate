using System.Collections.Generic;
using System.Data.Common;
using System.IO;
using System.Threading.Tasks;
using System.Transactions;
using Dapper;
using FluentAssertions;
using grate.Configuration;
using grate.Migration;
using grate.unittests.TestInfrastructure;
using NUnit.Framework;

namespace grate.unittests.Generic
{
    [TestFixture]
    public abstract class GenericDatabase
    {
        protected abstract IGrateTestContext Context { get; }

        [Test]
        public async Task Is_created_if_confed_and_it_does_not_exist()
        {
            var db = "NewDatabase";
            
            var migrator = GetMigrator(GetConfiguration(db, true));
            await migrator.Migrate();

            IEnumerable<string> databases;
            string sql = Context.Sql.SelectAllDatabases;
            
            using (new TransactionScope(TransactionScopeOption.Suppress, TransactionScopeAsyncFlowOption.Enabled))
            {
                await using var conn = Context.CreateAdminDbConnection();
                databases = await conn.QueryAsync<string>(sql);
            }
            databases.Should().Contain(db);
        }
     
        
        [Test]
        public async Task Is_not_created_if_not_confed()
        {
            var db = "SomeOtherdatabase";
            
            var migrator = GetMigrator(GetConfiguration(db, false));
            
            // The migration should throw an error, as the database does not exist.
            Assert.ThrowsAsync(Context.DbExceptionType, () => migrator.Migrate());

            // Ensure that the database was in fact not created 
            IEnumerable<string> databases;
            string? sql = Context.Sql.SelectAllDatabases;
            using (new TransactionScope(TransactionScopeOption.Suppress, TransactionScopeAsyncFlowOption.Enabled))
            {
                await using var conn = Context.CreateAdminDbConnection();
                databases = await conn.QueryAsync<string>(sql);
            }
            databases.Should().NotContain(db);
        }
        
        [Test]
        public async Task Does_not_error_if_confed_to_create_but_already_exists()
        {
            string? selectDatabasesSql =  Context.Sql.SelectAllDatabases;
            
            var db = "daataa";
            
            // Create the database manually before running the migration
            using (new TransactionScope(TransactionScopeOption.Suppress, TransactionScopeAsyncFlowOption.Enabled))
            {
                await using var conn = Context.CreateAdminDbConnection();
                conn.Open();
                await using var cmd = conn.CreateCommand();
                cmd.CommandText = $"CREATE DATABASE {db}";
                await cmd.ExecuteNonQueryAsync();
            }
            
            // Check that the database has been created
            IEnumerable<string>? databasesBeforeMigration = null;
            using (new TransactionScope(TransactionScopeOption.Suppress, TransactionScopeAsyncFlowOption.Enabled))
            {
                for (var i = 0; i < 5; i++)
                {
                    try
                    {
                        await using var conn = Context.CreateAdminDbConnection();
                        databasesBeforeMigration = await conn.QueryAsync<string>(selectDatabasesSql);
                        break;
                    }
                    catch (DbException) { }
                }
            }
            databasesBeforeMigration.Should().Contain(db);
            
            var migrator = GetMigrator(GetConfiguration(db, true));
            
            // There should be no errors running the migration
            Assert.DoesNotThrowAsync(() => migrator.Migrate());
        }
        
        [Test]
        public async Task Does_not_need_admin_connection_if_database_already_exists()
        {
            string? selectDatabasesSql = Context.Sql.SelectAllDatabases;
            
            var db = "datadatadatabase";
            
            // Create the database manually before running the migration
            using (new TransactionScope(TransactionScopeOption.Suppress, TransactionScopeAsyncFlowOption.Enabled))
            {
                for (var i = 0; i < 5; i++)
                {
                    try
                    {
                        await using var conn = Context.CreateAdminDbConnection();
                        conn.Open();
                        await using var cmd = conn.CreateCommand();
                        cmd.CommandText = $"CREATE DATABASE {db}";
                        await cmd.ExecuteNonQueryAsync();
                        break;
                    }
                    catch (DbException) { }
                }
            }
            
            // Check that the database has been created
            IEnumerable<string> databasesBeforeMigration;
            using (new TransactionScope(TransactionScopeOption.Suppress, TransactionScopeAsyncFlowOption.Enabled))
            {
                await using var conn = Context.CreateAdminDbConnection();
                databasesBeforeMigration = await conn.QueryAsync<string>(selectDatabasesSql);
            }
            databasesBeforeMigration.Should().Contain(db);
            
            // Change the admin connection string to rubbish and run the migration
            var migrator = GetMigrator(GetConfiguration(db, true, "Invalid stuff"));
            
            // There should be no errors running the migration
            Assert.DoesNotThrowAsync(() => migrator.Migrate());
        }


        private GrateMigrator GetMigrator(GrateConfiguration config) => Context.GetMigrator(config);
    

        private GrateConfiguration GetConfiguration(string databaseName, bool createDatabase, string? adminConnectionString = null)
        {
            return new()
            {
                CreateDatabase = createDatabase, 
                ConnectionString = Context.ConnectionString(databaseName),
                AdminConnectionString = adminConnectionString ?? Context.AdminConnectionString,
                KnownFolders = KnownFolders.In(new DirectoryInfo(@"C:\tmp\sql")),
                NonInteractive = true,
                DatabaseType = Context.DatabaseType
            };
        }
    }
}
