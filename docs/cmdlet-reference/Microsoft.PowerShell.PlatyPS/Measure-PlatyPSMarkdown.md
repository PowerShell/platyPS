---
document type: cmdlet
external help file: Microsoft.PowerShell.PlatyPS.dll-Help.xml
HelpUri: ''
Locale: en-US
Module Name: Microsoft.PowerShell.PlatyPS
ms.custom: OPS13
ms.date: 10/25/2024
PlatyPS schema version: 2024-05-01
title: Measure-PlatyPSMarkdown
---

# Measure-PlatyPSMarkdown

## SYNOPSIS

Determines the type of Markdown file.

## SYNTAX

### Path (Default)

```
Measure-PlatyPSMarkdown [-Path] <string[]> [<CommonParameters>]
```

### LiteralPath

```
Measure-PlatyPSMarkdown -LiteralPath <string[]> [<CommonParameters>]
```

## ALIASES

## DESCRIPTION

The command reads the content of the file and determines the type of content. The command returns a
**PlatyPSMarkdown** object that contains the content type and schema version of a PlatyPS markdown
file. This is useful for selecting just the files that contain **CommandHelp** or **ModuleHelp**
content.

## EXAMPLES

### Example 1 - Measure all Markdown files in a folder

```powershell
Measure-PlatyPSMarkdown -Path .\v2\Microsoft.PowerShell.PlatyPS\*.md
```

```Output
Title                               Filetype              Filepath
-----                               --------              --------
Compare-CommandHelp                 CommandHelp, V2Schema D:\Git\PS-Src\platyPS\v2docs\v2\Microsoft.PowerShell.PlatyPS\Compare-CommandHelp.md
Export-MamlCommandHelp              CommandHelp, V2Schema D:\Git\PS-Src\platyPS\v2docs\v2\Microsoft.PowerShell.PlatyPS\Export-MamlCommandHelp.md
Export-MarkdownCommandHelp          CommandHelp, V2Schema D:\Git\PS-Src\platyPS\v2docs\v2\Microsoft.PowerShell.PlatyPS\Export-MarkdownCommandHelp.md
Export-MarkdownModuleFile           CommandHelp, V2Schema D:\Git\PS-Src\platyPS\v2docs\v2\Microsoft.PowerShell.PlatyPS\Export-MarkdownModuleFile.md
Export-YamlCommandHelp              CommandHelp, V2Schema D:\Git\PS-Src\platyPS\v2docs\v2\Microsoft.PowerShell.PlatyPS\Export-YamlCommandHelp.md
Export-YamlModuleFile               CommandHelp, V2Schema D:\Git\PS-Src\platyPS\v2docs\v2\Microsoft.PowerShell.PlatyPS\Export-YamlModuleFile.md
Import-MamlHelp                     CommandHelp, V2Schema D:\Git\PS-Src\platyPS\v2docs\v2\Microsoft.PowerShell.PlatyPS\Import-MamlHelp.md
Import-MarkdownCommandHelp          CommandHelp, V2Schema D:\Git\PS-Src\platyPS\v2docs\v2\Microsoft.PowerShell.PlatyPS\Import-MarkdownCommandHelp.md
Import-MarkdownModuleFile           CommandHelp, V2Schema D:\Git\PS-Src\platyPS\v2docs\v2\Microsoft.PowerShell.PlatyPS\Import-MarkdownModuleFile.md
Import-YamlCommandHelp              CommandHelp, V2Schema D:\Git\PS-Src\platyPS\v2docs\v2\Microsoft.PowerShell.PlatyPS\Import-YamlCommandHelp.md
Import-YamlModuleFile               CommandHelp, V2Schema D:\Git\PS-Src\platyPS\v2docs\v2\Microsoft.PowerShell.PlatyPS\Import-YamlModuleFile.md
Measure-PlatyPSMarkdown             CommandHelp, V2Schema D:\Git\PS-Src\platyPS\v2docs\v2\Microsoft.PowerShell.PlatyPS\Measure-PlatyPSMarkdown.md
Microsoft.PowerShell.PlatyPS Module ModuleFile, V2Schema  D:\Git\PS-Src\platyPS\v2docs\v2\Microsoft.PowerShell.PlatyPS\Microsoft.PowerShell.PlatyPS.md
New-CommandHelp                     CommandHelp, V2Schema D:\Git\PS-Src\platyPS\v2docs\v2\Microsoft.PowerShell.PlatyPS\New-CommandHelp.md
New-MarkdownCommandHelp             CommandHelp, V2Schema D:\Git\PS-Src\platyPS\v2docs\v2\Microsoft.PowerShell.PlatyPS\New-MarkdownCommandHelp.md
New-MarkdownModuleFile              CommandHelp, V2Schema D:\Git\PS-Src\platyPS\v2docs\v2\Microsoft.PowerShell.PlatyPS\New-MarkdownModuleFile.md
Test-MarkdownCommandHelp            CommandHelp, V2Schema D:\Git\PS-Src\platyPS\v2docs\v2\Microsoft.PowerShell.PlatyPS\Test-MarkdownCommandHelp.md
Update-CommandHelp                  CommandHelp, V2Schema D:\Git\PS-Src\platyPS\v2docs\v2\Microsoft.PowerShell.PlatyPS\Update-CommandHelp.md
Update-MarkdownCommandHelp          CommandHelp, V2Schema D:\Git\PS-Src\platyPS\v2docs\v2\Microsoft.PowerShell.PlatyPS\Update-MarkdownCommandHelp.md
Update-MarkdownModuleFile           CommandHelp, V2Schema D:\Git\PS-Src\platyPS\v2docs\v2\Microsoft.PowerShell.PlatyPS\Update-MarkdownModuleFile.md
```

## PARAMETERS

### -LiteralPath

Specifies a path to one or more Markdown files. The value of **LiteralPath** is used exactly as it's
typed. No characters are interpreted as wildcards. If the path includes escape characters, enclose
it in single quotation marks. Single quotation marks tell PowerShell not to interpret any characters
as escape sequences.

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
  ValueFromPipeline: true
  ValueFromPipelineByPropertyName: false
  ValueFromRemainingArguments: false
DontShow: false
AcceptedValues: []
HelpMessage: ''
```

### -Path

The path to one or more Markdown files. Specifies a path to one or more locations.

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

### Microsoft.PowerShell.PlatyPS.MarkdownProbeInfo

## NOTES

For old format markdown files, the command examines the frontmatter and H2 structure to determine
the content type. For new format markdown files, the command examines the `document type` key in the
frontmatter.

The command doesn't validate the full structure of the markdown file. Use `Test-MarkdownCommandHelp`
to validate the structure of the markdown file.

## RELATED LINKS

- [Test-MarkdownCommandHelp](Test-MarkdownCommandHelp.md)
