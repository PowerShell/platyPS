using Markdown.MAML.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Markdown.MAML.Model.Markdown;

namespace Markdown.MAML.Parser
{
    public enum ParserMode
    {
        Full,
        /// <summary>
        /// It's aimed to be used in a merge scenario. 
        /// It will allow us preserve formatting existin Markdown as is.
        /// It doesn't try to do the following:
        /// 
        /// - escaping characters
        /// - parse hyper-links
        /// - handle soft-breaks and hard-breaks
        /// </summary>
        FormattingPreserve
    }

    public class MarkdownParser
    {
        #region Private Fields

        DocumentNode _currentDocument;
        private string _documentText;
        MarkdownPatternList _markdownPatterns;
        ParserMode _parserMode;
        List<ParagraphSpan> _currentParagraphSpans;
        Action<int, int> _progressCallback;
        Action<string> _infoCallback;
        int _reportByteCount;

        private static readonly string[] LINE_BREAKS = new[] { "\r\n", "\n" };
        private static readonly char[] YAML_SEPARATORS = new[] { ':' };

        #endregion

        #region Public Methods

        public MarkdownParser() : this(null) {}

        public MarkdownParser(Action<int, int> progressCallback, Action<string> infoCallback) : this(progressCallback)
        {
            _infoCallback = infoCallback;
        }

        public MarkdownParser(Action<int, int> progressCallback) : this(progressCallback, 3000) { }

        /// <summary>
        /// </summary>
        /// <param name="progressCallback">Progress callback called sometimes and report that param1 of param2 bytes processed.</param>
        /// <param name="reportByteCount">Call progressCallback every reportByteCount bytes.</param>
        public MarkdownParser(Action<int, int> progressCallback, int reportByteCount)
        {
            _progressCallback = progressCallback;
            _reportByteCount = reportByteCount;
        }

        public DocumentNode ParseString(string[] markdownStrings)
        {
            // default is full
            return ParseString(markdownStrings, ParserMode.Full, null);
        }

        public DocumentNode ParseString(string[] markdownStrings, ParserMode parseMode, string path)
        {
            this._parserMode = parseMode;
            this._path = path;
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
                // Skip YamlMetadata block at the begining, if present
                _documentText = this.SkipYamlMetadataBlock(PrepareDocumentString(markdownString));

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
                        @"```(\w*)(\r\n)*((.|\r\n)*?)```(\r\n)*",
                        this.CreateTickCodeBlock }
                };

