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
        public string GroupName { get; private set;}

        public string RegexPattern { get; private set; }

        public Action<Match, Group> MatchAction { get; private set; }

        public MarkdownPattern(
            string groupName, 
            string regexPattern, 
            Action<Match, Group> matchAction)
        {
            this.GroupName = groupName;
            this.RegexPattern = regexPattern;
            this.MatchAction = matchAction;
        }
    }

}
