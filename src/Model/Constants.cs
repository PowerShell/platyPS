// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Microsoft.PowerShell.PlatyPS.Model
{
    internal class ConstantsHelper
    {
        internal static string GetCommonParametersMessage()
        {
            List<string> paramWithDash = new();
            foreach (string param in Constants.CommonParametersNames)
            {
                paramWithDash.Add($"-{param}");
            }

            return
                ParagraphFormatHelper.FormatParagraph(
                    string.Format(Constants.CommonParametersFmt, string.Join(", ", paramWithDash)),
                    new ParagraphFormatSettings(){LineLength = 100}
                );
        }
    }

    internal static partial class Constants
    {
        internal static readonly List<string> EmptyStringList = new();

        internal static readonly char DirectorySeparator = System.IO.Path.DirectorySeparatorChar;
        internal static readonly StringBuilderPool StringBuilderPool = new StringBuilderPool();
        internal static readonly char[] LineSplitter = new char[] { '\r', '\n' };
        internal static readonly char[] Comma = new char[] { ',' };
        internal static readonly string WorkflowCommonParametersMessage = "This cmdlet also supports workflow specific common parameters.\nFor information, see [about_WorkflowCommonParameters](../PSWorkflow/About/about_WorkflowCommonParameters.md).";

        internal const string CommonParametersFmt = "This cmdlet supports the common parameters: {0}.\nFor more information, see [about_CommonParameters](https://go.microsoft.com/fwlink/?LinkID=113216).";
        internal const string RequiredParamTemplate = "-{0} <{1}>";
        internal const string OptionalParamTemplate = "[-{0} <{1}>]";
        internal const string RequiredPositionalParamTemplate = "[-{0}] <{1}>";
        internal const string OptionalPositionalParamTemplate = "[[-{0} <{1}>]]";
        internal const string OptionalSwitchParamTemplate = "[-{0}]";
        internal const string RequiredSwitchParamTemplate = "-{0}";
        internal const string RequiredParameterSetsTemplate = "True ({0}) False ({1})";

        internal const char   SingleSpace = ' ';
        internal const string TrueString = "True";
        internal const string FalseString = "False";
        internal const string NoneString = "None";
        internal const string NamedString = "Named";
        internal const string ParameterSetsAllName = "__AllParameterSets";
        internal const string DefaultString = "(Default)";
        internal const string ParameterSetsAll = "(All)";
        internal const string ParameterSetsAllForRequired = "All";
        internal const string DelimiterComma = ", ";
        internal const string SystemObjectTypename = "System.Object";
        internal const string SyntaxCommonParameters = "[<CommonParameters>]";

        internal const string FillInDescription = "{{ Fill in the Description }}";
        internal const string FillInSynopsis = "{{ Fill in the Synopsis }}";
        internal const string FillInNotes= "{{ Fill in the Notes }}";
        internal const string FillInExampleTitle = "Example 1";
        internal const string FillInExampleCode = @"{{ Add example code here }}";
        internal const string FillInExampleDescription = "{{ Add example description here }}";
        internal const string FillInParameterDescriptionTemplate = "{{{{ Fill {0} Description }}}}";
        internal const string FillInRelatedLinks = "{{ Fill in the related links here }}";
        internal const string UnnamedParameterSetTemplate = "UNNAMED_PARAMETER_SET_{0}";
        internal const string FillDownloadHelpLink = "{{ Update Download Link}}";
        internal const string FillHelpVersion = "{{ Please enter version of help manually (X.X.X.X) format }}";
        internal const string FillInGuid = "XXXXXXXXX-XXXX-XXXX-XXXX-XXXXXXXXXXXX";
        internal const string LocaleEnUs = "en-US";

        // TODO: ProgressAction is not a common parameter for all versions of PowerShell.
        //  This should not be added under all circumstances.
        internal static SortedSet<string> CommonParametersNames = new SortedSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "Debug",
            "ErrorAction",
            "ErrorVariable",
            "InformationAction",
            "InformationVariable",
            "OutBuffer",
            "OutVariable",
            "PipelineVariable",
            "ProgressAction",
            "Verbose",
            "WarningAction",
            "WarningVariable"
        };

    }
}
