// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections;
using System.Collections.Generic;

namespace Microsoft.PowerShell.PlatyPS.Model
{
    public class Diagnostics
    {
        /// <summary>
        /// The name of the file to associate with the messages
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// Whether any of the diagnostics messages are errors.
        /// </summary>
        public bool HadErrors { get; set; }

        public List<DiagnosticMessage> Messages { get; set; }
        public Diagnostics()
        {
            FileName = string.Empty;
            Messages = new List<DiagnosticMessage>();
            HadErrors = false;
        }   

        public Diagnostics(string fileName)
        {
            FileName = fileName;
            Messages = new List<DiagnosticMessage>();
            HadErrors = false;
        }

        public bool TryAddDiagnostic(DiagnosticMessage message)
        {
            try
            {
                Messages.Add(message);
                if (message.Severity == DiagnosticSeverity.Error)
                {
                    HadErrors = true;
                }
                return true;
            }
            catch // Any exception is a failure to add the message
            {
                return false;
            }
        }   
        public bool TryAddDiagnostic(DiagnosticMessageSource source, string message, DiagnosticSeverity severity, string identifier, int line)
        {
            try
            {
                var diagnostic = new DiagnosticMessage(source, message, severity, identifier, line);
                Messages.Add(diagnostic);
                if (diagnostic.Severity == DiagnosticSeverity.Error)
                {
                    HadErrors = true;
                }
                return true;
            }
            catch // Any exception is a failure to add the message
            {
                return false;
            }   
        }
    }
}    