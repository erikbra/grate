using System;
using System.Collections.Generic;
using System.IO;
using FluentAssertions;
using grate.Configuration;
using grate.Infrastructure;
using grate.Migration;
using Microsoft.Extensions.Logging.Abstractions;
using NUnit.Framework;

namespace grate.unittests.Infrastructure
{
    [TestFixture]
    [Category("Basic")]
    public class TokenReplacerTests
    {
        [TestCase("")]
        [TestCase(null)]
        public void EnsureEmptyStringIsLeftEmpty(string? input)
        {
            var tokens = new Dictionary<string, string?>();
            TokenReplacer.ReplaceTokens(tokens, input).Should().Be(string.Empty);
        }

        [Test]
        public void EnsureUnknownTokenIsIgnored()
        {
            var tokens = new Dictionary<string, string?>();
            var input = "This has a {{token}} in it.";
            TokenReplacer.ReplaceTokens(tokens, input).Should().Be(input);
        }

        [Test]
        public void EnsureTokensAreReplaced()
        {
            var tokens = new Dictionary<string, string?> { ["EnvName"] = "Test" };
            var input = "This is a {{EnvName}}.";
            TokenReplacer.ReplaceTokens(tokens, input).Should().Be("This is a Test.");
        }

        [Test]
        public void EnsureConfigMakesItToTokens()
        {
            var folders = KnownFolders.In(new DirectoryInfo(Path.GetTempPath()));
            var config = new GrateConfiguration() { SchemaName = "Test", KnownFolders = folders };
            var provider = new TokenProvider(config, GrateTestContext.SqlServer.DatabaseMigrator);
            var tokens = provider.GetTokens();

            tokens["SchemaName"].Should().Be("Test");

            //RH Only uses the name of a folder, not it's full path.  Make sure we're compat
            tokens["UpFolderName"].Should().Be("up");

        }

        [Test]
        public void EnsureDBMakesItToTokens()
        {
            var config = new GrateConfiguration()
            {
                ConnectionString = "Server=(LocalDb)\\mssqllocaldb;Database=TestDb;",
                KnownFolders = KnownFolders.In(new DirectoryInfo(Path.GetTempPath()))
            };


            var db = new SqlServerDatabase(NullLogger<SqlServerDatabase>.Instance);
            db.InitializeConnections(config);

            var provider = new TokenProvider(config, db);
            var tokens = provider.GetTokens();

            tokens["DatabaseName"].Should().Be("TestDb");
            tokens["ServerName"].Should().Be("(LocalDb)\\mssqllocaldb");
        }

    }
}
