// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

  /// <summary>
  ///    The diagnostic message source; Something that identifies the element to which diagnostic message pertains.
  /// </summary>
  public enum DiagnosticMessageSource
    {
        Metadata,
        Synopsis,
        Syntax,
        Alias,
        Description,
        Example,
        Parameter,
        Inputs,
        Outputs,
        Notes,
        Links,
        General,
        Merge,
        ModuleFileTitle,
        ModuleFileDescription,
        ModuleFileGroup,
        ModuleFileCommand
    }