using System;

namespace Markdown.MAML.Model.Markdown
{
    /// <summary>
    /// Define options that determine how sections will be formated when rendering markdown.
    /// </summary>
    [Flags()]
    public enum SectionFormatOption : byte
    {
        None = 0,

        /// <summary>
        /// A line break should be added after the section header.
        /// </summary>
        LineBreakAfterHeader = 1
    }
}
