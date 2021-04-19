using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using FluentAssertions;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging.Abstractions;
using moo.Configuration;
using moo.Infrastructure;
using moo.Migration;
using moo.unittests.TestInfrastructure;
using NSubstitute;
using NUnit.Framework;

namespace moo.unittests.SqlServer.Running_MigrationScripts
{
    [TestFixture]
    public class Order_Of_Scripts
    {
        private MooConfiguration? _config;
        private static string? AdminConnectionString() => $"Data Source=localhost,{MooTestContext.SqlServer.Port};Initial Catalog=master;User Id=sa;Password={MooTestContext.SqlServer.AdminPassword}";
        private static string? ConnectionString(string database) => $"Data Source=localhost,{MooTestContext.SqlServer.Port};Initial Catalog={database};User Id=sa;Password={MooTestContext.SqlServer.AdminPassword}";

        [Test()]
        public async Task Is_as_expected()
        {
            var db = TestConfig.RandomDatabase();

            MooMigrator? migrator;
            await using (migrator = GetMigrator(db, true))
            {
                await migrator.Migrate();
            }

            string[] scripts;
            string sql = "SELECT script_name FROM moo.ScriptsRun";
            
            await using (var conn = new SqlConnection(ConnectionString(db)))
            {
                scripts = (await conn.QueryAsync<string>(sql)).ToArray();
            }

            scripts.Should().HaveCount(14);

            KnownFolders knownFolders = _config?.KnownFolders ?? throw new ArgumentNullException(nameof(_config.KnownFolders));
            Assert.Multiple(() =>
                {
                    scripts[0].Should().Be("1_beforemigration.sql");
                    scripts[1].Should().Be("1_alterdatabase.sql");
                    scripts[2].Should().Be("1_aftercreate.sql");
                    scripts[3].Should().Be("1_beforeup.sql");
                    scripts[4].Should().Be("1_up.sql");
                    scripts[5].Should().Be("1_firstafterup.sql");
                    scripts[6].Should().Be("1_functions.sql");
                    scripts[7].Should().Be("1_views.sql");
                    scripts[8].Should().Be("1_sprocs.sql");
                    scripts[9].Should().Be("1_triggers.sql");
                    scripts[10].Should().Be("1_indexes.sql");
                    scripts[11].Should().Be("1_afterotherany.sql");
                    scripts[12].Should().Be("1_permissions.sql");
                    scripts[13].Should().Be("1_aftermigration.sql");
                }
            );
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

            var dummyFile = Path.GetTempFileName();
            File.Delete(dummyFile);

            var scriptsDir = Directory.CreateDirectory(dummyFile);

            _config = new MooConfiguration()
            {
                CreateDatabase = createDatabase, 
                ConnectionString = connectionString,
                AdminConnectionString = AdminConnectionString(),
                Version = "a.b.c.d",
                KnownFolders = KnownFolders.In(scriptsDir),
                AlterDatabase = true
            };

            CreateDummySql(_config.KnownFolders.AfterMigration, "1_aftermigration.sql");
            CreateDummySql(_config.KnownFolders.AlterDatabase, "1_alterdatabase.sql");
            CreateDummySql(_config.KnownFolders.BeforeMigration, "1_beforemigration.sql");
            CreateDummySql(_config.KnownFolders.Functions, "1_functions.sql");
            CreateDummySql(_config.KnownFolders.Indexes, "1_indexes.sql");
            CreateDummySql(_config.KnownFolders.Permissions, "1_permissions.sql");
            CreateDummySql(_config.KnownFolders.RunAfterCreateDatabase, "1_aftercreate.sql");
            CreateDummySql(_config.KnownFolders.RunAfterOtherAnyTimeScripts, "1_afterotherany.sql");
            CreateDummySql(_config.KnownFolders.RunBeforeUp, "1_beforeup.sql");
            CreateDummySql(_config.KnownFolders.RunFirstAfterUp, "1_firstafterup.sql");
            CreateDummySql(_config.KnownFolders.Sprocs, "1_sprocs.sql");
            CreateDummySql(_config.KnownFolders.Triggers, "1_triggers.sql");
            CreateDummySql(_config.KnownFolders.Up, "1_up.sql");
            CreateDummySql(_config.KnownFolders.Views, "1_views.sql");

            dbMigrator.ApplyConfig(_config);

            return migrator;
        }

        private static void CreateDummySql(MigrationsFolder? folder, string filename)
        {
            var dummySql = "SELECT @@VERSION";

            var path = folder?.Path ?? throw new ArgumentException(nameof(folder.Path));

            if (!path.Exists)
            {
                path.Create();
            }

            File.WriteAllText(Path.Combine(path.ToString(), filename), dummySql);
        }
    }
}
