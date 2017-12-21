using System;
using Markdown.MAML.Renderer;
using Xunit;

namespace Markdown.MAML.Test
{
    public static class Common
    {
        public static void AssertMultilineEqual(string expected, string actual)
        {
            Assert.Equal(RenderCleaner.NormalizeLineBreaks(expected),
                         RenderCleaner.NormalizeLineBreaks(actual));
        }
    }
}
