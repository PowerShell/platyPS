using Markdown.MAML.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Markdown.MAML.Parser
{
    public class MarkdownParser
    {
        private Markdown _innerParser = new Markdown();
        private string _currentLine;
        private int _currentLineIndex = -1;
        private string[] _markdownLines;

        public DocumentNode ParseString(string markdownString)
        {
            if (string.IsNullOrWhiteSpace(markdownString))
            {
                // TODO: Throw ArgumentException instead?
                return null;
            }

            // Split the markdown string into lines
            _markdownLines =
                _innerParser
                    .Normalize(markdownString)
                    .Trim()
                    .Split('\n');

            // Move to the starting line
            this.MoveToNextLine();

            return this.ParseDocument();
        }

        private string MoveToNextLine()
        {
            _currentLineIndex++;

            if (_currentLineIndex >= _markdownLines.Length)
            {
                _currentLineIndex = _markdownLines.Length;
                _currentLine = null;
            }
            else
            {
                _currentLine = _markdownLines[_currentLineIndex];
            }

            return _currentLine;
        }

        private string PeekNextLine()
        {
            if (_currentLineIndex + 1 < _markdownLines.Length)
            {
                return _markdownLines[_currentLineIndex + 1];
            }

            return null;
        }

        private DocumentNode ParseDocument()
        {
            DocumentNode documentNode = new DocumentNode();

            IEnumerable<Func<MarkdownNode>> parseActions =
                new List<Func<MarkdownNode>>
                {
                    // The order of these parsers is important.  The
                    // most greedy parsers should go at the end.
                    this.ParseHeading,
                    this.ParseCodeBlock,
                    this.ParseParagraph
                };

            while (_currentLine != null)
            {
                // Skip over empty lines that aren't part of a node
                if (string.IsNullOrWhiteSpace(_currentLine))
                {
                    this.MoveToNextLine();
                    continue;
                }

                // Find the first parseable element
                MarkdownNode foundNode =
                    parseActions
                        .Select(parser => (MarkdownNode)parser())
                        .FirstOrDefault(node => node != null);

                if (foundNode != null)
                {
                    // Store the node and get the next line
                    documentNode.AddChildNode(foundNode);
                    this.MoveToNextLine();
                }
                else
                {
                    break;
                }
            }

            return documentNode;
        }

        private HeadingNode ParseHeading()
        {
            HeadingNode headingNode = null;
            string nextLine = this.PeekNextLine();

            if (this._currentLine.StartsWith("#"))
            {
                // Get the heading level and text
                int i;
                int headingLevel = 0;
                for (i = 0; i < _currentLine.Length; i++)
                {
                    if (_currentLine[i] != '#')
                    {
                        break;
                    }

                    headingLevel++;
                }

                headingNode =
                    new HeadingNode(
                        _currentLine.Substring(i).Trim(),
                        headingLevel);
            }
            else if (nextLine != null && 
                     (nextLine.StartsWith("--") ||
                      nextLine.StartsWith("==")))
            {
                // Underline must be at least as long as heading string
                if (nextLine.Length >= _currentLine.Length)
                {
                    headingNode =
                        new HeadingNode(
                            _currentLine.Trim(),
                            nextLine[0] == '=' ? 1 : 2);

                    // Move past the next line
                    this.MoveToNextLine();
                    this.MoveToNextLine();
                }
            }

            // TODO: Look for children

            return headingNode;
        }

        private ParagraphNode ParseParagraph()
        {
            string paragraphText = string.Empty;

            // Any line that starts with an alphanumeric character is valid
            while (_currentLine != null && 
                   (_currentLine.Length == 0 ||
                    char.IsLetterOrDigit(_currentLine[0])))
            {
                paragraphText = 
                    paragraphText +
                    (string.IsNullOrEmpty(paragraphText) ? string.Empty : "\n") +
                    _currentLine;

                // Get the next line
                this.MoveToNextLine();
            }

            return
                !string.IsNullOrEmpty(paragraphText) ?
                    new ParagraphNode(paragraphText) :
                    null;
        }

        private CodeBlockNode ParseCodeBlock()
        {
            CodeBlockNode codeBlockNode = null;

            if (_currentLine.StartsWith("```"))
            {
                // Gather the code block text until the final ```
                string codeBlockText = _currentLine.Trim('`', ' ');

                // TODO: Handle cases where more text comes before or after ```?
                while (this.MoveToNextLine() != null &&
                        _currentLine.Trim().Equals("```") == false)
                {
                    // If there's already text in the code block, add a newline
                    codeBlockText =
                        codeBlockText +
                        (string.IsNullOrEmpty(codeBlockText) ? string.Empty : "\n") +
                        _currentLine;
                }

                codeBlockNode = new CodeBlockNode(codeBlockText);
            }

            return codeBlockNode;
        }
    }
}
