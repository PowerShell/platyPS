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
            text = Regex.Replace(text, "Â ", " ");
            return text;
        }

        public static string NormalizeLineBreaks(string text)
        {
            return Regex.Replace(text, "\r\n?|\n", "\r\n");
        }
    }
}
