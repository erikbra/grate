using System.Text.RegularExpressions;
using static System.Text.RegularExpressions.RegexOptions;

namespace grate.Infrastructure;

public class BatchSplitterReplacer
{
    private string Replacement { get; }
    private readonly Regex _regex;

    public BatchSplitterReplacer(string pattern, string replacement)
    {
        Replacement = replacement;
        _regex = new Regex(pattern, IgnoreCase | Multiline);
    }

    public string Replace(string text)
    {
        var replace = _regex.Replace(text, ReplaceBatchSeparator);
        
        // Combine multiple consecutive replacement tokens with one (needed for Oracle, if ; and / are on separate lines)
        replace = Regex.Replace(
                replace, 
                Regex.Escape(Replacement) + "(\\s*)" + Regex.Escape(Replacement),
                "$1" + Replacement);
        
        return replace;
    }

    private string ReplaceBatchSeparator(Match match)
    {
        var groups = match.Groups;
        var replacement = groups["BATCHSPLITTER"].Success ? Replacement : string.Empty;
        return groups["KEEP1"].Value + replacement + groups["KEEP2"].Value;
    }
}
