using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Markdown.MAML.Parser
{
    public class MarkdownPatternList : IEnumerable<MarkdownPattern>
    {
        private List<MarkdownPattern> _patternList = new List<MarkdownPattern>();
        private Dictionary<string, Action<Match, Group>> _matchGroupActions = 
            new Dictionary<string, Action<Match, Group>>();

        public void Add(string groupName, string regexPattern, Action<Match, Group> matchAction)
        {
            _patternList.Add(
                new MarkdownPattern(
                    groupName,
                    regexPattern,
                    matchAction));


            _matchGroupActions.Add(
                groupName,
                matchAction);
        }

        public bool TryExecuteMatchAction(string groupName, Match regexMatch, Group matchGroup)
        {
            Action<Match, Group> matchAction = null;

            if (_matchGroupActions.TryGetValue(groupName, out matchAction))
            {
                matchAction(regexMatch, matchGroup);
                return true;
            }

            return false;
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
