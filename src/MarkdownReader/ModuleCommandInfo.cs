// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;

namespace Microsoft.PowerShell.PlatyPS
{
    public class ModuleCommandInfo : IEquatable<ModuleCommandInfo>
    {
        public string Name { get; set; }
        public string Link { get; set; }
        public string Description { get; set; }
        public ModuleCommandInfo()
        {
            Name = string.Empty;
            Link = string.Empty;
            Description = string.Empty;
        }

        public ModuleCommandInfo(ModuleCommandInfo mci)
        {
            Name = mci.Name;
            Link = mci.Link;
            Description = mci.Description;
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode() ^ Link.GetHashCode() ^ Description.GetHashCode();
        }

        public bool Equals(ModuleCommandInfo other)
        {
            if (other is null)
            {
                return false;
            }

            return string.Compare(Name, other.Name) == 0 &&
                string.Compare(Link, other.Link) == 0 &&
                string.Compare(Description, other.Description) == 0;
        }

        public override bool Equals(object other)
        {
            if (other is null)
            {
                return false;
            }

            if (other is ModuleCommandInfo info2)
            {
                return Equals(info2);
            }

            return false;
        }

        public static bool operator == (ModuleCommandInfo info1, ModuleCommandInfo info2)
        {
            if (info1 is not null && info2 is not null)
            {
                return info1.Equals(info2);
            }

            return false;
        }

        public static bool operator !=(ModuleCommandInfo info1, ModuleCommandInfo info2)
        {
            if (info1 is not null && info2 is not null)
            {
                return ! info1.Equals(info2);
            }

            return false;
        }
    }
}
