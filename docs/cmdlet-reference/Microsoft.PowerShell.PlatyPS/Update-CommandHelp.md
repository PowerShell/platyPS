---
document type: cmdlet
external help file: Microsoft.PowerShell.PlatyPS.dll-Help.xml
HelpUri: ''
Locale: en-US
Module Name: Microsoft.PowerShell.PlatyPS
ms.custom: OPS13
ms.date: 10/25/2024
PlatyPS schema version: 2024-05-01
title: Update-CommandHelp
---

# Update-CommandHelp

## SYNOPSIS

Updates an imported **CommandHelp** object with the information from session cmdlet of the same
name.

## SYNTAX

### Path (Default)

```
Update-CommandHelp [-Path] <string[]> [-WhatIf] [-Confirm] [<CommonParameters>]
```

### LiteralPath

```
Update-CommandHelp -LiteralPath <string[]> [-WhatIf] [-Confirm] [<CommonParameters>]
```

## ALIASES

## DESCRIPTION

This cmdlet imports a **CommandHelp** object from a Markdown file and updates the object with the
information from the session cmdlet of the same name. The updated object can then be re-exported to
update the source Markdown file.

## EXAMPLES

### Example 1

```powershell
$mdfiles = Measure-PlatyPSMarkdown -Path .\v1\Microsoft.PowerShell.PlatyPS\*.md
$cmdobj = $mdfiles | Where-Object Filetype -match 'CommandHelp' |
    Update-CommandHelp -Path {$_.FilePath}
$cmdobj.count
```

```Output
19
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

### -LiteralPath

Specifies a path to one or more markdown command files. The value of **LiteralPath** is used exactly
as it's typed. No characters are interpreted as wildcards. If the path includes escape characters,
enclose it in single quotation marks. Single quotation marks tell PowerShell not to interpret any
characters as escape sequences.

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

Specifies the path to one or more Markdown command file. Wildcard characters are permitted.

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

### System.String

## OUTPUTS

### Microsoft.PowerShell.PlatyPS.Model.CommandHelp

## NOTES

This command is similar to the `Update-MarkdownCommandHelp` cmdlet, but it updates the
**CommandHelp** object in memory instead of updating the source Markdown file.

## RELATED LINKS

- [Update-MarkdownCommandHelp](Update-MarkdownCommandHelp.md)
