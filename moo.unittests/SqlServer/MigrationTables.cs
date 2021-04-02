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
    public class MigrationTables
    {
        private static string? AdminConnectionString() => $"Data Source=localhost,{MooTestContext.SqlServer.Port};Initial Catalog=master;User Id=sa;Password={MooTestContext.SqlServer.AdminPassword}";
        private static string? ConnectionString(string database) => $"Data Source=localhost,{MooTestContext.SqlServer.Port};Initial Catalog={database};User Id=sa;Password={MooTestContext.SqlServer.AdminPassword}";

        [TestCase("ScriptsRun")]
        [TestCase("ScriptsRunErrors")]
        [TestCase("Version")]
        public async Task Are_created_if_they_do_not_exist(string tableName)
        {
            var db = "MonoBonoJono";
            var fullTableName = "moo." + tableName;

            await using (var migrator = GetMigrator(db, true))
            {
                await migrator.Migrate();
            }

            IEnumerable<string> scripts;
            string sql = $"SELECT modified_date FROM {fullTableName}";
            
            await using (var conn = new SqlConnection(ConnectionString(db)))
            {
                scripts = await conn.QueryAsync<string>(sql);
            }
            scripts.Should().NotBeNull();
        }


        private MooMigrator GetMigrator(string databaseName, bool createDatabase)
        {
            var connectionString = ConnectionString(databaseName);

            var dbLogger = new NullLogger<SqlServerDatabase>();
            var factory = Substitute.For<IFactory>();
            factory.GetService<DatabaseType, IDatabase>(DatabaseType.sqlserver)
                .Returns(new SqlServerDatabase(dbLogger));

            var dbMigrator = new DbMigrator(factory, new NullLogger<DbMigrator>(), new HashGenerator());
            var migrator = new MooMigrator(new NullLogger<MooMigrator>(), dbMigrator);

            var config = new MooConfiguration()
            {
                CreateDatabase = createDatabase, 
                ConnectionString = connectionString,
                AdminConnectionString = AdminConnectionString(),
                KnownFolders = KnownFolders.In(new DirectoryInfo(@"C:\tmp\sql"))
            };
            dbMigrator.ApplyConfig(config);

            return migrator;
        }
        
    }
}