            if (this._parserMode == ParserMode.Full)
            {
                _markdownPatterns.Append(
                    new MarkdownPatternList
                    { 
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
                            @"\[(.+?)\]\((.*)\)",
                            this.CreateHyperlinkSpan },

                        {   "bold",
                            @"\*\*(.+?)\*\*",
                            this.CreateBoldSpan },

                        {   "italic",
                            @"\*(.+?)\*",
                            this.CreateItalicSpan },

                        {   "bold2",
                            @"(?<![a-zA-Z0-9])__(.+?)__(?![a-zA-Z0-9])",
                            this.CreateBoldSpan },
                        {   "italic2",
                            @"(?<![a-zA-Z0-9])_(.+?)_(?![a-zA-Z0-9])",
                            this.CreateItalicSpan },

                    }
                );
            }
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
                                currentColumnNumber,
                                _path);

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
                            currentColumnNumber,
                            _path);

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
                            currentColumnNumber,
                            _path);

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
                    sourceExtent,

                    // Detect if a line break after the header exists. Mutiple line breaks will be reduced to one.
                    (regexMatch.Groups[3].Captures.Count > 1) ? SectionFormatOption.LineBreakAfterHeader : SectionFormatOption.None));
        }

        private void CreateHashHeader2(Match regexMatch, SourceExtent sourceExtent)
        {
            this.FinishParagraph();

            _currentDocument.AddChildNode(
                new HeadingNode(
                    regexMatch.Groups[3].Value,
                    regexMatch.Groups[2].Value.Length,
                    sourceExtent,

                    // Detect if a line break after the header exists. Mutiple line breaks will be reduced to one.
                    (regexMatch.Groups[4].Captures.Count > 1) ? SectionFormatOption.LineBreakAfterHeader : SectionFormatOption.None));
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
                    sourceExtent,

                    // Detect if a line break after the header exists. Mutiple line breaks will be reduced to one. 
                    (regexMatch.Groups[3].Captures.Count > 1) ? SectionFormatOption.LineBreakAfterHeader : SectionFormatOption.None));
        }

        private void CreateTickCodeBlock(Match regexMatch, SourceExtent sourceExtent)
        {
                this.FinishParagraph();

                _currentDocument.AddChildNode(
                    new CodeBlockNode(
                        regexMatch.Groups[1].Value,
                        regexMatch.Groups[3].Value,
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
                        sourceExtent,
                        this._parserMode));
            }
        }

        private void CreateItalicSpan(Match regexMatch, SourceExtent sourceExtent)
        {
            this.StartParagraph();

            _currentParagraphSpans.Add(
                new TextSpan(
                    regexMatch.Groups[1].Value,
                    sourceExtent,
                    this._parserMode,
                    TextSpanStyle.Italic));
        }

        private void CreateBoldSpan(Match regexMatch, SourceExtent sourceExtent)
        {
            this.StartParagraph();

            _currentParagraphSpans.Add(
                new TextSpan(
                    regexMatch.Groups[1].Value,
                    sourceExtent,
                    this._parserMode,
                    TextSpanStyle.Bold));
        }

        private void CreateHyperlinkSpan(Match regexMatch, SourceExtent sourceExtent)
        {
            this.StartParagraph();

            _currentParagraphSpans.Add(
                new HyperlinkSpan(
                    regexMatch.Groups[1].Value,
                    regexMatch.Groups[2].Value,
                    sourceExtent,
                    this._parserMode));
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

        /// <summary>
        /// we only parse simple key-value pairs here
        /// </summary>
        /// <param name="yamlSnippet"></param>
        /// <returns></returns>
        internal static Dictionary<string, string> ParseYamlKeyValuePairs(string yamlSnippet)
        {
            var result = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            foreach (string lineIterator in yamlSnippet.Split(LINE_BREAKS, StringSplitOptions.None))
            {
                var line = lineIterator.Trim();
                if (string.IsNullOrEmpty(line.Trim()))
                {
                    continue;
                }

                string[] parts = line.Split(YAML_SEPARATORS, 2);
                if (parts.Length != 2)
                {
                    throw new ArgumentException("Invalid yaml: expected simple key-value pairs");
                }

                var key = parts[0].Trim();
                var value = parts[1].Trim();
                // we treat empty value as null
                result[key] = string.IsNullOrEmpty(value) ? null : value;
            }

            return result;
        }

        public static Dictionary<string, string> GetYamlMetadata(string markdownString)
        {
            markdownString = PrepareDocumentString(markdownString);
            int endPosition = GetYamlMetadataBlockEndOffset(markdownString);
            if (endPosition < 0)
            {
                return null;
            }
            else
            {
                const int OFFSET = 5;
                return ParseYamlKeyValuePairs(markdownString.Substring(OFFSET, endPosition - OFFSET));
            }
        }

        private static int GetYamlMetadataBlockEndOffset(string markdownString)
        {
            const int OFFSET = 5;
            if (markdownString.StartsWith("---\r\n"))
            {
                return markdownString.IndexOf("\r\n---", OFFSET);
            }

            return -1;
        }

        private string SkipYamlMetadataBlock(string documentString)
        {
            int offset = GetYamlMetadataBlockEndOffset(documentString);
            if (offset >= 0)
            {
                const int OFFSET = 5;
                return documentString.Substring(offset + OFFSET).TrimStart();
            }

            return documentString;
        }

        private static string PrepareDocumentString(string documentString)
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

        public static string UnwindMarkdownCharsEscaping(string spanText)
        {
            // this is reverse for the code in MarkdownV2Renderer.GetEscapedMarkdownText()
            spanText = spanText
                .Replace("\r\n([^\r])", "$1")

                .Replace(@"\[", @"[")
                .Replace(@"\]", @"]")
                // per https://github.com/PowerShell/platyPS/issues/121 we don't perform escaping for () in markdown renderer, but we do in the parser
                .Replace(@"\(", @"(")
                .Replace(@"\)", @")")

                .Replace(@"\`", @"`")

                .Replace(@"\<", "<")
                .Replace(@"\>", ">")

                .Replace(@"\_", "_")
                .Replace(@"\*", "*")

                .Replace(@"\\", @"\");

            // any dummy value with length >= 2
            _PrevString = "foo";
            _InList = false;

            return Regex.Replace(spanText, "([^\r\n]*)(\r\n|$)", new MatchEvaluator(LineBreaksMatchEvaluater)).Replace(" \r\n", "\r\n").Trim();
        }

        // hacky state for matcher, it would cause race condition if we decided to run in parallel
        private static string _PrevString;
        private static bool _InList;
        private string _path;

        internal static bool HasListPrefix(string s)
        {
            if (s.Length >= 2)
            {
                if (s[0] == '-' && s[1] == '-' || 
                    s[0] == '-' && s[1] == ' ' ||
                    s[0] == '*' && s[1] == ' ')
                {
                    return true;
                }
            }

            return false;
        }

        private static string LineBreaksMatchEvaluater(Match match)
        {
            // here we want proper line breaks
            // if it's a list, then preserve line-breaks.
            // otherwise, convert one line-break into a space and two-line breaks into a line-break.

            var g1 = _PrevString;
            var g2 = match.Groups[1].Value;
            _PrevString = g2;

            if (string.IsNullOrWhiteSpace(g2))
            {
                return "\r\n";
            }

            if (HasListPrefix(g1) && HasListPrefix(g2))
            {
                // this is a list
                _InList = true;
                return "\r\n" + g2;
            }

            if (_InList)
            {
                // now we are not in list, so we just finished it
                // we preserve one more line ending
                _InList = false;
                g2 = "\r\n" + g2;
            }

            return g2 + " ";
        }

        /// <summary>
        /// Callback to PowerShell host session to display reporting info on the console.
        /// </summary>
        /// <param name="format"></param>
        /// <param name="objects"></param>
        private void Report(string format, params object[] objects)
        {
            if (_infoCallback != null)
            {
                _infoCallback.Invoke(string.Format(format, objects));
            }
        }

        #endregion
    }
}
