using System.Text.RegularExpressions;

namespace Markdown.MAML.Renderer
{
    internal static class RenderCleaner
    {
        /// <summary>
        /// This method runs all of the normalization tools in the cleaner. This allows full normalization without needing to call
        /// each and every method on a string.
        /// </summary>
        /// <param name="text">The string that needs to be cleaned.</param>
        /// <returns>Normalizaed string: WhiteSpaces, Quotes and Dashes, Linebreaks</returns>
        public static string FullNormalization(string text)
        {
            return NormalizeQuotesAndDashes(
                        NormalizeLineBreaks(
                            NormalizeWhitespaces(text)));
        }

        public static string NormalizeWhitespaces(string text)
        {
            // there are many spaces
            // https://www.cs.tut.fi/~jkorpela/chars/spaces.html
            // we interested in just a few of them.
            // Also, we use en-space \uc2a0 for tabulation inside NOTES list, when we convert them into lists.
            // it's a hack, this characte is not a whitespace, but that's how we work-around help engine limitations.
            text = text
                // non-breakable white-space to a normal one
                .Replace('\u00a0', ' ')
                .Replace('\uc2a0', ' ')
                // also try to clean up these ACSII characters
                .Replace("Â ", " ")
                .Replace('Ã', ' ')
            ;

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
