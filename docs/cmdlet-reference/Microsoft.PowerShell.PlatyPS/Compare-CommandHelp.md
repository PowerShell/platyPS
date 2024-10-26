---
document type: cmdlet
external help file: Microsoft.PowerShell.PlatyPS.dll-Help.xml
HelpUri: ''
Locale: en-US
Module Name: Microsoft.PowerShell.PlatyPS
ms.custom: OPS13
ms.date: 10/25/2024
PlatyPS schema version: 2024-05-01
title: Compare-CommandHelp
---

# Compare-CommandHelp

## SYNOPSIS

Compares two **CommandHelp** objects and produces a detailed report showing the differences.

## SYNTAX

### __AllParameterSets

```
Compare-CommandHelp [-ReferenceCommandHelp] <CommandHelp> [-DifferenceCommandHelp] <CommandHelp>
 [-PropertyNamesToExclude <string[]>] [<CommonParameters>]
```

## ALIASES

## DESCRIPTION

`Compare-CommandHelp` is a troubleshooting tool that compares two CommandHelp objects and produces a
detailed report showing the differences. For example, you can use this to compare objects imported
from different sources, such as two different versions of Markdown files.

## EXAMPLES

### Example 1

```powershell
$refcmd = Import-MarkdownCommandHelp -Path .\v1\Microsoft.PowerShell.PlatyPS\Compare-CommandHelp.md
$diffcmd = Import-MarkdownCommandHelp -Path .\v2\Microsoft.PowerShell.PlatyPS\Compare-CommandHelp.md
Compare-CommandHelp -ReferenceCommandHelp $refcmd -DifferenceCommandHelp $diffcmd > .\diff.log
```

## PARAMETERS

### -DifferenceCommandHelp

The CommandHelp object to compare against the reference object.

```yaml
Type: Microsoft.PowerShell.PlatyPS.Model.CommandHelp
DefaultValue: ''
SupportsWildcards: false
ParameterValue: []
Aliases: []
ParameterSets:
- Name: (All)
  Position: 1
  IsRequired: true
  ValueFromPipeline: false
  ValueFromPipelineByPropertyName: true
  ValueFromRemainingArguments: false
DontShow: false
AcceptedValues: []
HelpMessage: ''
```

### -PropertyNamesToExclude

A list of one or more property names to exclude from the comparison.

```yaml
Type: System.String[]
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

### -ReferenceCommandHelp

The base CommandHelp object to be compared to the difference object.

```yaml
Type: Microsoft.PowerShell.PlatyPS.Model.CommandHelp
DefaultValue: ''
SupportsWildcards: false
ParameterValue: []
Aliases: []
ParameterSets:
- Name: (All)
  Position: 0
  IsRequired: true
  ValueFromPipeline: false
  ValueFromPipelineByPropertyName: true
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

### Microsoft.PowerShell.PlatyPS.Model.CommandHelp

## OUTPUTS

### System.String

## NOTES

## RELATED LINKS

- [Import-MarkdownHelp](Import-MarkdownHelp.md)
- [Import-YamlCommandHelp](Import-YamlCommandHelp.md)
