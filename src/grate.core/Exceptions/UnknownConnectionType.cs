using System.Runtime.CompilerServices;

namespace grate.Exceptions;

public class UnknownConnectionType(
    object? connectionType,
    [CallerArgumentExpression(nameof(connectionType))]
    string argumentName = "")
    : ArgumentOutOfRangeException(argumentName, connectionType, "Unknown connection type: " + connectionType);
