using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Markdown.MAML.Parser
{
    public class MarkdownPattern
    {
        private Regex patternRegex;

        public string PatternName { get; private set;}

        public string PatternString { get; private set; }

        public Action<Match> MatchAction { get; private set; }

        public MarkdownPattern(
            string patternName, 
            string patternString, 
            Action<Match> matchAction)
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

        public bool TryMatchString(string inputString, out Match regexMatch)
        {
            regexMatch = this.patternRegex.Match(inputString);

            return regexMatch.Success;
        }
    }
}
