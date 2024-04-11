using grate.Migration;

namespace grate.Infrastructure;

internal class StatementSplitter
{
    public const string BatchTerminatorReplacementString = @" |{[_REMOVE_]}| ";

    private readonly BatchSplitterReplacer _replacer;

    public StatementSplitter(ISyntax syntax)
    {
        _replacer = new BatchSplitterReplacer(syntax);
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
        return !string.IsNullOrEmpty(trimmedStatement.ToLower()
            .Replace(Environment.NewLine, string.Empty)
            .Replace("\n", string.Empty) // This is necessary to make script with unix-style line endings work on grate on Windows
            .Replace(" ", string.Empty));
    }

}
