// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Management.Automation;
using Microsoft.PowerShell.PlatyPS.Model;
using System.Linq;

namespace Microsoft.PowerShell.PlatyPS
{
    /// <summary>
    /// Cmdlet to import a markdown file and convert it to a CommandHelp object.
    /// </summary>

    [Cmdlet(VerbsData.Compare, "CommandHelp")]
    [OutputType(typeof(System.String[]))]
    public class CompareCommandHelpCommand : PSCmdlet
    {
        /// <summary>
        /// The reference for comparison.
        /// </summary>
        [Parameter(Position = 0, Mandatory = true, ValueFromPipelineByPropertyName = true)]
        public CommandHelp? Reference { get; set; }

        [Parameter(Position = 1, Mandatory = true, ValueFromPipelineByPropertyName = true)]
        public CommandHelp? Difference { get; set; }

        [Parameter]
        public string[] PropertyNamesToExclude { get; set; } = new string[0];

        List<String> DiagnosticMessages = new();

        List<string> Differences = new();
        bool HadDifferences;

        protected override void ProcessRecord()
        {
            if (Reference is null)
            {
                WriteError(new ErrorRecord(new ArgumentNullException("Reference object may not be null"), "errorId", ErrorCategory.InvalidData, null));
                return;
            }

            if (Difference is null)
            {
                WriteError(new ErrorRecord(new ArgumentNullException("Difference object may not be null"), "errorId", ErrorCategory.InvalidData, null));
                return;
            }


            var refObject = new PSObject((CommandHelp)Reference);
            var difObject = new PSObject((CommandHelp)Difference);

            CompareProperty(refObject, difObject, 0, Reference.GetType().Name);

            WriteObject(DiagnosticMessages, true);
            DiagnosticMessages.Clear();
        }

        protected override void EndProcessing()
        {
            WriteObject(string.Format("Comparison result: '{0}'", HadDifferences ? "NOT OK" : "OK"));
        }

        private void CompareProperty(PSObject refObject, PSObject difObject, int offset, string objectPath)
        {
            var spaces = new string(' ', offset);
            foreach (var property in refObject.Properties)
            {
                if (PropertyNamesToExclude.Any(n => string.Compare(n, property.Name, true) == 0))
                {
                    DiagnosticMessages.Add($"M excluding comparison of {objectPath}.{property.Name}");
                    continue;
                }

                if (property.Value is String || ! (property.Value is IEnumerable))
                {
                    var difProperty = difObject.Properties.Where(p => string.Compare(property.Name, p.Name) == 0).First();
                    if (LanguagePrimitives.Equals(difProperty.Value, property.Value))
                    {
                        var vString = property.Value is null ? string.Empty : property.Value.ToString();
                        var val = vString.Length > 20 ? vString.Substring(0,17) : vString;
                        DiagnosticMessages.Add($"S {spaces}{objectPath}.{property.Name} are the same ({val})");
                    }
                    else
                    {
                        DiagnosticMessages.Add($"D {spaces}{objectPath}.{property.Name} are not the same '{property.Value}' vs '{difProperty.Value}'");
                        HadDifferences = true;
                    }
                }
                else if (property.Value is IDictionary)
                {
                    DiagnosticMessages.Add($"M Inspecting dictionary {objectPath}.{property.Name}");
                    var rDictionary = (IDictionary)property.Value;
                    var dDictionary = (IDictionary)difObject.Properties.Where(p => string.Compare(property.Name, p.Name) == 0).First().Value;

                    foreach(var key in dDictionary.Keys)
                    {
                        if (!rDictionary.Contains(key))
                        {
                            DiagnosticMessages.Add($"D {spaces}  {objectPath}.{property.Name}: {key} does not exist in reference");
                            HadDifferences = true;
                        }
                    }

                    foreach (var key in rDictionary.Keys)
                    {
                        if (LanguagePrimitives.Equals(rDictionary[key], dDictionary[key]))
                        {
                            DiagnosticMessages.Add($"S {spaces}  {objectPath}.{property.Name}: {key} are the same ({rDictionary[key]})");
                        }
                        else
                        {
                            DiagnosticMessages.Add($"D {spaces}  {objectPath}.{property.Name}: {key} not the same {rDictionary[key]} vs {dDictionary[key]}");
                            HadDifferences = true;
                        }
                    }

                }
                else if (property.Value is IList rList)
                {
                    DiagnosticMessages.Add($"M Inspecting list {objectPath}.{property.Name}");
                    var dList = (IList)difObject.Properties.Where(p => string.Compare(property.Name, p.Name) == 0).First().Value;
                    if (rList.Count == 0 && dList.Count == 0)
                    {
                        DiagnosticMessages.Add($"S {spaces}{objectPath}.{property.Name} lists are empty");
                    }
                    else if (rList.Count != dList.Count)
                    {
                        DiagnosticMessages.Add($"D {spaces}{objectPath}.{property.Name} lists are different sizes ({rList.Count} vs {dList.Count})");
                        HadDifferences = true;
                    }
                    else
                    {
                        for (int i = 0; i < rList.Count; i++)
                        {
                            var rPSO = new PSObject(rList[i]);
                            var dPSO = new PSObject(dList[i]);
                            CompareProperty(rPSO, dPSO, offset + 2, $"{objectPath}.{property.Name}");
                        }
                    }
                }
                else if (property.Value is Array rArray)
                {
                    DiagnosticMessages.Add($"M Inspecting array {objectPath}.{property.Name}");
                    var dArray = (Array)difObject.Properties.Where(p => string.Compare(property.Name, p.Name) == 0).First().Value;
                    if (rArray.Length == 0 && dArray.Length == 0)
                    {
                        DiagnosticMessages.Add($"S {spaces}Arrays are empty");
                    }
                    else if (rArray.Length != dArray.Length)
                    {
                        DiagnosticMessages.Add($"D {spaces}Arrays are different sizes ({rArray.Length} vs {dArray.Length})");
                    }
                    else
                    {
                        for (int i = 0; i < rArray.Length; i++)
                        {
                            var rPSO = new PSObject(rArray.GetValue(i));
                            var dPSO = new PSObject(dArray.GetValue(i));
                            CompareProperty(rPSO, dPSO, offset + 2, "{objectPath}.{property.Name}");
                        }
                    }
                }
                else
                {
                    DiagnosticMessages.Add($"M {spaces}Skipping {objectPath}.{property.Name}");
                }
            }
        }
    }
}