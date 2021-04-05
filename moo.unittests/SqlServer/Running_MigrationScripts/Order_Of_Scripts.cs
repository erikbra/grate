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
            var db = "GoinRoundDrinkingMoonshine";

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

            scripts.Should().HaveCount(13);

            KnownFolders knownFolders = _config?.KnownFolders ?? throw new ArgumentNullException(nameof(_config.KnownFolders));
            Assert.Multiple(() =>
                {
                    AssertScriptPath(scripts[0], knownFolders.BeforeMigration);
                    AssertScriptPath(scripts[1], knownFolders.AlterDatabase);
                    AssertScriptPath(scripts[2], knownFolders.RunAfterCreateDatabase);
                    AssertScriptPath(scripts[3], knownFolders.RunBeforeUp);
                    AssertScriptPath(scripts[4], knownFolders.Up);
                    AssertScriptPath(scripts[5], knownFolders.RunFirstAfterUp);
                    AssertScriptPath(scripts[6], knownFolders.Views);
                    AssertScriptPath(scripts[7], knownFolders.Sprocs);
                    AssertScriptPath(scripts[8], knownFolders.Triggers);
                    AssertScriptPath(scripts[9], knownFolders.Indexes);
                    AssertScriptPath(scripts[10], knownFolders.RunAfterOtherAnyTimeScripts);
                    AssertScriptPath(scripts[11], knownFolders.Permissions);
                    AssertScriptPath(scripts[12], knownFolders.AfterMigration);
                }
            );
        }

        private static void AssertScriptPath(string scriptPath, MigrationsFolder? migrationsFolder)
        {
            scriptPath.Should().Be(Path.Combine(migrationsFolder!.Path.ToString(), "1_jalla.sql"));
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

            CreateDummySql(_config.KnownFolders.AfterMigration);
            CreateDummySql(_config.KnownFolders.AlterDatabase);
            CreateDummySql(_config.KnownFolders.BeforeMigration);
            CreateDummySql(_config.KnownFolders.Functions);
            CreateDummySql(_config.KnownFolders.Indexes);
            CreateDummySql(_config.KnownFolders.Permissions);
            CreateDummySql(_config.KnownFolders.RunAfterCreateDatabase);
            CreateDummySql(_config.KnownFolders.RunAfterOtherAnyTimeScripts);
            CreateDummySql(_config.KnownFolders.RunBeforeUp);
            CreateDummySql(_config.KnownFolders.RunFirstAfterUp);
            CreateDummySql(_config.KnownFolders.Sprocs);
            CreateDummySql(_config.KnownFolders.Triggers);
            CreateDummySql(_config.KnownFolders.Up);
            CreateDummySql(_config.KnownFolders.Views);

            dbMigrator.ApplyConfig(_config);

            return migrator;
        }

        private static void CreateDummySql(MigrationsFolder? folder)
        {
            var dummySql = "SELECT @@VERSION";

            var path = folder?.Path ?? throw new ArgumentException(nameof(folder.Path));

            if (!path.Exists)
            {
                path.Create();
            }

            File.WriteAllText(Path.Combine(path.ToString(), "1_jalla.sql"), dummySql);
        }
    }
}
