---
document type: cmdlet
external help file: Microsoft.PowerShell.PlatyPS-Help.xml
HelpUri: ''
Locale: en-US
Module Name: Microsoft.PowerShell.PlatyPS
ms.custom: OPS13
ms.date: 10/25/2024
PlatyPS schema version: 2024-05-01
title: Show-HelpPreview
---

# Show-HelpPreview

## SYNOPSIS

View MAML file contents as it would appear when output by `Get-Help`.

## SYNTAX

### Path

```
Show-HelpPreview [-Path] <string[]> [-ConvertNotesToList] [-ConvertDoubleDashLists]
 [<CommonParameters>]
```

### LiteralPath

```
Show-HelpPreview [-LiteralPath] <string[]> [-ConvertNotesToList] [-ConvertDoubleDashLists]
 [<CommonParameters>]
```

## ALIASES

## DESCRIPTION

The `Show-HelpPreview` cmdlet displays your generated external help as `Get-Help` output. Specify
one or more files in Microsoft Assistance Markup Language (MAML) format.

## EXAMPLES

### Example 1 - Display the contents of a MAML file

This example displays the contents of PlatyPS MAML file as it would appear when output by
`Get-Help`.

```powershell
Show-HelpPreview -Path .\Microsoft.PowerShell.PlatyPS\Microsoft.PowerShell.PlatyPS-Help.xml
```

## PARAMETERS

### -ConvertDoubleDashLists

Indicates that this cmdlet converts double-hyphen list bullets into single-hyphen bullets.
Double-hyphen lists are common in Windows PowerShell documentation. Markdown accepts single-hyphens
for lists.

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

### -ConvertNotesToList

Indicates that this cmdlet formats multiple paragraph items in the **NOTES** section as single list
items.

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

### -LiteralPath

Specifies one or more paths to MAML help files. Wildcards aren't supported.

```yaml
Type: System.String[]
DefaultValue: ''
SupportsWildcards: false
ParameterValue: []
Aliases:
- PSPath
- LP
ParameterSets:
- Name: LiteralPath
  Position: 1
  IsRequired: true
  ValueFromPipeline: true
  ValueFromPipelineByPropertyName: false
  ValueFromRemainingArguments: false
DontShow: false
AcceptedValues: []
HelpMessage: ''
```

### -Path

Specifies one or more paths to MAML help files. Wildcards are supported.

```yaml
Type: System.String[]
DefaultValue: ''
SupportsWildcards: true
ParameterValue: []
Aliases: []
ParameterSets:
- Name: Path
  Position: 1
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

You can pipe path strings to this command.

## OUTPUTS

### MamlCommandHelpInfo

This cmdlet returns a `MamlCommandHelpInfo` objects. This is the same object that is returned by
`Get-Help`.

## NOTES

## RELATED LINKS

- [Get-Help](xref:Microsoft.PowerShell.Core.Get-Help)
