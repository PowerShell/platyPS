using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Markdown.MAML.Parser
{
    public class HelpSchemaException : ArgumentException
    {
        public HelpSchemaException(string message) : base(message)
        {
        }
    }
}
