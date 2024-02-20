// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;

namespace Microsoft.PowerShell.PlatyPS.Model
{
    /// <summary>
    /// Model for representing data for help of a command.
    /// </summary>
    public partial class CommandHelp : IEquatable<CommandHelp>
    {
        public bool Equals(CommandHelp other)
        {
            if (other is null)
                return false;

            // if both null, return false
            if (string.Compare(this?.Description, other?.Description, StringComparison.CurrentCulture) != 0)
            {
                return false;
            }

            // if other Description is null, return false
            if (string.Compare(this.Description, other?.Description, StringComparison.CurrentCulture) != 0)
            {
                return false;
            }

            // if other ExternalHelpFile is null, return false
            if (string.Compare(this.ExternalHelpFile, other?.ExternalHelpFile, StringComparison.CurrentCulture) != 0)
            {
                return false;
            }

            if (this.HasCmdletBinding != other?.HasCmdletBinding)
            {
                return false;
            }

            if (this.HasWorkflowCommonParameters != other?.HasWorkflowCommonParameters)
            {
                return false;
            }

            // Locale
            if (this.Locale != other?.Locale)
            {
                return false;
            }

            // ModuleGuid
            if (this.ModuleGuid != other?.ModuleGuid)
            {
                return false;
            }

            // ModuleName
            if (string.Compare(this.ModuleName, other?.ModuleName, StringComparison.CurrentCulture) != 0)
            {
                return false;
            }

            // Notes
            if (string.Compare(this.Notes, other?.Notes, StringComparison.CurrentCulture) != 0)
            {
                return false;
            }

            // OnlineVersionUrl
            if (string.Compare(this.OnlineVersionUrl, other?.OnlineVersionUrl, StringComparison.CurrentCulture) != 0)
            {
                return false;
            }

            // SchemaVersion
            if (string.Compare(this.SchemaVersion, other?.SchemaVersion, StringComparison.CurrentCulture) != 0)
            {
                return false;
            }

            // Synopsis
            if (string.Compare(this.Synopsis, other?.Synopsis, StringComparison.CurrentCulture) != 0)
            {
                return false;
            }

            // Title
            if (string.Compare(this.Title, other?.Title, StringComparison.CurrentCulture) != 0)
            {
                return false;
            }

            if (this.Aliases is not null && other?.Aliases is not null && this.Aliases.SequenceEqual(other.Aliases) == false)
            {
                return false;
            }
            else if (
                (this.Aliases is null && other?.Aliases is not null && other.Aliases.Count != 0) ||
                (this.Aliases is not null && this.Aliases.Count != 0 && other?.Aliases is null)
                )
            {
                return false;
            }

            if (this.Examples is not null && this.Examples.SequenceEqual(other?.Examples) == false)
            {
                return false;
            }

            if (this.Syntax is not null && this.Syntax.SequenceEqual(other?.Syntax) == false)
            {
                return false;
            }

            if (this.Parameters is not null && this.Parameters.SequenceEqual(other?.Parameters) == false)
            {
                return false;
            }

            if (this.RelatedLinks is not null && this.RelatedLinks.SequenceEqual(other?.RelatedLinks) == false)
            {
                return false;
            }

            if (!CompareMetadata(this?.Metadata, other?.Metadata))
            {
                return false;
            }

            if (this.Inputs is not null && this.Inputs.SequenceEqual(other?.Inputs) == false)
            {
                return false;
            }

            if (this.Outputs is not null && this.Outputs.SequenceEqual(other?.Outputs) == false)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Compare two ordered dictionaries.
        /// </summary>
        /// <param name="metadata1">Metadata from a CommandHelp object</param>
        /// <param name="metadata2">Metadata from another CommandHelp object</param>
        /// <returns>bool</returns>
        private bool CompareMetadata(OrderedDictionary? metadata1, OrderedDictionary? metadata2)
        {
            if (metadata1 is null || metadata2 is null)
            {
                return false;
            }

            if (metadata1.Count != metadata2.Count)
            {
                return false;
            }

            for (int i = 0; i < metadata1.Count; i++)
            {
                var key1 = metadata1.Cast<DictionaryEntry>().ElementAt(i).Key;
                var value1 = metadata1.Cast<DictionaryEntry>().ElementAt(i).Value;
                var key2 = metadata2.Cast<DictionaryEntry>().ElementAt(i).Key;
                var value2 = metadata2.Cast<DictionaryEntry>().ElementAt(i).Value;

                if (!key1.Equals(key2) || !value1.Equals(value2))
                {
                    return false;
                }
            }

            return true;
        }

        public override bool Equals(Object obj)
        {
            if (obj is null)
                return false;

            CommandHelp? commandHelpObj = obj as CommandHelp;
            if (commandHelpObj is null)
                return false;
            else
                return Equals(commandHelpObj);
        }

        /// <summary>
        /// Override the GetHashCode function.
        /// </summary>
        /// <returns>An integer representing the hash.</returns>
        public override int GetHashCode()
        {
            return this.GetHashCode();
        }

        /// <summary>
        /// Override the == operator.
        /// </summary>
        /// <param name="commandHelp1"></param>
        /// <param name="commandHelp2"></param>
        /// <returns></returns>
        public static bool operator == (CommandHelp commandHelp1, CommandHelp commandHelp2)
        {
            if (commandHelp1 is not null && commandHelp2 is not null)
            {
                return commandHelp1.Equals(commandHelp2);
            }

            return false;
        }

        public static bool operator != (CommandHelp commandHelp1, CommandHelp commandHelp2)
        {
            if (commandHelp1 is not null && commandHelp2 is not null)
            {
                return ! commandHelp1.Equals(commandHelp2);
            }

            return false;
        }

        /// <summary>
        /// Compare two lists of strings.
        /// These lists must be in the same order as they represent documentation which has a specific order.
        /// </summary>
        /// <param name="list1">a list of strings.</param>
        /// <param name="list2">a list of strings.</param>
        /// <returns>boolean</returns>
        public static bool CompareLists(List<string> list1, List<string> list2)
        {
            return list1.SequenceEqual(list2);
        }
    }
}
