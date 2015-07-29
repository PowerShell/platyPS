using Markdown.MAML.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Markdown.MAML.Model.Markdown;

namespace Markdown.MAML.Parser
{
    public class MarkdownParser
    {
        #region Private Fields

        DocumentNode _currentDocument;
        private string _remainingText;
        MarkdownPatternList _markdownPatternList;
        List<ParagraphSpan> _currentParagraphSpans;

        #endregion

        #region Public Methods

        public DocumentNode ParseString(string markdownString)
        {
            if (string.IsNullOrWhiteSpace(markdownString))
            {
                // TODO: Throw ArgumentException instead?
                return null;
            }

            // Initialize the pattern list
            this.InitializePatternList();

            // Trim the leading whitespace off of the string
            _remainingText = this.PrepareDocumentString(markdownString);

            _currentDocument = new DocumentNode();
            this.ParseDocument();

            return _currentDocument;
        }

        #endregion

        #region Initialization

        private void InitializePatternList()
        {
            _markdownPatternList =
                new MarkdownPatternList
                {
                    // Headers
                    {   "hash_header", 
                        @"((?<hash_header>\#+[ ]*(.+?))[ ]*\#*(\r\n)+)", 
                        this.CreateHashHeader },

                    {   "underline_header", 
                        "((?<underline_header>(.+?)[ ]*\r\n(=+|-+))[ ]*(\r\n)+)", 
                        this.CreateUnderlineHeader },

                    // Code blocks
                    {   "tick_codeblock",
                        @"((```)(\r\n)*(?<tick_codeblock>((.|\r\n)+?))(\r\n)*(```))(\r\n)*",
                        this.CreateTickCodeBlock },

                    // Paragraph spans
                    {   "hardbreak",
                        "(?<hardbreak>  \r\n)",
                        this.CreateHardBreakSpan },

                    {   "softbreak",
                        "(?<softbreak>(\r\n){{1}})",
                        this.CreateSoftBreakSpan },

                    {   "new_paragraph",
                        "(?<new_paragraph>(\r\n){{2,}})",
                        this.CreateParagraph },

                    {   "hyperlink",
                        "(?<hyperlink>\\[(.+?)\\]\\(https?://[^'\">\\s]+\\))",
                        this.CreateHyperlinkSpan },

                        // We allow hyperlinks with empty URI.
                    {   "emptyHyperlink",
                        "(?<emptyHyperlink>\\[(.+?)\\]\\(\\))",
                        this.CreateHyperlinkSpan },

                        // Normal need a negative look-ahead for hyperlinks pattern.
                    {   "normalWithHyperlink",
                        @"\s*(?<normalWithHyperlink>[{0}{1}]+)\[.*\]\(.*\)",
                        this.CreateNormalSpan },

                    {   "normal",
                        @"\s*(?<normal>[{0}{1}\[\]]+)",
                        this.CreateNormalSpan },

                    {   "italic",
                        @"(\*(?<italic>[{0}]+)\*)",
                        this.CreateItalicSpan },

                    {   "bold",
                        @"(\*\*(?<bold>[{0}]+)\*\*)",
                        this.CreateBoldSpan }
                };
        }

        #endregion

        #region Parsing Methods

        private void ParseDocument()
        {
            // TODO: These patterns are old and should be converted into
            // something more like the newer patterns which use non-greedy
            // character groups like (.?+)
            string textPattern = @"\w\s\d\-\.,'\""!\?\\<>/=&:;\$\|@\{\}\+";
            string additionalTextPattern = "\\(\\)(\r\n){1}";

            // Create the list of regexes from the pattern list
            string[] regexParts =
                _markdownPatternList
                    .Select(pattern => "^" + pattern.RegexPattern)
                    .ToArray();

            // Create a combined regex with all of the defined patterns
            Regex markdownRegex = 
                new Regex(
                    string.Format(
                        string.Join("|", regexParts),
                        textPattern,
                        additionalTextPattern), 
                    RegexOptions.ExplicitCapture |
                    RegexOptions.Compiled);

            // This algorithm works by taking the current document string
            // and using a regex to pull the first recognized substring beginning
            // at character 0.  Once a substring has been matched, that substring
            // is removed from the string and the loop starts over.
            while (_remainingText.Length > 0)
            {
                string matchedGroupName = null;
                Match regexMatch = markdownRegex.Match(_remainingText);
                
                if (!regexMatch.Success)
                {
                    string textExcerpt =
                        _remainingText.Length > 50 ?
                            _remainingText.Substring(0, 50) + "..." :
                            _remainingText;

                    throw new Exception(
                        "Failed to find a matching rule for text: " + textExcerpt);
                }

                Group matchGroup = this.GetMatchedGroup(markdownRegex, regexMatch, out matchedGroupName);

                // Try to run the match action for the matched group
                bool executedAction =
                    _markdownPatternList.TryExecuteMatchAction(
                        matchedGroupName,
                        regexMatch,
                        matchGroup);

                // TODO: remove this special case hack
                int groupLength = regexMatch.Length;
                if (matchedGroupName == "normalWithHyperlink")
                {
                    groupLength = matchGroup.Length;
                }

                // Get rid of the entire matched string
                _remainingText = _remainingText.Substring(groupLength);

                if (!executedAction)
                {
                    // Reached some unknown text, break out
                    break;
                }
            }

            // Finish any remaining paragraph
            this.FinishParagraph();
        }

