using System;
using System.Runtime.CompilerServices;

namespace grate.Exceptions;

public class UnknownConnectionType: ArgumentOutOfRangeException
{
    public UnknownConnectionType(object? connectionType,
        [CallerArgumentExpression("connectionType")] string argumentName = "")
        : base(argumentName, connectionType, "Unknown connection type: " + connectionType)
    { }
}
