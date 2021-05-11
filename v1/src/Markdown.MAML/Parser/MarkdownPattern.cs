using Markdown.MAML.Model.Markdown;
using System;
using System.Text.RegularExpressions;

namespace Markdown.MAML.Parser
{
    public class MarkdownPattern
    {
        private Regex patternRegex;

        public string PatternName { get; private set;}

        public string PatternString { get; private set; }

        public Action<Match, SourceExtent> MatchAction { get; private set; }

        public MarkdownPattern(
            string patternName, 
            string patternString, 
            Action<Match, SourceExtent> matchAction)
        {
            this.PatternName = patternName;
            this.PatternString = patternString;
            this.MatchAction = matchAction;

            this.patternRegex =
                new Regex(
                    patternString,
                    RegexOptions.IgnorePatternWhitespace |
                    RegexOptions.Compiled);
        }

        public bool TryMatchString(string inputString, int startOffset, out Match regexMatch)
        {
            // the intention is to speed-up match process for the long strings.
            // This number is completely orbitrary
            // TODO: remove this perf hack
            const int longestExpectedParagraphSpan = 10000;
            regexMatch = this.patternRegex.Match(inputString, startOffset, Math.Min(inputString.Length - startOffset, longestExpectedParagraphSpan));
            return regexMatch.Success;
        }
    }
}
