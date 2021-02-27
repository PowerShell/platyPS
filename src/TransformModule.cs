using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Management.Automation.Runspaces;
using System.Text;
using Microsoft.PowerShell.PlatyPS.Model;

namespace Microsoft.PowerShell.PlatyPS
{
    internal class TransformModule : TransformBase
    {
        public TransformModule(PSSession session) : base(session)
        {
        }

        internal override Collection<CommandHelp> Transform(string[] moduleNames)
        {
            Collection<CommandHelp> cmdHelp = new Collection<CommandHelp>();

            return cmdHelp;
        }
    }
}