using Markdown.MAML.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Markdown.MAML.Parser
{
    public class MarkdownParser
    {
        private Markdown _innerParser = new Markdown();

        DocumentNode _currentDocument;
        private string _remainingText;
        List<ParagraphSpan> _currentParagraphSpans;

        public DocumentNode ParseString(string markdownString)
        {
            if (string.IsNullOrWhiteSpace(markdownString))
            {
                // TODO: Throw ArgumentException instead?
                return null;
            }

            _remainingText = markdownString.TrimStart();
            _currentDocument = new DocumentNode();

            this.ParseDocument();

            return _currentDocument;
        }


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

        private void ParseDocument()
        {
            string[] regexParts =
                new string[]
                {
                    // Header regexes
                    @"^((?<hash_header>\#+[ ]*(.+?))[ ]*\#*(\r\n)+)",
                    "^((?<underline_header>(.+?)[ ]*\r\n(=+|-+))[ ]*(\r\n)+)",

                    // Code block regexes
                    @"^((```)(\r\n)*(?<tick_codeblock>((.|\r\n)+?))(\r\n)*(```))(\r\n)*",

                    // Paragraph span regexes
                    "^(?<hardbreak>  \r\n)",
                    "^(?<softbreak>(\r\n){{1}})",
                    "^(?<new_paragraph>(\r\n){{2,}})",
                    @"^\s*(?<normal>[{0}{1}]+)",
                    @"^(\*(?<italic>[{0}]+)\*)",
                    @"^(\*\*(?<bold>[{0}]+)\*\*)",
                    "^(?<hyperlink>\\[(.+?)\\]\\(https?://[^'\">\\s]+\\))",
                };

            string matchedGroupName = null;
            string textPattern = @"a-zA-Z-\.,' ";
            string additionalTextPattern = "\\(\\)(\r\n){1}";

            // Create a combined regex with all of the defined patterns
            Regex markdownRegex = 
                new Regex(
                    string.Format(
                        string.Join("|", regexParts),
                        textPattern,
                        additionalTextPattern), 
                    RegexOptions.ExplicitCapture |
                    RegexOptions.Compiled);

            while (_remainingText.Length > 0)
            {
                Match regexMatch = markdownRegex.Match(_remainingText);
                Group matchGroup = this.GetMatchedGroup(markdownRegex, regexMatch, out matchedGroupName);

                // Headers
                if (matchedGroupName == "hash_header")
                {
                    this.FinishParagraph();

                    _currentDocument.AddChildNode(
                        new HeadingNode(
                            matchGroup.Value.Trim('#', ' '),
                            matchGroup.Value.LastIndexOf('#') + 1));
                }
                else if (matchedGroupName == "underline_header")
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

                // Code blocks
                else if (matchedGroupName == "tick_codeblock")
                {
                    this.FinishParagraph();

                    _currentDocument.AddChildNode(
                        new CodeBlockNode(
                            matchGroup.Value.Trim('`', '\r', '\n')));
                }

                // Paragraph spans
                else if (matchedGroupName == "normal")
                {
                    this.StartParagraph();

                    _currentParagraphSpans.Add(
                        new TextSpan(
                            matchGroup.Value.Trim()));
                }
                else if (matchedGroupName == "italic")
                {
                    this.StartParagraph();

                    _currentParagraphSpans.Add(new TextSpan(matchGroup.Value, false, true));
                }
                else if (matchedGroupName == "bold")
                {
                    this.StartParagraph();

                    _currentParagraphSpans.Add(new TextSpan(matchGroup.Value, true, false));
                }
                else if (matchedGroupName == "hyperlink")
                {
                    this.StartParagraph();

                    string[] hyperlinkParts = matchGroup.Value.Split('(');

                    _currentParagraphSpans.Add(
                        new HyperlinkSpan(
                            hyperlinkParts[0].Trim('[', ']'),
                            hyperlinkParts[1].Trim(')')));
                }
                else if (matchedGroupName == "softbreak")
                {
                    // Don't create a span?
                }
                else if (matchedGroupName == "hardbreak")
                {
                    this.StartParagraph();

                    _currentParagraphSpans.Add(new HardBreakSpan());
                }
                else if (matchedGroupName == "new_paragraph")
                {
                    this.FinishParagraph();
                    this.StartParagraph();
                }

                // Get rid of the entire matched string
                _remainingText = _remainingText.Substring(regexMatch.Length);

                if (matchedGroupName == null)
                {
                    // Reached some unknown text, break out
                    break;
                }
            }

            // Finish any remaining paragraph
            this.FinishParagraph();
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
    }
}
