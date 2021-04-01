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

        [TestCase("ScriptsRun")]
        [TestCase("ScriptsRunErrors")]
        [TestCase("Version")]
        public async Task Are_created_if_they_do_not_exist(string tableName)
        {
            var db = "MonoBonoJono";
            var fullTableName = "moo." + tableName;
            
            var migrator = GetMigrator(db, true);
            await migrator.Migrate();

            IEnumerable<string> scripts;
            string? sql = $"SELECT script_name FROM {fullTableName}";
            
            using (new TransactionScope(TransactionScopeOption.Suppress, TransactionScopeAsyncFlowOption.Enabled))
            {
                await using (var conn = new SqlConnection(AdminConnectionString()))
                {
                    scripts = await conn.QueryAsync<string>(sql);
                }
            }
            scripts.Should().NotBeNull();
        }


        private MooMigrator GetMigrator(string databaseName, bool createDatabase)
        {
            var db = databaseName;
            var pw = MooTestContext.SqlServer.AdminPassword;
            var port = MooTestContext.SqlServer.Port;

            var connectionString = $"Data Source=localhost,{port};Initial Catalog={db};User Id=sa;Password={pw}";

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
