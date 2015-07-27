using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Markdown.MAML.Model;

namespace Markdown.MAML.Parser
{
    /// <summary>
    /// This class contain logic to render maml from the Model.
    /// </summary>
    public class MamlRenderer
    {
        private StringBuilder _stringBuilder = new StringBuilder();

        private const string XML_PREAMBULA =
            @"<command:command xmlns:maml=""http://schemas.microsoft.com/maml/2004/10"" xmlns:command=""http://schemas.microsoft.com/maml/dev/command/2004/10"" xmlns:dev=""http://schemas.microsoft.com/maml/dev/2004/10"" xmlns:MSHelp=""http://msdn.microsoft.com/mshelp"">";

        public MamlRenderer(DocumentNode root)
        {
            
        }

        public string ToMamlString()
        {
            _stringBuilder.Clear();
            _stringBuilder.Append(XML_PREAMBULA);
            return _stringBuilder.ToString();
        }
    }
}
