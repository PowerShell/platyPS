---
content type: cmdlet
external help file: Microsoft.PowerShell.PlatyPS.dll-Help.xml
HelpUri: ''
Locale: en-US
Module Name: Microsoft.PowerShell.PlatyPS
ms.date: 05/29/2024
PlatyPS schema version: 2024-05-01
title: Import-MarkdownModuleFile
---

# Import-MarkdownModuleFile

## SYNOPSIS

Imports a Markdown module file into a **ModuleHelp** object.

## SYNTAX

### Path (Default)

```
Import-MarkdownModuleFile [-Path] <System.String[]> [<CommonParameters>]
```

### LiteralPath

```
Import-MarkdownModuleFile -LiteralPath <System.String[]> [<CommonParameters>]
```

## ALIASES

## DESCRIPTION

The command imports Markdown files containing module help and creates **ModuleHelp** objects. The
**ModuleHelp** object is a structured representation of the help content that can be used to export
to different formats.

## EXAMPLES

### Example 1 - Convert

```powershell
Import-MarkdownModuleFile .\v2\Microsoft.PowerShell.PlatyPS.md
```

```Output
Metadata        : {[Module Name, Microsoft.PowerShell.PlatyPS], [Module Guid, 0bdcabef-a4b7-4a6d-bf7e-d879817ebbff],
                  [Download Help Link, ''], [Help Version, '1.0.0']…}
Title           : Microsoft.PowerShell.PlatyPS Module
Module          : Microsoft.PowerShell.PlatyPS
ModuleGuid      : 0bdcabef-a4b7-4a6d-bf7e-d879817ebbff
Description     : This module contains cmdlets to help with the creation help content for PowerShell commands.
Locale          : en-US
OptionalElement :
Commands        : {Compare-CommandHelp, ConvertTo-CommandHelp, Export-MamlCommandHelp, Export-MarkdownCommandHelp…}
Diagnostics     : Microsoft.PowerShell.PlatyPS.Model.Diagnostics

```

## PARAMETERS

### -LiteralPath

Specifies a path to one or more locations containing markdown files. The value of **LiteralPath** is
used exactly as it's typed. No characters are interpreted as wildcards. If the path includes escape
characters, enclose it in single quotation marks. Single quotation marks tell PowerShell not to
interpret any characters as escape sequences.

For more information, see
[about_Quoting_Rules](/powershell/module/microsoft.powershell.core/about/about_CommonParameters).

```yaml
Type: System.String[]
DefaultValue: ''
VariableLength: true
SupportsWildcards: false
ParameterValue: []
Aliases: []
ParameterSets:
- Name: LiteralPath
  Position: Named
  IsRequired: true
  ValueByPipeline: true
  ValueByPipelineByPropertyName: false
  ValueFromRemainingArguments: false
DontShow: false
AcceptedValues: []
HelpMessage: ''
```

### -Path

Specifies the path to an item. This cmdlet gets the item at the specified location. Wildcard
characters are permitted. This parameter is required, but the parameter name Path is optional.

Use a dot (`.`) to specify the current location. Use the wildcard character (`*`) to specify all
items in the current location.

```yaml
Type: System.String[]
DefaultValue: ''
VariableLength: true
SupportsWildcards: true
ParameterValue: []
Aliases: []
ParameterSets:
- Name: Path
  Position: 0
  IsRequired: true
  ValueByPipeline: true
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

- [Export-MarkdownModuleFile](Export-MarkdownModuleFile.md)
- [Export-YamlModuleFile](Export-YamlModuleFile.md)
- [Import-MarkdownModuleFile](Import-MarkdownModuleFile.md)