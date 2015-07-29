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
        MarkdownPatternList _markdownPatterns;
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
            _markdownPatterns =
                new MarkdownPatternList
                {
                    // Headers
                    {   "hash_header", 
                        @"(\#+)[ ]*(.+?)[ ]*\#*(\r\n)+", 
                        this.CreateHashHeader },

                    {   "underline_header", 
                        @"(.+?)[ ]*\r\n(==+|--+)[ ]*(\r\n)+", 
                        this.CreateUnderlineHeader },

                    // Code blocks
                    {   "tick_codeblock",
                        @"```(\r\n)*((.|\r\n)+?)```(\r\n)*",
                        this.CreateTickCodeBlock },

                    // Paragraph spans
                    {   "hardbreak",
                        @"[ ]{2}\r\n",
                        this.CreateHardBreakSpan },

                    {   "softbreak",
                        @"(\r\n){{1}}",
                        this.CreateSoftBreakSpan },

                    {   "new_paragraph",
                        @"(\r\n){{2,}}",
                        this.CreateParagraph },

                    {   "hyperlink",
                        @"\[(.+?)\]\((https?://[^'\"">\s]+)?\)",
                        this.CreateHyperlinkSpan },

                    {   "bold",
                        @"\*\*(.+?)\*\*",
                        this.CreateBoldSpan },

                    {   "italic",
                        @"\*(.+?)\*",
                        this.CreateItalicSpan }
                };
        }

        #endregion

        #region Parsing Methods

        private void ParseDocument()
        {
            // This algorithm works by taking the current document string
            // and using a regex to pull the first recognized substring beginning
            // at character 0.  Once a substring has been matched, that substring
            // is removed from the string and the loop starts over.
            while (_remainingText.Length > 0)
            {
                // Try each of the patterns to find a match
                Match firstMatch = null;
                MarkdownPattern firstMatchedPattern = null;
                foreach (MarkdownPattern markdownPattern in _markdownPatterns)
                {
                    Match regexMatch = null;
                    if (markdownPattern.TryMatchString(_remainingText, out regexMatch))
                    {
                        if (firstMatch == null || firstMatch.Index > regexMatch.Index)
                        {
                            firstMatch = regexMatch;
                            firstMatchedPattern = markdownPattern;
                        }
                    }
                }

                int newStartingPosition = -1;
                if (firstMatch != null)
                {
                    // Gather all text before this point into a paragraph
                    if (firstMatch.Index > 0)
                    {
                        this.StartParagraph();

                        this.CreateNormalSpan(
                            _remainingText.Substring(
                                0,
                                firstMatch.Index));
                    }

                    // Run the match action for the pattern
                    firstMatchedPattern.MatchAction(firstMatch);

                    // Execute the action for the match
                    newStartingPosition = firstMatch.Index + firstMatch.Length;
                }
                else
                {
                    // If no match found, treat the rest of the text as a span
                    this.CreateNormalSpan(_remainingText);
                    newStartingPosition = _remainingText.Length;
                }

                // Trim the head of the string
                _remainingText = 
                    _remainingText.Substring(
                        newStartingPosition);
            }

            // Finish any remaining paragraph
            this.FinishParagraph();
        }

        private void CreateHashHeader(Match regexMatch)
        {
            this.FinishParagraph();

            _currentDocument.AddChildNode(
                new HeadingNode(
                    regexMatch.Groups[2].Value,
                    regexMatch.Groups[1].Value.Length));
        }

        private void CreateUnderlineHeader(Match regexMatch)
        {
            this.FinishParagraph();

            int headerLevel =
                regexMatch.Groups[2].Value[0] == '=' ?
                    1 : 2;

            _currentDocument.AddChildNode(
                new HeadingNode(
                    regexMatch.Groups[1].Value,
                    headerLevel));
        }

        private void CreateTickCodeBlock(Match regexMatch)
        {
                this.FinishParagraph();

                _currentDocument.AddChildNode(
                    new CodeBlockNode(
                        regexMatch.Groups[2].Value));
        }

        private void CreateNormalSpan(string spanText)
        {
            this.StartParagraph();

            // TODO: Replace all newlines with spaces?  We
            // might want to add line breaks only when the
            // user has intentionally typed a hard break string
            // (  \r\n)

            // If the span is merely whitespace, don't add it
            if (!string.IsNullOrWhiteSpace(spanText))
            {
                _currentParagraphSpans.Add(new TextSpan(spanText));
            }
        }

        private void CreateItalicSpan(Match regexMatch)
        {
            this.StartParagraph();

            _currentParagraphSpans.Add(
                new TextSpan(
                    regexMatch.Groups[1].Value,
                    TextSpanStyle.Italic));
        }

        private void CreateBoldSpan(Match regexMatch)
        {
            this.StartParagraph();

            _currentParagraphSpans.Add(
                new TextSpan(
                    regexMatch.Groups[1].Value,
                    TextSpanStyle.Bold));
        }

        private void CreateHyperlinkSpan(Match regexMatch)
        {
            this.StartParagraph();

            _currentParagraphSpans.Add(
                new HyperlinkSpan(
                    regexMatch.Groups[1].Value,
                    regexMatch.Groups[2].Value));
        }

        private void CreateSoftBreakSpan(Match regexMatch)
        {
            // Don't create a span?
        }

        private void CreateHardBreakSpan(Match regexMatch)
        {
            this.StartParagraph();

            _currentParagraphSpans.Add(new HardBreakSpan());
        }

        private void CreateParagraph(Match regexMatch)
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

        #endregion
    }
}
