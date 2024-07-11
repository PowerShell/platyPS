---
document type: cmdlet
external help file: Microsoft.PowerShell.PlatyPS.dll-Help.xml
HelpUri: ''
Locale: en-US
Module Name: Microsoft.PowerShell.PlatyPS
ms.date: 07/10/2024
PlatyPS schema version: 2024-05-01
title: Import-YamlModuleFile
---

# Import-YamlModuleFile

## SYNOPSIS

Imports a Yaml module file into a **ModuleHelp** object.

## SYNTAX

### Path (Default)

```
Import-YamlModuleFile [-Path] <string[]> [<CommonParameters>]
```

### LiteralPath

```
Import-YamlModuleFile -LiteralPath <string[]> [<CommonParameters>]
```

## ALIASES

## DESCRIPTION

The command imports Yaml files containing module help and creates **ModuleHelp** objects. The
**ModuleHelp** object is a structured representation of the help content that can be used to export
to different formats.

## EXAMPLES

### Example 1 - Import a Yaml module file to a **ModuleHelp** object

```powershell
Import-YamlModuleFile .\Microsoft.PowerShell.PlatyPS.yml
```

```Output
Metadata        : {[document type, module], [HelpInfoUri, ], [Locale, en-US], [Module Guid,
                  0bdcabef-a4b7-4a6d-bf7e-d879817ebbff]…}
Title           : Microsoft.PowerShell.PlatyPS Module
Module          : Microsoft.PowerShell.PlatyPS
ModuleGuid      : 0bdcabef-a4b7-4a6d-bf7e-d879817ebbff
Description     : This module contains cmdlets to help with the creation help content for PowerShell commands.
Locale          : en-US
OptionalElement :
Commands        : {Compare-CommandHelp, Export-MamlCommandHelp, Export-MarkdownCommandHelp, Export-MarkdownModuleFile…}
Diagnostics     : Microsoft.PowerShell.PlatyPS.Model.Diagnostics
```

## PARAMETERS

### -LiteralPath

Specifies a path to one or more locations containing yaml module files. The value of **LiteralPath**
is used exactly as it's typed. No characters are interpreted as wildcards. If the path includes
escape characters, enclose it in single quotation marks. Single quotation marks tell PowerShell not
to interpret any characters as escape sequences.

For more information, see
[about_Quoting_Rules](/powershell/module/microsoft.powershell.core/about/about_CommonParameters).

```yaml
Type: System.String[]
DefaultValue: ''
SupportsWildcards: false
ParameterValue: []
Aliases: []
ParameterSets:
- Name: LiteralPath
  Position: Named
  IsRequired: true
  ValueFromPipeline: true
  ValueFromPipelineByPropertyName: false
  ValueFromRemainingArguments: false
DontShow: false
AcceptedValues: []
HelpMessage: ''
```

### -Path

Specifies a path to one or more locations containing yaml module files.

```yaml
Type: System.String[]
DefaultValue: ''
SupportsWildcards: true
ParameterValue: []
Aliases: []
ParameterSets:
- Name: Path
  Position: 0
  IsRequired: true
  ValueFromPipeline: true
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

### System.String

## OUTPUTS

### Microsoft.PowerShell.PlatyPS.ModuleFileInfo

## NOTES

## RELATED LINKS

[Import-MarkdownModuleFile](Import-MarkdownModuleFile.md)
