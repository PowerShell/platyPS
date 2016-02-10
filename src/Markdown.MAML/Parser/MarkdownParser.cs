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
        private string _documentText;
        MarkdownPatternList _markdownPatterns;
        List<ParagraphSpan> _currentParagraphSpans;
        Action<int, int> _progressCallback;
        int _reportByteCount;

        #endregion

        #region Public Methods

        public MarkdownParser() : this(null) {}

        public MarkdownParser(Action<int, int> progressCallback) : this(progressCallback, 3000) { }

        /// <summary>
        /// </summary>
        /// <param name="progressCallback">Progress callback called sometimes and report that param1 of param2 bytes processed.</param>
        /// <param name="reportByteCount">Call progressCallback every reportByteCount bytes.</param>
        public MarkdownParser(Action<int, int> progressCallback, int reportByteCount)
        {
            _progressCallback = progressCallback;
        }

        public DocumentNode ParseString(string markdownString)
        {
            return ParseString(new string[] { markdownString });
        }

        public DocumentNode ParseString(string[] markdownStrings)
        {
            this.InitializePatternList();
            _currentDocument = new DocumentNode();

            for (int i = 0; i < markdownStrings.Length; i++)
            {
                string markdownString = markdownStrings[i];
                // if there are more then 1 markdownString, we should report progress
                // per strings, not for the whole string.
                if (markdownStrings.Length > 1 && this._progressCallback != null)
                {
                    this._progressCallback(i, markdownStrings.Length);
                }

                if (string.IsNullOrWhiteSpace(markdownString))
                {
                    continue;
                }

                // Trim the leading whitespace off of the string
                _documentText = this.PrepareDocumentString(markdownString);

                this.ParseDocument(markdownStrings.Length == 1);
            }

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
                    
                    // This is a dirty hack to tell avoid hash_headers, if it's not a beginning of a line
                    {   "hash_header2", 
                        @"(\r\n)+(\#+)[ ]*(.+?)[ ]*\#*(\r\n)+", 
                        this.CreateHashHeader2 },

                    {   "underline_header", 
                        @"(.+?)[ ]*\r\n(==+|--+)[ ]*(\r\n)+", 
                        this.CreateUnderlineHeader },

                    // Code blocks
                    {   "tick_codeblock",
                        @"```\w*(\r\n)*((.|\r\n)*?)```(\r\n)*",
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

        private void ParseDocument(bool reportProgress)
        {
            // This algorithm works by applying each Markdown pattern to the
            // document string starting at startOffset and finding which match
            // is nearest to the startOffset.  If the match is at a position
            // greater than startOffset, the starting part of the string is
            // then treated as a normal text span before the matched section is
            // converted into its node type.
            int startOffset = 0;
            int currentLineNumber = 0;
            int currentColumnNumber = 0;

            int lastOffset = 0;
 
            while (startOffset < _documentText.Length)
            {
                // progress reporting
                if (reportProgress && _progressCallback != null)
                {
                    if (lastOffset + _reportByteCount < startOffset)
                    {
                        _progressCallback(startOffset, _documentText.Length);
                        lastOffset = startOffset;
                    }
                }

                // Try each of the patterns to find a match
                Match firstMatch = null;
                MarkdownPattern firstMatchedPattern = null;
                foreach (MarkdownPattern markdownPattern in _markdownPatterns)
                {
                    Match regexMatch = null;
                    // This is a dirty hack to tell avoid hash_headers, if it's not a beginning of a line
                    if (markdownPattern.PatternName == "hash_header")
                    {
                        if (_documentText[startOffset] != '#')
                        {
                            continue;
                        }
                    }

                    if (markdownPattern.TryMatchString(_documentText, startOffset, out regexMatch))
                    {
                        if (firstMatch == null || firstMatch.Index > regexMatch.Index)
                        {
                            firstMatch = regexMatch;
                            firstMatchedPattern = markdownPattern;
                            if (regexMatch.Index == startOffset)
                            {
                                // no reason to continue
                                break;
                            } 
                        }
                    }
                }

                if (firstMatch != null)
                {
                    // Gather all text before this point into a paragraph
                    if (firstMatch.Index > 0)
                    {
                        this.StartParagraph();

                        // Get the extent of the span text
                        SourceExtent spanExtent =
                            new SourceExtent(
                                _documentText,
                                startOffset,
                                firstMatch.Index,
                                currentLineNumber,
                                currentColumnNumber);

                        this.CreateNormalSpan(
                            spanExtent.OriginalText,
                            spanExtent);

                        // Make sure the line and column number are updated
                        // before calculating the position of the match
                        currentLineNumber = spanExtent.Line.End;
                        currentColumnNumber = spanExtent.Column.End;
                    }

                    // Count the newlines in the entire span
                    SourceExtent matchExtent =
                        new SourceExtent(
                            _documentText,
                            firstMatch.Index,
                            firstMatch.Index + firstMatch.Length,
                            currentLineNumber,
                            currentColumnNumber);

                    // Run the match action for the pattern
                    firstMatchedPattern.MatchAction(firstMatch, matchExtent);

                    // Calculate the next offset, line, and column
                    startOffset = firstMatch.Index + firstMatch.Length;
                    currentLineNumber = matchExtent.Line.End;
                    currentColumnNumber = matchExtent.Column.End;
                }
                else
                {
                    // Get the extent containing the remaining text
                    SourceExtent spanExtent =
                        new SourceExtent(
                            _documentText,
                            startOffset,
                            _documentText.Length,
                            currentLineNumber,
                            currentColumnNumber);

                    // If no match found, treat the rest of the text as a span
                    this.CreateNormalSpan(
                        spanExtent.OriginalText,
                        spanExtent);

                    startOffset = _documentText.Length;
                }
            }

            // Finish any remaining paragraph
            this.FinishParagraph();
        }

        private void CreateHashHeader(Match regexMatch, SourceExtent sourceExtent)
        {
            this.FinishParagraph();

            _currentDocument.AddChildNode(
                new HeadingNode(
                    regexMatch.Groups[2].Value,
                    regexMatch.Groups[1].Value.Length,
                    sourceExtent));
        }

        private void CreateHashHeader2(Match regexMatch, SourceExtent sourceExtent)
        {
            this.FinishParagraph();

            _currentDocument.AddChildNode(
                new HeadingNode(
                    regexMatch.Groups[3].Value,
                    regexMatch.Groups[2].Value.Length,
                    sourceExtent));
        }

        private void CreateUnderlineHeader(Match regexMatch, SourceExtent sourceExtent)
        {
            this.FinishParagraph();

            int headerLevel =
                regexMatch.Groups[2].Value[0] == '=' ?
                    1 : 2;

            _currentDocument.AddChildNode(
                new HeadingNode(
                    regexMatch.Groups[1].Value,
                    headerLevel,
                    sourceExtent));
        }

        private void CreateTickCodeBlock(Match regexMatch, SourceExtent sourceExtent)
        {
                this.FinishParagraph();

                _currentDocument.AddChildNode(
                    new CodeBlockNode(
                        regexMatch.Groups[2].Value,
                        sourceExtent));
        }

        private void CreateNormalSpan(string spanText, SourceExtent sourceExtent)
        {
            this.StartParagraph();

            // TODO: Replace all newlines with spaces?  We
            // might want to add line breaks only when the
            // user has intentionally typed a hard break string
            // (  \r\n)

            // If the span is merely whitespace, don't add it
            if (!string.IsNullOrWhiteSpace(spanText))
            {
                _currentParagraphSpans.Add(
                    new TextSpan(
                        spanText,
                        sourceExtent));
            }
        }

        private void CreateItalicSpan(Match regexMatch, SourceExtent sourceExtent)
        {
            this.StartParagraph();

            _currentParagraphSpans.Add(
                new TextSpan(
                    regexMatch.Groups[1].Value,
                    sourceExtent,
                    TextSpanStyle.Italic));
        }

        private void CreateBoldSpan(Match regexMatch, SourceExtent sourceExtent)
        {
            this.StartParagraph();

            _currentParagraphSpans.Add(
                new TextSpan(
                    regexMatch.Groups[1].Value,
                    sourceExtent,
                    TextSpanStyle.Bold));
        }

        private void CreateHyperlinkSpan(Match regexMatch, SourceExtent sourceExtent)
        {
            this.StartParagraph();

            _currentParagraphSpans.Add(
                new HyperlinkSpan(
                    regexMatch.Groups[1].Value,
                    regexMatch.Groups[2].Value,
                    sourceExtent));
        }

        private void CreateSoftBreakSpan(Match regexMatch, SourceExtent sourceExtent)
        {
            // Don't create a span?
        }

        private void CreateHardBreakSpan(Match regexMatch, SourceExtent sourceExtent)
        {
            this.StartParagraph();

            _currentParagraphSpans.Add(
                new HardBreakSpan(
                    sourceExtent));
        }

        private void CreateParagraph(Match regexMatch, SourceExtent sourceExtent)
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
            // regex source: http://stackoverflow.com/questions/3219014/what-is-a-cross-platform-regex-for-removal-of-line-breaks
            return Regex.Replace(documentString, "\r\n?|\n", "\r\n");
        }

        #endregion
    }
}
