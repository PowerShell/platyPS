using System;

namespace Markdown.MAML.Transformer
{
    public class HelpSchemaException : ArgumentException
    {
        public HelpSchemaException(string message) : base(message)
        {
        }
    }
}
