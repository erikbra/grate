using System.Runtime.CompilerServices;

namespace grate.Exceptions;

public class UnknownTransactionHandling(
    object? transactionHandling,
    [CallerArgumentExpression(nameof(transactionHandling))]
    string argumentName = "")
    : ArgumentOutOfRangeException(argumentName, transactionHandling,
        "Unknown transaction handling : " + transactionHandling);
