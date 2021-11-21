﻿using System;
using System.Threading.Tasks;
using Dapper;
using FluentAssertions;
using grate.Configuration;
using grate.unittests.TestInfrastructure;
using NUnit.Framework;

namespace grate.unittests.Generic.Running_MigrationScripts
{
    [TestFixture]
    public abstract class TokenScripts : MigrationsScriptsBase
    {
        
        protected virtual string CreateDatabaseName => "create view grate as select '{{DatabaseName}}' as dbase";
        protected virtual string CreateViewMyCustomToken => "create view grate as select '{{MyCustomToken}}' as dbase";
        
        [Test]
        public async Task EnsureTokensAreReplaced()
        {
            var db = TestConfig.RandomDatabase().ToUpper();

            var knownFolders = KnownFolders.In(CreateRandomTempDirectory());
            var path = knownFolders?.Views?.Path ?? throw new Exception("Config Fail");
            WriteSql(path, "token.sql", CreateDatabaseName);

            await using (var migrator = Context.GetMigrator(db, knownFolders))
            {
                await migrator.Migrate();
            }

            string sql = $"SELECT dbase FROM grate";
            await using var conn = Context.CreateDbConnection(db);
            var actual = await conn.QuerySingleAsync<string>(sql);
            actual.Should().Be(db);

        }

        [Test]
        public async Task EnsureUserTokensAreReplaced()
        {
            var db = TestConfig.RandomDatabase();

            var knownFolders = KnownFolders.In(CreateRandomTempDirectory());
            var path = knownFolders?.Views?.Path ?? throw new Exception("Config Fail");
            WriteSql(path, "token.sql", CreateViewMyCustomToken);
            
            var config = Context.GetConfiguration(db, knownFolders) with
            {
                UserTokens = new[] {"mycustomtoken=token1"}, // This is important!
            };

            await using (var migrator = Context.GetMigrator(config))
            {
                await migrator.Migrate();
            }

            string sql = $"SELECT dbase FROM grate";
            await using var conn = Context.CreateDbConnection(db);
            var actual = await conn.QuerySingleAsync<string>(sql);
            actual.Should().Be("token1");

        }
    }
}
