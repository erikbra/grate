using System.Collections.Generic;
using FluentAssertions;
using grate.Configuration;
using grate.Infrastructure;
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
            var config = new GrateConfiguration() { SchemaName = "Test"};
            var provider = new TokenProvider(config);
            var tokens = provider.GetTokens();

            tokens["SchemaName"].Should().Be("Test");

        }

    }
}
