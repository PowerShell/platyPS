---
document type: cmdlet
external help file: Microsoft.PowerShell.PlatyPS.dll-Help.xml
HelpUri: ''
Locale: en-US
Module Name: Microsoft.PowerShell.PlatyPS
ms.custom: OPS13
ms.date: 10/25/2024
PlatyPS schema version: 2024-05-01
title: Import-MamlHelp
---

# Import-MamlHelp

## SYNOPSIS

Imports MAML help from a file and creates **CommandHelp** objects for each command in the file.

## SYNTAX

### Path (Default)

```
Import-MamlHelp [-Path] <string[]> [<CommonParameters>]
```

### LiteralPath

```
Import-MamlHelp -LiteralPath <string[]> [<CommonParameters>]
```

## ALIASES

## DESCRIPTION

This command can be used to create **CommandHelp** objects from MAML help files. These objects can
be used to generate Markdown help files. You can also use this command to test the validity of the
MAML help files.

## EXAMPLES

### Example 1

```powershell
Import-MamlHelp -Path .\maml\Microsoft.PowerShell.PlatyPS.dll-Help.xml
```

## PARAMETERS

### -LiteralPath

The path to the MAML help file. Specifies a path to one or more locations. The value of LiteralPath
is used exactly as it's typed. No characters are interpreted as wildcards. If the path includes
escape characters, enclose it in single quotation marks. Single quotation marks tell PowerShell not
to interpret any characters as escape sequences.

For more information, see
[about_Quoting_Rules](/powershell/module/microsoft.powershell.core/about/about_quoting_rules).

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
  ValueFromPipeline: false
  ValueFromPipelineByPropertyName: true
  ValueFromRemainingArguments: false
DontShow: false
AcceptedValues: []
HelpMessage: ''
```

### -Path

The path to the MAML help file. Specifies a path to one or more locations.

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

### Microsoft.PowerShell.PlatyPS.Model.CommandHelp

## NOTES

The MAML file format doesn't support all the information that can be stored in a **CommandHelp**
object. If you convert the imported information to markdown format, the resulting markdown file
is not a complete representation of the original command.

## RELATED LINKS

- [Export-MamlHelp](Export-MamlCommandHelp.md)
