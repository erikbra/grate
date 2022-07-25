using System;
using System.Threading.Tasks;
using System.Transactions;

namespace grate.Infrastructure;

public static class Snippet
{
    public static async Task<TResult> RunInSeparateTransaction<TResult>(Func<Task<TResult>> snippet)
    {
        using var s = new TransactionScope(TransactionScopeOption.Suppress, TransactionScopeAsyncFlowOption.Enabled);
        var result = await snippet();
        s.Complete();
        return result;
    }
    
    public static async Task RunInSeparateTransaction(Func<Task> snippet)
    {
        using var s = new TransactionScope(TransactionScopeOption.Suppress, TransactionScopeAsyncFlowOption.Enabled);
        await snippet();
        s.Complete();
    }
    
}
