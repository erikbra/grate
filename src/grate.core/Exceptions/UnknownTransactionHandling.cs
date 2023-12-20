using System;
using System.Runtime.CompilerServices;

namespace grate.Exceptions;

public class UnknownTransactionHandling : ArgumentOutOfRangeException
{
    public UnknownTransactionHandling(object? transactionHandling,
        [CallerArgumentExpression(nameof(transactionHandling))] string argumentName = "")
        : base(argumentName, transactionHandling, "Unknown transaction handling : " + transactionHandling)
    { }
}
