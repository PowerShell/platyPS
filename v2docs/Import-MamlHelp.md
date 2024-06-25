---
content type: cmdlet
external help file: Microsoft.PowerShell.PlatyPS.dll-Help.xml
HelpUri: ''
Locale: en-US
Module Name: Microsoft.PowerShell.PlatyPS
ms.date: 05/29/2024
PlatyPS schema version: 2024-05-01
title: Import-MamlHelp
---

# Import-MamlHelp

## SYNOPSIS

Imports MAML help from a file and creates **CommandHelp** objects for each command in the file.

## SYNTAX

### Default (Default)

```
Import-MamlHelp [-MamlFile] <System.String>
 [-Settings <Microsoft.PowerShell.PlatyPS.TransformSettings>] [<CommonParameters>]
```

## ALIASES

## DESCRIPTION

This command can be used to create **CommandHelp** objects from MAML help files. These objects can
be used to generate Markdown help files.

## EXAMPLES

### Example 1

```powershell
PS C:\> {{ Add example code here }}
```

{{ Add example description here }}

## PARAMETERS

### -MamlFile

The path to the MAML help file.

```yaml
Type: System.String
DefaultValue: ''
VariableLength: true
SupportsWildcards: true
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

### -Settings

{{ Fill Settings Description }}

```yaml
Type: Microsoft.PowerShell.PlatyPS.TransformSettings
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

### CommonParameters

This cmdlet supports the common parameters: -Debug, -ErrorAction, -ErrorVariable,
-InformationAction, -InformationVariable, -OutBuffer, -OutVariable, -PipelineVariable,
-ProgressAction, -Verbose, -WarningAction, -WarningVariable. For more information, see
[about_CommonParameters](https://go.microsoft.com/fwlink/?LinkID=113216).

## INPUTS

### System.String

## OUTPUTS

### Microsoft.PowerShell.PlatyPS.Model.CommandHelp

## NOTES

## RELATED LINKS

{{ Fill in the related links here }}

