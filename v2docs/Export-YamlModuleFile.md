---
content type: cmdlet
external help file: Microsoft.PowerShell.PlatyPS.dll-Help.xml
HelpUri: ''
Locale: en-US
Module Name: Microsoft.PowerShell.PlatyPS
ms.date: 05/29/2024
PlatyPS schema version: 2024-05-01
title: Export-YamlModuleFile
---

# Export-YamlModuleFile

## SYNOPSIS

Exports a **ModuleFileInfo** object to a Yaml file.

## SYNTAX

### Default (Default)

```
Export-YamlModuleFile [-ModuleFile] <Microsoft.PowerShell.PlatyPS.ModuleFileInfo[]>
 [-Encoding <System.Text.Encoding>] [-Force] [-OutputFolder <System.String>]
 [-Metadata <System.Collections.Hashtable>] [-WhatIf] [-Confirm] [<CommonParameters>]
```

## ALIASES

## DESCRIPTION

This command exports a **ModuleFileInfo** object to a markdown file. You can get a
**ModuleFileInfo** object by using the `Import-MarkdownModuleFile` cmdlet. You can import a module
file that's written in the old format and export it to the new format.

## EXAMPLES

### Example 1 - Export a ModuleFileInfo object to a markdown file

In this example, a **ModuleFileInfo** object by importing a module Markdown file. That object is
then exported to a Yaml file using the `Export-YamlModuleFile`.

```powershell
Import-MarkdownModuleFile -Path .\v2\Microsoft.PowerShell.PlatyPS.md |
    Export-YamlModuleFile -OutputFolder .
```

```Output
    Directory: D:\Git\PS-Src\platyPS\v2docs

Mode                 LastWriteTime         Length Name
----                 -------------         ------ ----
-a---           5/20/2024  5:02 PM           1709 Microsoft.PowerShell.PlatyPS.yml
```

## PARAMETERS

### -Confirm

{{ Fill Confirm Description }}

```yaml
Type: System.Management.Automation.SwitchParameter
DefaultValue: ''
VariableLength: true
SupportsWildcards: false
ParameterValue: []
Aliases:
- cf
ParameterSets:
- Name: (All)
  Position: Named
  IsRequired: false
  ValueByPipeline: false
  ValueByPipelineByPropertyName: false
  ValueFromRemainingArguments: false
DontShow: false
AcceptedValues: []
HelpMessage: ''
```

### -Encoding

The encoding to use when writing the markdown file. If no value is specified, encoding defaults to
the value of the `$OutputEncoding` preference variable.

```yaml
Type: System.Text.Encoding
DefaultValue: ''
VariableLength: true
SupportsWildcards: false
ParameterValue: []
Aliases: []
ParameterSets:
- Name: (All)
  Position: Named
  IsRequired: false
  ValueByPipeline: false
  ValueByPipelineByPropertyName: false
  ValueFromRemainingArguments: false
DontShow: false
AcceptedValues: []
HelpMessage: ''
```

### -Force

Use the **Force** parameter to overwrite the output file if it already exists.

```yaml
Type: System.Management.Automation.SwitchParameter
DefaultValue: ''
VariableLength: true
SupportsWildcards: false
ParameterValue: []
Aliases: []
ParameterSets:
- Name: (All)
  Position: Named
  IsRequired: false
  ValueByPipeline: false
  ValueByPipelineByPropertyName: false
  ValueFromRemainingArguments: false
DontShow: false
AcceptedValues: []
HelpMessage: ''
```

### -Metadata

{{ Fill Metadata Description }}

```yaml
Type: System.Collections.Hashtable
DefaultValue: ''
VariableLength: true
SupportsWildcards: false
ParameterValue: []
Aliases: []
ParameterSets:
- Name: (All)
  Position: Named
  IsRequired: false
  ValueByPipeline: false
  ValueByPipelineByPropertyName: false
  ValueFromRemainingArguments: false
DontShow: false
AcceptedValues: []
HelpMessage: ''
```

### -ModuleFile

The **ModuleFileInfo** object to export. You can pass the **ModuleFileInfo** object on the pipeline
or by using the **ModuleFile** parameter.

```yaml
Type: Microsoft.PowerShell.PlatyPS.ModuleFileInfo[]
DefaultValue: ''
VariableLength: true
SupportsWildcards: false
ParameterValue: []
Aliases: []
ParameterSets:
- Name: (All)
  Position: 0
  IsRequired: true
  ValueByPipeline: true
  ValueByPipelineByPropertyName: false
  ValueFromRemainingArguments: false
DontShow: false
AcceptedValues: []
HelpMessage: ''
```

### -OutputFolder

The **ModuleFileInfo** object to export to a markdown file. This object is created by the
`Import-MarkdownModuleFile` cmdlet.

```yaml
Type: System.String
DefaultValue: ''
VariableLength: true
SupportsWildcards: false
ParameterValue: []
Aliases: []
ParameterSets:
- Name: (All)
  Position: Named
  IsRequired: false
  ValueByPipeline: false
  ValueByPipelineByPropertyName: false
  ValueFromRemainingArguments: false
DontShow: false
AcceptedValues: []
HelpMessage: ''
```

### -WhatIf

{{ Fill WhatIf Description }}

```yaml
Type: System.Management.Automation.SwitchParameter
DefaultValue: ''
VariableLength: true
SupportsWildcards: false
ParameterValue: []
Aliases:
- wi
ParameterSets:
- Name: (All)
  Position: Named
  IsRequired: false
  ValueByPipeline: false
  ValueByPipelineByPropertyName: false
  ValueFromRemainingArguments: false
DontShow: false
AcceptedValues: []
HelpMessage: ''
```

### CommonParameters

This cmdlet supports the common parameters: -Debug, -ErrorAction, -ErrorVariable,
-InformationAction, -InformationVariable, -OutBuffer, -OutVariable, -PipelineVariable,
-ProgressAction, -Verbose, -WarningAction, -WarningVariable. For more information, see
[about_CommonParameters](https://go.microsoft.com/fwlink/?LinkID=113216).

## INPUTS

### Microsoft.PowerShell.PlatyPS.ModuleFileInfo

## OUTPUTS

### System.IO.FileInfo

## NOTES

## RELATED LINKS

- [Export-MarkdownModuleFile](Export-MarkdownModuleFile.md)
- [Import-MarkdownModuleFile](Import-MarkdownModuleFile.md)
