using System.Diagnostics;

namespace Markdown.MAML.Model.Markdown
{
    public class SourceRange
    {
        public int Start { get; private set; }

        public int End { get; private set; }

        public SourceRange(int start, int end)
        {
            this.Start = start;
            this.End = end;
        }
    }

    [DebuggerDisplay("StartPos = (L: {Line.Start}, C: {Column.Start}), EndPos = (L: {Line.End}, C: {Column.End}), Text = {OriginalText}")]
    public class SourceExtent
    {
        public string File { get; private set; }

        public SourceRange Line { get; private set; }

        public SourceRange Column { get; private set; }

        public SourceRange Offset { get; private set; }

        public string OriginalText { get; private set; }

        public SourceExtent(
            string sourceText,
            int startOffset,
            int endOffset,
            int currentLineNumber,
            int currentColumnNumber,
            string file)
        {
            int newLineNumber = currentLineNumber;
            int newColumnNumber = currentColumnNumber;
            this.CalculatePositionFromNewlines(
                sourceText,
                startOffset,
                endOffset,
                ref newLineNumber,
                ref newColumnNumber);

            this.File = file;
            this.Offset = new SourceRange(startOffset, endOffset);
            this.Line = new SourceRange(currentLineNumber, newLineNumber);
            this.Column = new SourceRange(currentColumnNumber, newColumnNumber);
            this.OriginalText = 
                sourceText.Substring(
                    this.Offset.Start, 
                    this.Offset.End - this.Offset.Start);
        }

        private void CalculatePositionFromNewlines(
            string subString, 
            int startOffset, 
            int endOffset, 
            ref int lineNumber, 
            ref int columnNumber)
        {
            // This method assumes that lineNumber and columnNumber just need to be
            // updated because they are running counters for the current position.

            int offset;
            int lineStartOffset = startOffset;
            for (offset = startOffset; offset < endOffset; offset++)
            {
                if (subString[offset] == '\n')
                {
                    lineNumber++;

                    // Keep track of the offset number that starts this line so
                    // we can use it to calculate the new column position
                    lineStartOffset = offset + 1;
                }
            }

            // The new column position is the offset from the current line's
            // starting offset
            columnNumber += offset - lineStartOffset;
        }
    }
}
