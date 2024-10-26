---
document type: cmdlet
external help file: Microsoft.PowerShell.PlatyPS.dll-Help.xml
HelpUri: ''
Locale: en-US
Module Name: Microsoft.PowerShell.PlatyPS
ms.custom: OPS13
ms.date: 10/25/2024
PlatyPS schema version: 2024-05-01
title: Export-YamlModuleFile
---

# Export-YamlModuleFile

## SYNOPSIS

Exports a **ModuleFileInfo** object to a Yaml file.

## SYNTAX

### __AllParameterSets

```
Export-YamlModuleFile [-ModuleFileInfo] <ModuleFileInfo[]> [-Encoding <Encoding>] [-Force]
 [-OutputFolder <string>] [-Metadata <hashtable>] [-WhatIf] [-Confirm] [<CommonParameters>]
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
Import-MarkdownModuleFile -Path .\v2\Microsoft.PowerShell.PlatyPS\Microsoft.PowerShell.PlatyPS.md |
    Export-YamlModuleFile -OutputFolder .\v2\yaml
```

```Output
    Directory: D:\Git\PS-Src\platyPS\v2docs\v2\yaml

Mode                 LastWriteTime         Length Name
----                 -------------         ------ ----
-a---           8/26/2024  4:00 PM           2594 Microsoft.PowerShell.PlatyPS.yml
```

## PARAMETERS

### -Confirm

Prompts you for confirmation before running the cmdlet.

```yaml
Type: System.Management.Automation.SwitchParameter
DefaultValue: ''
SupportsWildcards: false
ParameterValue: []
Aliases:
- cf
ParameterSets:
- Name: (All)
  Position: Named
  IsRequired: false
  ValueFromPipeline: false
  ValueFromPipelineByPropertyName: false
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
SupportsWildcards: false
ParameterValue: []
Aliases: []
ParameterSets:
- Name: (All)
  Position: Named
  IsRequired: false
  ValueFromPipeline: false
  ValueFromPipelineByPropertyName: false
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
SupportsWildcards: false
ParameterValue: []
Aliases: []
ParameterSets:
- Name: (All)
  Position: Named
  IsRequired: false
  ValueFromPipeline: false
  ValueFromPipelineByPropertyName: false
  ValueFromRemainingArguments: false
DontShow: false
AcceptedValues: []
HelpMessage: ''
```

### -Metadata

The metadata to add to the markdown file. The metadata is a hashtable that contains key-value pairs.

```yaml
Type: System.Collections.Hashtable
DefaultValue: ''
SupportsWildcards: false
ParameterValue: []
Aliases: []
ParameterSets:
- Name: (All)
  Position: Named
  IsRequired: false
  ValueFromPipeline: false
  ValueFromPipelineByPropertyName: false
  ValueFromRemainingArguments: false
DontShow: false
AcceptedValues: []
HelpMessage: ''
```

### -ModuleFileInfo

The **ModuleFileInfo** object to export. You can pass the **ModuleFileInfo** object on the pipeline
or by using the **ModuleFileInfo** parameter.

```yaml
Type: Microsoft.PowerShell.PlatyPS.ModuleFileInfo[]
DefaultValue: ''
SupportsWildcards: false
ParameterValue: []
Aliases: []
ParameterSets:
- Name: (All)
  Position: 0
  IsRequired: true
  ValueFromPipeline: true
  ValueFromPipelineByPropertyName: false
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
SupportsWildcards: false
ParameterValue: []
Aliases: []
ParameterSets:
- Name: (All)
  Position: Named
  IsRequired: false
  ValueFromPipeline: false
  ValueFromPipelineByPropertyName: false
  ValueFromRemainingArguments: false
DontShow: false
AcceptedValues: []
HelpMessage: ''
```

### -WhatIf

Shows what would happen if the cmdlet runs. The cmdlet isn't run.

```yaml
Type: System.Management.Automation.SwitchParameter
DefaultValue: ''
SupportsWildcards: false
ParameterValue: []
Aliases:
- wi
ParameterSets:
- Name: (All)
  Position: Named
  IsRequired: false
  ValueFromPipeline: false
  ValueFromPipelineByPropertyName: false
  ValueFromRemainingArguments: false
DontShow: false
AcceptedValues: []
HelpMessage: ''
```

### CommonParameters

This cmdlet supports the common parameters: -Debug, -ErrorAction, -ErrorVariable,
-InformationAction, -InformationVariable, -OutBuffer, -OutVariable, -PipelineVariable,
-ProgressAction, -Verbose, -WarningAction, and -WarningVariable. For more information, see
[about_CommonParameters](https://go.microsoft.com/fwlink/?LinkID=113216).

## INPUTS

### Microsoft.PowerShell.PlatyPS.ModuleFileInfo

## OUTPUTS

### System.IO.FileInfo

## NOTES

## RELATED LINKS

- [Export-MarkdownModuleFile](Export-MarkdownModuleFile.md)
- [Import-MarkdownModuleFile](Import-MarkdownModuleFile.md)
