 // Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;

namespace Microsoft.PowerShell.PlatyPS
{
    public class MarkdownCommandHelpValidationResult
    {
        public string Path { get; set; }
        public bool IsValid { get; set; }
        public List<string> Messages { get; set; }
        public MarkdownCommandHelpValidationResult()
        {
            Path = string.Empty;
            IsValid = true;
            Messages = new List<string>();
        }

        public MarkdownCommandHelpValidationResult(string path, bool isValid, List<string> messages)
        {
            Path = path;
            IsValid = isValid;
            Messages = messages;
        }

        public override string ToString()
        {
            return $"Path: {Path}, IsValid: {IsValid}";
        }

    }
}