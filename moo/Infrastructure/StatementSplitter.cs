using System.Collections.Generic;

namespace moo.Infrastructure
{
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
            return replaced.Split(BatchTerminatorReplacementString);
        }

    }
}