using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using FluentAssertions;
using grate.Configuration;
using grate.Infrastructure;
using grate.Migration;
using grate.unittests.TestInfrastructure;
using Microsoft.Extensions.Logging.Abstractions;
using Npgsql;
using NSubstitute;
using NUnit.Framework;

namespace grate.unittests.Generic.Running_MigrationScripts
{
    [TestFixture]
    public abstract class Environment_scripts
    {
        protected abstract IGrateTestContext Context { get; }

        [Test]
        public async Task Are_not_run_if_not_in_environment()
        {
            var db = TestConfig.RandomDatabase();

            GrateMigrator? migrator;
            
            var knownFolders = KnownFolders.In(CreateRandomTempDirectory());
            CreateDummySql(knownFolders.Up, "1_.OTHER.filename.ENV.sql");

            string[] environments = new[] {"TEST"};
            await using (migrator = Context.GetMigrator(db, true, knownFolders, environments))
            {
                await migrator.Migrate();
            }

            string[] scripts;
            string sql = "SELECT script_name FROM grate.\"ScriptsRun\"";
            
            await using (var conn = Context.CreateDbConnection(Context.ConnectionString(db)))
            {
                scripts = (await conn.QueryAsync<string>(sql)).ToArray();
            }

            scripts.Should().BeEmpty();
        }
        
        [Test]
        public async Task Are_run_if_in_environment()
        {
            var db = TestConfig.RandomDatabase();

            GrateMigrator? migrator;
            
            var knownFolders = KnownFolders.In(CreateRandomTempDirectory());
            CreateDummySql(knownFolders.Up, "1_.TEST.filename.ENV.sql");
            CreateDummySql(knownFolders.Up, "2_.TEST.ENV.otherfilename.sql");

            string[] environments = new[] {"TEST"};
            await using (migrator = Context.GetMigrator(db, true, knownFolders, environments))
            {
                await migrator.Migrate();
            }

            string[] scripts;
            string sql = "SELECT script_name FROM grate.\"ScriptsRun\"";
            
            await using (var conn = Context.CreateDbConnection(Context.ConnectionString(db)))
            {
                scripts = (await conn.QueryAsync<string>(sql)).ToArray();
            }

            scripts.Should().HaveCount(2);
        }
        
        [Test]
        public async Task Non_environment_scripts_are_always_run()
        {
            var db = TestConfig.RandomDatabase();

            GrateMigrator? migrator;
            
            var knownFolders = KnownFolders.In(CreateRandomTempDirectory());
            CreateDummySql(knownFolders.Up, "1_.filename.sql");
            CreateDummySql(knownFolders.Up, "2_.TEST.ENV.otherfilename.sql");
            CreateDummySql(knownFolders.Up, "2_.TEST.ENV.somethingelse.sql");

            string[] environments = new[] {"PROD"};
            await using (migrator = Context.GetMigrator(db, true, knownFolders, environments))
            {
                await migrator.Migrate();
            }

            string[] scripts;
            string sql = "SELECT script_name FROM grate.\"ScriptsRun\"";
            
            await using (var conn = Context.CreateDbConnection(Context.ConnectionString(db)))
            {
                scripts = (await conn.QueryAsync<string>(sql)).ToArray();
            }

            scripts.Should().HaveCount(1);
        }

        private static DirectoryInfo CreateRandomTempDirectory()
        {
            var dummyFile = Path.GetTempFileName();
            File.Delete(dummyFile);

            var scriptsDir = Directory.CreateDirectory(dummyFile);
            return scriptsDir;
        }

        private void CreateDummySql(MigrationsFolder? folder, string filename)
        {
            var dummySql = Context.Sql.SelectVersion;
            var path = MakeSurePathExists(folder);
            WriteSql(path, filename, dummySql);
        }
       
        private static void WriteSql(DirectoryInfo path, string filename, string? sql)
        {
            File.WriteAllText(Path.Combine(path.ToString(), filename), sql);
        }

        private static DirectoryInfo MakeSurePathExists(MigrationsFolder? folder)
        {
            var path = folder?.Path ?? throw new ArgumentException(nameof(folder.Path));

            if (!path.Exists)
            {
                path.Create();
            }

            return path;
        }
    }
}
