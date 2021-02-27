using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Management.Automation.Runspaces;
using System.Text;
using System.Threading.Tasks;
using Microsoft.PowerShell.PlatyPS.Model;

namespace Microsoft.PowerShell.PlatyPS
{
    internal class TransformMaml : TransformBase
    {
        public TransformMaml(PSSession session) : base(session)
        {
        }

        internal override Collection<CommandHelp> Transform(string[] mamlFileNames)
        {
            throw new NotImplementedException();
        }
    }
}
