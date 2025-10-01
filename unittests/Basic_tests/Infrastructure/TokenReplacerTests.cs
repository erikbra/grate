using FluentAssertions;
using grate.Configuration;
using grate.Infrastructure;
using grate.Migration;
using NSubstitute;

namespace Basic_tests.Infrastructure;


public class TokenReplacerTests
{
    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void EnsureEmptyStringIsLeftEmpty(string? input)
    {
        var tokens = new Dictionary<string, string?>();
        TokenReplacer.ReplaceTokens(tokens, input).Should().Be(string.Empty);
    }

    [Fact]
    public void EnsureUnknownTokenIsIgnored()
    {
        var tokens = new Dictionary<string, string?>();
        var input = "This has a {{token}} in it.";
        TokenReplacer.ReplaceTokens(tokens, input).Should().Be(input);
    }

    [Fact]
    public void EnsureTokensAreReplaced()
    {
        var tokens = new Dictionary<string, string?> { ["EnvName"] = "Test" };
        var input = "This is a {{EnvName}}.";
        TokenReplacer.ReplaceTokens(tokens, input).Should().Be("This is a Test.");
    }

    [Fact]
    public void EnsureConfigMakesItToTokens()
    {
        var folders = Folders.Default;
        var config = new GrateConfiguration() { SchemaName = "Test", Folders = folders };
        var provider = new TokenProvider(config, Substitute.For<IDatabase>());
        var tokens = provider.GetTokens();

        tokens["SchemaName"].Should().Be("Test");

        //RH Only uses the name of a folder, not it's full path.  Make sure we're compat
        tokens["UpFolderName"].Should().Be("up");

    }

    [Fact]
    public void EnsureUserTokenParserWorks()
    {
        TokenProvider.ParseUserToken("token=value   ").Should().Be(("token", "value"));
        Assert.Throws<ArgumentOutOfRangeException>(() => TokenProvider.ParseUserToken("token"));

        // #641: While we initially wanted to protect migrating users from our change to use multiple `--ut` command line params, there's
        // legitimate scenarios where we want an `=` in the value.  
        TokenProvider.ParseUserToken("token1=value=with=equals").Should().Be(("token1", "value=with=equals"));
        TokenProvider.ParseUserToken("token1=value1;token2=value2").Should().Be(("token1", "value1;token2=value2"));
    }
}
