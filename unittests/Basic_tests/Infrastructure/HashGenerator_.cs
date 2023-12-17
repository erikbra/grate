using FluentAssertions;
using grate.Infrastructure;
using Xunit;

namespace Basic_tests.Infrastructure;


// ReSharper disable once InconsistentNaming
public class HashGenerator_
{
    [Fact]
    public void Generates_the_correct_hash()
    {
        string text_to_hash = "I want to see what the freak is going on here";
        string expected_hash = "TMGPZJmBhSO5uYbf/TBqNA==";

        var hashGen = new HashGenerator();
        hashGen.Hash(text_to_hash).Should().Be(expected_hash);
    }
}
