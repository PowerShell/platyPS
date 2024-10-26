---
document type: cmdlet
external help file: Microsoft.PowerShell.PlatyPS.dll-Help.xml
HelpUri: ''
Locale: en-US
Module Name: Microsoft.PowerShell.PlatyPS
ms.custom: OPS13
ms.date: 10/25/2024
PlatyPS schema version: 2024-05-01
title: Export-MarkdownModuleFile
---

# Export-MarkdownModuleFile

## SYNOPSIS

Exports a **ModuleFileInfo** object to a markdown file.

## SYNTAX

### __AllParameterSets

```
Export-MarkdownModuleFile [-ModuleFileInfo] <ModuleFileInfo[]> [-Encoding <Encoding>] [-Force]
 [-Metadata <hashtable>] [-OutputFolder <string>] [-WhatIf] [-Confirm] [<CommonParameters>]
```

## ALIASES

## DESCRIPTION

This command exports a **ModuleFileInfo** object to a markdown file. You can add metadata
frontmatter to the markdown file by using the **Metadata** parameter. You can get a
**ModuleFileInfo** object by using the `Import-MarkdownModuleFile` cmdlet. You can import a module
file that's written in the old format and export it to the new format.

## EXAMPLES

### Example 1 - Convert an old module file to the new format

In this example, a **ModuleFileInfo** object by importing a module Markdown file from the `.\v1`
folder. That object is then exported to a Markdown file in the new format using the
`Export-MarkdownModuleFile`.

```powershell
Import-MarkdownModuleFile -Path .\v1\Microsoft.PowerShell.PlatyPS\Microsoft.PowerShell.PlatyPS.md |
    Export-MarkdownModuleFile -OutputFolder .\v1\new\Microsoft.PowerShell.PlatyPS -Force
```

```Output
    Directory: D:\Git\PS-Src\platyPS\v2docs\v1\new\Microsoft.PowerShell.PlatyPS

Mode                 LastWriteTime         Length Name
----                 -------------         ------ ----
-a---           8/26/2024  3:38 PM           2716 Microsoft.PowerShell.PlatyPS.md
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

The metadata to add to the frontmatter of the markdown file. The metadata is a hashtable where the
you specify the key and value pairs to add to the frontmatter. New key names are added to the
existing frontmatter. The values of existing keys are overwritten.

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

The **ModuleFileInfo** object to export to a markdown file. This object is created by the
`Import-MarkdownModuleFile` cmdlet.

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

The folder where the markdown file is saved. If the folder doesn't exist, it's created.

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

- [Import-MarkdownModuleFile](Import-MarkdownModuleFile.md)
