using System;
using System.Collections.ObjectModel;
using System.Management.Automation.Runspaces;
using Microsoft.PowerShell.PlatyPS.Model;

namespace Microsoft.PowerShell.PlatyPS
{
    internal abstract class TransformBase
    {
        protected PSSession Session { get; set; }

        public TransformBase(PSSession session) => Session = session;

        internal abstract Collection<CommandHelp> Transform(string[] source);
    }
}