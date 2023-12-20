using System;
using System.Collections.Generic;
using System.Linq;

namespace grate.Infrastructure;

public class StatementSplitter
{
    public const string BatchTerminatorReplacementString = @" |{[_REMOVE_]}| ";

    private readonly BatchSplitterReplacer _replacer;

    public StatementSplitter(string separatorRegex)
    {
        _replacer = new BatchSplitterReplacer(separatorRegex, BatchTerminatorReplacementString);
    }

    public IEnumerable<string> Split(string statement)
    {
        var replaced = _replacer.Replace(statement);

        var statements = replaced.Split(BatchTerminatorReplacementString);
        return statements.Where(HasScriptsToRun);
    }

    private static bool HasScriptsToRun(string sqlStatement)
    {
        var trimmedStatement = sqlStatement.Replace(BatchTerminatorReplacementString, string.Empty, StringComparison.InvariantCultureIgnoreCase);
        return !string.IsNullOrEmpty(trimmedStatement.ToLower().Replace(Environment.NewLine, string.Empty).Replace(" ", string.Empty));
    }

}
