// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.PowerShell.PlatyPS
{
    public class ModuleCommandGroup : IEquatable<ModuleCommandGroup>
    {
        public string GroupTitle { get; set; }
        public List<ModuleCommandInfo> Commands { get; set; }

        public ModuleCommandGroup()
        {
            GroupTitle = string.Empty;
            Commands = new();
        }

        public ModuleCommandGroup(ModuleCommandGroup mcg)
        {
            GroupTitle = mcg.GroupTitle;
            Commands = new();
            foreach(var command in mcg.Commands)
            {
                Commands.Add(new ModuleCommandInfo(command));
            }
        }

        public ModuleCommandGroup(string title)
        {
            GroupTitle = title;
            Commands = new();
        }

       public override int GetHashCode()
        {
            return (GroupTitle, Commands).GetHashCode();
        }

        public bool Equals(ModuleCommandGroup other)
        {
            if (other is null)
            {
                return false;
            }

            return (
                string.Compare(GroupTitle, other.GroupTitle, StringComparison.CurrentCultureIgnoreCase) == 0 &&
                Commands.SequenceEqual(other.Commands)
                );
        }

        public override bool Equals(object other)
        {
            if (other is null)
            {
                return false;
            }

            if (other is ModuleCommandGroup group2)
            {
                return Equals(group2);
            }

            return false;
        }

        public static bool operator == (ModuleCommandGroup group1, ModuleCommandGroup group2)
        {
            if (group1 is not null && group2 is not null)
            {
                return group1.Equals(group2);
            }

            return false;
        }

        public static bool operator !=(ModuleCommandGroup group1, ModuleCommandGroup group2)
        {
            if (group1 is not null && group2 is not null)
            {
                return ! group1.Equals(group2);
            }

            return false;
        }
    }
}