        private void CreateHashHeader(Match regexMatch, Group matchGroup)
        {
            this.FinishParagraph();

            _currentDocument.AddChildNode(
                new HeadingNode(
                    matchGroup.Value.Trim('#', ' '),
                    matchGroup.Value.LastIndexOf('#') + 1));
        }

        private void CreateUnderlineHeader(Match regexMatch, Group matchGroup)
        {
            this.FinishParagraph();

            string[] headerLines = matchGroup.Value.Split('\n');

            int headerLevel = 0;
            if (headerLines[1][0] == '=')
            {
                headerLevel = 1;
            }
            else
            {
                headerLevel = 2;
            }

            _currentDocument.AddChildNode(
                new HeadingNode(
                    headerLines[0].Trim(),
                    headerLevel));
        }

        private void CreateTickCodeBlock(Match regexMatch, Group matchGroup)
        {
                this.FinishParagraph();

                _currentDocument.AddChildNode(
                    new CodeBlockNode(
                           matchGroup.Value.Trim('`', '\r', '\n')));
        }

        private void CreateNormalSpan(Match regexMatch, Group matchGroup)
        {
            this.StartParagraph();

            // TODO: Replace all newlines with spaces?  We
            // might want to add line breaks only when the
            // user has intentionally typed a hard break string
            // (  \r\n)

            _currentParagraphSpans.Add(
                new TextSpan(
                    matchGroup.Value.Trim()));
        }

        private void CreateItalicSpan(Match regexMatch, Group matchGroup)
        {
            this.StartParagraph();

            _currentParagraphSpans.Add(new TextSpan(matchGroup.Value, false, true));
        }

        private void CreateBoldSpan(Match regexMatch, Group matchGroup)
        {
            this.StartParagraph();

            _currentParagraphSpans.Add(new TextSpan(matchGroup.Value, true, false));
        }

        private void CreateHyperlinkSpan(Match regexMatch, Group matchGroup)
        {
            this.StartParagraph();

            string[] hyperlinkParts = matchGroup.Value.Split('(');

            _currentParagraphSpans.Add(
                new HyperlinkSpan(
                    hyperlinkParts[0].Trim('[', ']'),
                    hyperlinkParts[1].Trim(')')));
        }

        private void CreateSoftBreakSpan(Match regexMatch, Group matchGroup)
        {
            // Don't create a span?
        }

        private void CreateHardBreakSpan(Match regexMatch, Group matchGroup)
        {
            this.StartParagraph();

            _currentParagraphSpans.Add(new HardBreakSpan());
        }

        private void CreateParagraph(Match regexMatch, Group matchGroup)
        {
            this.FinishParagraph();
            this.StartParagraph();
        }

        #endregion

        #region Parser State Management

        private void StartParagraph()
        {
            if (_currentParagraphSpans == null)
            {
                _currentParagraphSpans = new List<ParagraphSpan>();
            }
        }

        private void FinishParagraph()
        {
            if (_currentParagraphSpans != null && 
                _currentParagraphSpans.Count > 0)
            {
                _currentDocument.AddChildNode(
                    new ParagraphNode(
                        _currentParagraphSpans));

                _currentParagraphSpans = null;
            }
        }

        #endregion

        #region Helper Methods

        private string PrepareDocumentString(string documentString)
        {
            // Trim any leading whitespace off of the string
            documentString = documentString.TrimStart();

            // Replace any invalid characters
            // TODO: Find a better way to deal with this problem in a general way
            documentString = documentString.Replace('–', '-');

            // Make sure all newlines are \r\n.  In some environments,
            // verbatim string literals have \r instead of \r\n.
            return Regex.Replace(documentString, "[^\r]\n", "\r\n");

        }

        private Group GetMatchedGroup(Regex spanRegex, Match regexMatch, out string matchedGroupName)
        {
            matchedGroupName = null;

            foreach (string groupName in spanRegex.GetGroupNames())
            {
                Group matchGroup = regexMatch.Groups[groupName];

                if (groupName != "0" && matchGroup.Success)
                {
                    matchedGroupName = groupName;
                    return matchGroup;
                }
            }

            return null;
        }

        #endregion
    }
}
