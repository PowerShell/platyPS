using Markdown.MAML.Model.Markdown;
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
    ///     { "patternName", "regexPatternString", SomeFunctionName }
    /// }
    /// </summary>
    public class MarkdownPatternList : IEnumerable<MarkdownPattern>
    {
        private List<MarkdownPattern> _patternList = new List<MarkdownPattern>();

        public void Add(string patternName, string regexPattern, Action<Match, SourceExtent> matchAction)
        {
            _patternList.Add(
                new MarkdownPattern(
                    patternName,
                    regexPattern,
                    matchAction));
        }

        public void Append(MarkdownPatternList list)
        {
            _patternList.AddRange(list);
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
