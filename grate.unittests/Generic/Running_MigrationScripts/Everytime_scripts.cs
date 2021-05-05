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
    public abstract class Everytime_scripts
    {
        protected abstract IGrateTestContext Context { get; }

        [Test]
        public async Task Are_run_every_time_even_when_unchanged()
        {
            var db = TestConfig.RandomDatabase();

            GrateMigrator? migrator;
            
            var knownFolders = KnownFolders.In(CreateRandomTempDirectory());
            CreateDummySql(knownFolders.Permissions);

            for (var i = 0; i < 3; i++)
            {
                await using (migrator = Context.GetMigrator(db, true, knownFolders))
                {
                    await migrator.Migrate();
                }
            }

            string[] scripts;
            string sql = "SELECT script_name FROM grate.\"ScriptsRun\"";
            
            await using (var conn = Context.CreateDbConnection(Context.ConnectionString(db)))
            {
                scripts = (await conn.QueryAsync<string>(sql)).ToArray();
            }

            scripts.Should().HaveCount(3);
        }
        
        [Test]
        public async Task Are_recognized_by_script_name()
        {
            var db = TestConfig.RandomDatabase();

            GrateMigrator? migrator;
            
            var knownFolders = KnownFolders.In(CreateRandomTempDirectory());

            var folder = knownFolders.Up;// not an everytime folder
            
            CreateDummySql(folder); 
            CreateEveryTimeScriptFile(folder); 
            CreateOtherEveryTimeScriptFile(folder); 

            for (var i = 0; i < 3; i++)
            {
                await using (migrator = Context.GetMigrator(db, true, knownFolders))
                {
                    await migrator.Migrate();
                }
            }

            string[] scripts;
            string sql = "SELECT script_name FROM grate.\"ScriptsRun\"";
            
            await using (var conn = Context.CreateDbConnection(Context.ConnectionString(db)))
            {
                scripts = (await conn.QueryAsync<string>(sql)).ToArray();
            }

            scripts.Should().HaveCount(7); // one time script ran once, the two everytime scripts ran every time.
        }

        private static DirectoryInfo CreateRandomTempDirectory()
        {
            var dummyFile = Path.GetTempFileName();
            File.Delete(dummyFile);

            var scriptsDir = Directory.CreateDirectory(dummyFile);
            return scriptsDir;
        }

        private static void CreateDummySql(MigrationsFolder? folder)
        {
            var dummySql = "SELECT version()";
            var path = MakeSurePathExists(folder);
            WriteSql(path, "1_jalla.sql", dummySql);
        }
        
        private static void CreateEveryTimeScriptFile(MigrationsFolder? folder)
        {
            var dummySql = "SELECT current_database()";
            var path = MakeSurePathExists(folder);
            WriteSql(path, "everytime.1_jalla.sql", dummySql);
        }
        
        private static void CreateOtherEveryTimeScriptFile(MigrationsFolder? folder)
        {
            var dummySql = "SELECT version()";
            var path = MakeSurePathExists(folder);
            WriteSql(path, "1_jalla.everytime.and.always.sql", dummySql);
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
