using System;
using System.Runtime.CompilerServices;

namespace grate.Exceptions;

public class UnknownConnectionType : ArgumentOutOfRangeException
{
    public UnknownConnectionType(object? connectionType,
        [CallerArgumentExpression(nameof(connectionType))] string argumentName = "")
        : base(argumentName, connectionType, "Unknown connection type: " + connectionType)
    { }
}
