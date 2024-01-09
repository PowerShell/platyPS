// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Text;

namespace Microsoft.PowerShell.PlatyPS
{
    public class ParagraphFormatSettings
    {
        /// <summary>
        /// The maximum length of a line in the paragraph.
        /// This includes indentation.
        /// </summary>
        public ushort LineLength { get; set; }
        
        /// <summary>
        /// How much indentation to add to each line.
        /// </summary>
        public byte Indentation { get; set; }

        /// <summary>
        /// how much indentation to add to the first line.
        /// </summary>
        public byte FirstLineIndentation { get; set; }

        /// <summary>
        /// The default settings for a paragraph arg:
        /// LineLength = 80
        /// No indentation
        /// No first line indentation
        /// </summary>
        public ParagraphFormatSettings()
        {
            LineLength = 80;
            Indentation = 0;
            FirstLineIndentation = 0;
        }

        public ParagraphFormatSettings(ushort lineLength, byte indentation, byte firstLineIndentation)
        {
            LineLength = lineLength;
            Indentation = indentation;
            FirstLineIndentation = firstLineIndentation;
        }   
    }

    /// <summary>
    /// Helper class to format a string into a paragraph.
    /// It can also indent the paragraph, and add extra indentation to the first line.
    /// </summary>
    public class ParagraphFormatHelper
    {
        public static string FormatParagraph(string input, ParagraphFormatSettings settings)
        {
            StringBuilder outputLine = new StringBuilder(); // used for formatting the entire paragraph
            StringBuilder currentLine = new StringBuilder(); // used for the current line


            if (settings.Indentation > 0)
            {
                currentLine.Append(new string(' ', settings.Indentation));
            }

            if (settings.FirstLineIndentation > 0)
            {
                currentLine.Append(new string(' ', settings.FirstLineIndentation));
            }

            string[] words = input.Split(new char[]{' '}, StringSplitOptions.RemoveEmptyEntries);
            foreach(string word in words)
            {
                if (currentLine.Length + word.Length + 1 > settings.LineLength)
                {
                    outputLine.AppendLine(currentLine.ToString().TrimEnd());
                    currentLine.Clear();
                    currentLine.Append(new string(' ', settings.Indentation));
                }

                currentLine.Append(word);
                currentLine.Append(' ');
            }   

            // Only add the last line if it's not empty
            if (currentLine.Length > 0)
            {
                outputLine.Append(currentLine.ToString().TrimEnd());
            }
            return outputLine.ToString();
        }

        /// <summary>
        /// Take a string array and join it into a single string, then format it into a paragraph.
        /// </summary>
        /// <param name="strings"></param>
        /// <param name="settings"></param>
        /// <returns></returns>
        public static string FormatParagraph(string[] strings, ParagraphFormatSettings settings)
        {
            return FormatParagraph(string.Join(" ", strings), settings);
        }
    }
}
