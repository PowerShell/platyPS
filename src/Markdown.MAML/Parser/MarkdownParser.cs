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
                return null;
            }

            _currentLine = _markdownLines[_currentLineIndex];
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
                    this.ParseHeading
                };

            while (_currentLine != null)
            {
                // Find the first parseable element
                MarkdownNode foundNode =
                    parseActions
                        .Select(parser => (MarkdownNode)parser())
                        .FirstOrDefault();

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
                }
            }

            // TODO: Look for children

            return headingNode;
        }
    }
}
