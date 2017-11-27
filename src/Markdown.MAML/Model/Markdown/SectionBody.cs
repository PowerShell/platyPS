﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Markdown.MAML.Model.Markdown
{
    /// <summary>
    /// A section of text with formatting options.
    /// </summary>
    public sealed class SectionBody
    {
        public SectionBody()
        {
            FormatOption = SectionFormatOption.None;
        }

        public SectionBody(string text, SectionFormatOption formatOption)
        {
            Text = text;
            FormatOption = formatOption;
        }

        /// <summary>
        /// The text of the section body.
        /// </summary>
        public string Text { get; private set; }

        /// <summary>
        /// Format options that control markdown generation.
        /// </summary>
        public SectionFormatOption FormatOption { get; private set; }

        public override string ToString()
        {
            return Text;
        }

        public static SectionBody New(string text, SectionFormatOption formatOption = SectionFormatOption.None)
        {
            return new SectionBody(text, formatOption);
        }

        public static implicit operator SectionBody(string text)
        {
            return new SectionBody(text, SectionFormatOption.None);
        }
    }
}
