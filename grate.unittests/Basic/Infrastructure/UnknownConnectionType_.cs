using FluentAssertions;
using grate.Infrastructure;
using NUnit.Framework;

namespace grate.unittests.Basic.Infrastructure;

// ReSharper disable once InconsistentNaming
public class UnknownConnectionType_
{
    [Test]
    public void Outputs_The_Correct_Error_Message()
    {
        var p = "jalla";
        var e = Assert.Throws<UnknownConnectionType>(() => throw new UnknownConnectionType(p));

        e?.Message.Should().Be(
$@"Unknown connection type: {p} (Parameter 'p')
Actual value was {p}.");
    }
    
    [Test]
    public void Includes_The_Correct_ActualValue()
    {
        var p = "jalla";
        var e = Assert.Throws<UnknownConnectionType>(() => throw new UnknownConnectionType(p));

        e?.ActualValue.Should().Be(p);
    }
    
    [Test]
    public void Includes_The_Correct_ParamName()
    {
        var p = "jalla";
        var e = Assert.Throws<UnknownConnectionType>(() => throw new UnknownConnectionType(p));

        e?.ParamName.Should().Be("p");
    }
    
    
}
