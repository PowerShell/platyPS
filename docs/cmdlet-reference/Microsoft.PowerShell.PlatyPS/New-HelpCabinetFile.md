---
document type: cmdlet
external help file: Microsoft.PowerShell.PlatyPS-Help.xml
HelpUri: ''
Locale: en-US
Module Name: Microsoft.PowerShell.PlatyPS
ms.custom: OPS13
ms.date: 10/25/2024
PlatyPS schema version: 2024-05-01
title: New-HelpCabinetFile
---

# New-HelpCabinetFile

## SYNOPSIS

Creates a help cabinet file for a module that can be published as updateable help content.

## SYNTAX

### __AllParameterSets

```
New-HelpCabinetFile [-CabinetFilesFolder] <string> [-MarkdownModuleFile] <string>
 [-OutputFolder] <string> [-WhatIf] [-Confirm] [<CommonParameters>]
```

## ALIASES

## DESCRIPTION

This cmdlet create `.cab` and `.zip` files that contain help files for a module.

> [!NOTE]
> This cmdlet depends on the `MakeCab.exe` native command, which is only available on Windows. This
> cmdlet raises an error if used on non-Windows machines.

This cmdlet uses metadata stored in the module Markdown file to name your `.cab` and `.zip` files.
This naming matches the pattern that the PowerShell help system requires for use as updatable help.

This cmdlet also generates or updates an existing `Helpinfo.xml` file. That file provides versioning
and locale details to the PowerShell help system.

## EXAMPLES

### Example 1 - Create updateable help files for a module

```powershell
$params = @{
    CabinetFilesFolder = '.\maml\Microsoft.PowerShell.PlatyPS'
    MarkdownModuleFile = '.\Microsoft.PowerShell.PlatyPS\Microsoft.PowerShell.PlatyPS.md'
    OutputFolder       = '.\cab'
}
New-HelpCabinetFile @params
```

## PARAMETERS

### -CabinetFilesFolder

The path to the folder containing the MAML file to be packaged.

```yaml
Type: System.String
DefaultValue: ''
SupportsWildcards: false
ParameterValue: []
Aliases: []
ParameterSets:
- Name: (All)
  Position: 0
  IsRequired: true
  ValueFromPipeline: false
  ValueFromPipelineByPropertyName: false
  ValueFromRemainingArguments: false
DontShow: false
AcceptedValues: []
HelpMessage: ''
```

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

### -MarkdownModuleFile

Specifies the full path of the module Markdown file.

```yaml
Type: System.String
DefaultValue: ''
SupportsWildcards: false
ParameterValue: []
Aliases: []
ParameterSets:
- Name: (All)
  Position: 1
  IsRequired: true
  ValueFromPipeline: false
  ValueFromPipelineByPropertyName: false
  ValueFromRemainingArguments: false
DontShow: false
AcceptedValues: []
HelpMessage: ''
```

### -OutputFolder

The location where you want to write the `.cab`, `.zip`, and `HelpInfo.xml` files.

```yaml
Type: System.String
DefaultValue: ''
SupportsWildcards: false
ParameterValue: []
Aliases: []
ParameterSets:
- Name: (All)
  Position: 2
  IsRequired: true
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

## OUTPUTS

### System.IO.FileInfo

The **FileInfo** objects that represent the files created by this cmdlet.

## NOTES

## RELATED LINKS
