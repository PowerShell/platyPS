using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Markdown.MAML.Parser
{
    /// <summary>
    /// This class provides a way to concisely define a list of
    /// MarkdownPatterns using C# initializer syntax.  The Add
    /// method here is what gets invoked when using the following
    /// initializer syntax:
    /// 
    /// new MarkdownPatternList
    /// {
    ///     {
    /// }
    /// </summary>
    public class MarkdownPatternList : IEnumerable<MarkdownPattern>
    {
        private List<MarkdownPattern> _patternList = new List<MarkdownPattern>();
        private Dictionary<string, Action<Match>> _matchGroupActions = 
            new Dictionary<string, Action<Match>>();

        public void Add(string patternName, string regexPattern, Action<Match> matchAction)
        {
            _patternList.Add(
                new MarkdownPattern(
                    patternName,
                    regexPattern,
                    matchAction));


            _matchGroupActions.Add(
                patternName,
                matchAction);
        }

        public IEnumerator<MarkdownPattern> GetEnumerator()
        {
            return _patternList.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _patternList.GetEnumerator();
        }
    }
}
