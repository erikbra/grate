using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Transactions;
using Dapper;
using FluentAssertions;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging.Abstractions;
using moo.Configuration;
using moo.Infrastructure;
using moo.Migration;
using NSubstitute;
using NUnit.Framework;

namespace moo.unittests.SqlServer
{
    [TestFixture]
    public class Database
    {
        private static string? AdminConnectionString() => $"Data Source=localhost,{MooTestContext.SqlServer.Port};Initial Catalog=master;User Id=sa;Password={MooTestContext.SqlServer.AdminPassword}";
        private static string? ConnectionString(string database) => $"Data Source=localhost,{MooTestContext.SqlServer.Port};Initial Catalog={database};User Id=sa;Password={MooTestContext.SqlServer.AdminPassword}";

        [Test]
        public async Task Is_created_if_confed_and_it_does_not_exist()
        {
            var db = "NewDatabase";
            
            var migrator = GetMigrator(GetConfiguration(db, true));
            await migrator.Migrate();

            IEnumerable<string> databases;
            const string? sql = "SELECT name FROM sys.databases";
            
            using (new TransactionScope(TransactionScopeOption.Suppress, TransactionScopeAsyncFlowOption.Enabled))
            {
                await using (var conn = new SqlConnection(AdminConnectionString()))
                {
                    databases = await conn.QueryAsync<string>(sql);
                }
            }
            databases.Should().Contain(db);
        }
        
        [Test]
        public async Task Is_not_created_if_not_confed()
        {
            var db = "SomeOtherdatabase";
            
            var migrator = GetMigrator(GetConfiguration(db, false));
            
            // The migration should throw an error, as the database does not exist.
            Assert.ThrowsAsync<SqlException>(() => migrator.Migrate());

            // Ensure that the database was in fact not created 
            IEnumerable<string> databases;
            const string? sql = "SELECT name FROM sys.databases";
            using (new TransactionScope(TransactionScopeOption.Suppress, TransactionScopeAsyncFlowOption.Enabled))
            {
                await using (var conn = new SqlConnection(AdminConnectionString()))
                {
                    databases = await conn.QueryAsync<string>(sql);
                }
            }
            databases.Should().NotContain(db);
        }
        
        [Test]
        public async Task Does_not_error_if_confed_to_create_but_already_exists()
        {
            const string? selectDatabasesSql = "SELECT name FROM sys.databases";
            
            var db = "daataa";
            
            // Create the database manually before running the migration
            using (new TransactionScope(TransactionScopeOption.Suppress, TransactionScopeAsyncFlowOption.Enabled))
            {
                await using (var conn = new SqlConnection(AdminConnectionString()))
                {
                    conn.Open();
                    await using var cmd = conn.CreateCommand();
                    cmd.CommandText = $"CREATE DATABASE {db}";
                    await cmd.ExecuteNonQueryAsync();
                }
            }
            
            // Check that the database has been created
            IEnumerable<string>? databasesBeforeMigration = null;
            using (new TransactionScope(TransactionScopeOption.Suppress, TransactionScopeAsyncFlowOption.Enabled))
            {
                for (var i = 0; i < 5; i++)
                {
                    try
                    {
                        await using (var conn = new SqlConnection(AdminConnectionString()))
                        {
                            databasesBeforeMigration = await conn.QueryAsync<string>(selectDatabasesSql);
                        }
                        break;
                    }
                    catch (SqlException) { }
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
            const string? selectDatabasesSql = "SELECT name FROM sys.databases";
            
            var db = "datadatadatabase";
            
            // Create the database manually before running the migration
            using (new TransactionScope(TransactionScopeOption.Suppress, TransactionScopeAsyncFlowOption.Enabled))
            {
                for (var i = 0; i < 5; i++)
                {
                    try
                    {
                        await using (var conn = new SqlConnection(AdminConnectionString()))
                        {
                            conn.Open();
                            await using var cmd = conn.CreateCommand();
                            cmd.CommandText = $"CREATE DATABASE {db}";
                            await cmd.ExecuteNonQueryAsync();
                        }
                        break;
                    }
                    catch (SqlException) { }
                }
            }
            
            // Check that the database has been created
            IEnumerable<string> databasesBeforeMigration;
            using (new TransactionScope(TransactionScopeOption.Suppress, TransactionScopeAsyncFlowOption.Enabled))
            {
                await using (var conn = new SqlConnection(AdminConnectionString()))
                {
                    databasesBeforeMigration = await conn.QueryAsync<string>(selectDatabasesSql);
                }
            }
            databasesBeforeMigration.Should().Contain(db);
            
            // Change the admin connection string to rubbish and run the migration
            var migrator = GetMigrator(GetConfiguration(db, true, "Invalid stuff"));
            
            // There should be no errors running the migration
            Assert.DoesNotThrowAsync(() => migrator.Migrate());
        }


        private MooMigrator GetMigrator(MooConfiguration config)
        {
            var dbLogger = new NullLogger<SqlServerDatabase>();
            var factory = Substitute.For<IFactory>();
            factory.GetService<DatabaseType, IDatabase>(DatabaseType.sqlserver)
                .Returns(new SqlServerDatabase(dbLogger));

            var dbMigrator = new DbMigrator(factory, new NullLogger<DbMigrator>(), new HashGenerator());
            var migrator = new MooMigrator(new NullLogger<MooMigrator>(), dbMigrator);
            
            dbMigrator.ApplyConfig(config);
            return migrator;
        }

        private static MooConfiguration GetConfiguration(string databaseName, bool createDatabase, string? adminConnectionString = null)
        {
            return new MooConfiguration()
            {
                CreateDatabase = createDatabase, 
                ConnectionString = ConnectionString(databaseName),
                AdminConnectionString = adminConnectionString ?? AdminConnectionString(),
                KnownFolders = KnownFolders.In(new DirectoryInfo(@"C:\tmp\sql"))
            };
        }
    }
}
