using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using Microsoft.PowerShell.Commands;

namespace Markdown.MAML.Renderer
{
    static class RenderCleaner
    {
        public static string NormalizeWhitespaces(string text)
        {
            // non-breakable white-space to a normal one
            text = text.Replace('\uc2a0', ' ');
            return text;
        }

        public static string NormalizeQuotesAndDashes(string text)
        {
            // NOTE: quotes has left, right + low and reversed variants
            // we don't normalize a low-single one, because it could be treated as comma

            // single quote
            text = Regex.Replace(text, "\u2018|\u2019|\u201b", "\'");

            // double quote
            text = Regex.Replace(text, "\u201c|\u201d|\u201e|\u201f", "\"");

            // there are 100500 dashes types in unicode https://www.cs.tut.fi/~jkorpela/dashes.html
            // we want to normalize at least the most commonly used
            text = Regex.Replace(text, "\u05be|\u1806|\u2010|\u2011|\u00AD|\u2012|\u2013|\u2014|\u2015|\u2212", "-");

            return text;
        }

        public static string NormalizeLineBreaks(string text)
        {
            return Regex.Replace(text, "\r\n?|\n", "\r\n");
        }
    }
}
