---
document type: cmdlet
external help file: Microsoft.PowerShell.PlatyPS.dll-Help.xml
HelpUri: ''
Locale: en-US
Module Name: Microsoft.PowerShell.PlatyPS
ms.custom: OPS13
ms.date: 10/25/2024
PlatyPS schema version: 2024-05-01
title: Export-MarkdownCommandHelp
---

# Export-MarkdownCommandHelp

## SYNOPSIS

Exports a **CommandHelp** object to a markdown file.

## SYNTAX

### __AllParameterSets

```
Export-MarkdownCommandHelp [-CommandHelp] <CommandHelp[]> [-Encoding <Encoding>] [-Force]
 [-OutputFolder <string>] [-Metadata <hashtable>] [-WhatIf] [-Confirm] [<CommonParameters>]
```

## ALIASES

## DESCRIPTION

This command exports a **CommandHelp** object to a markdown file. You can add metadata frontmatter
to the markdown file by using the **Metadata** parameter. You can get a **CommandHelp** object by
using the `Export-MarkdownCommandHelp` cmdlet or one of the `Import-*` cmdlets.

## EXAMPLES

### Example 1 - Convert old Markdown help content to the new format

This example imports Markdown help in the old format from the `.\v1` folder and exports it to the
`.\v2` folder in the new format.

```powershell
$chobj = Import-MarkdownCommandHelp -Path .\v1\Export-YamlModuleFile.md
Export-MarkdownCommandHelp -CommandHelp $chobj -OutputFolder .\v2
```

```Output
    Directory: D:\Git\PS-Src\platyPS\v2docs\v2

Mode                 LastWriteTime         Length Name
----                 -------------         ------ ----
-a---           5/20/2024  3:33 PM           2892 Export-YamlModuleFile.md
```

### Example 2 - Convert a folder of old Markdown help content to the new format

This example imports Markdown help in the old format from the `.\v1` folder and exports it to the
`.\v1\new` folder in the new format. If necessary, `Export-MarkdownCommandHelp` creates the
destination folders. The new markdown files are written to folder named for the module they belong
to.

```powershell
$mdfiles = Measure-PlatyPSMarkdown -Path .\v1\Microsoft.PowerShell.PlatyPS\*.md
$mdfiles | Where-Object Filetype -match 'CommandHelp' |
    Import-MarkdownCommandHelp -Path {$_.FilePath} |
    Export-MarkdownCommandHelp -OutputFolder .\v1\new
```

```Output
    Directory: D:\Git\PS-Src\platyPS\v2docs\v1\new\Microsoft.PowerShell.PlatyPS

Mode                 LastWriteTime         Length Name
----                 -------------         ------ ----
-a---           8/26/2024  3:30 PM           3194 Compare-CommandHelp.md
-a---           8/26/2024  3:30 PM           4282 Export-MamlCommandHelp.md
-a---           8/26/2024  3:30 PM           6401 Export-MarkdownCommandHelp.md
-a---           8/26/2024  3:30 PM           4874 Export-MarkdownModuleFile.md
-a---           8/26/2024  3:30 PM           6429 Export-YamlCommandHelp.md
-a---           8/26/2024  3:30 PM           5601 Export-YamlModuleFile.md
-a---           8/26/2024  3:30 PM           2724 Import-MamlHelp.md
-a---           8/26/2024  3:30 PM           3733 Import-MarkdownCommandHelp.md
-a---           8/26/2024  3:30 PM           3870 Import-MarkdownModuleFile.md
-a---           8/26/2024  3:30 PM           3907 Import-YamlCommandHelp.md
-a---           8/26/2024  3:30 PM           3477 Import-YamlModuleFile.md
-a---           8/26/2024  3:30 PM           2088 Measure-PlatyPSMarkdown.md
-a---           8/26/2024  3:30 PM           4748 New-CommandHelp.md
-a---           8/26/2024  3:30 PM           8647 New-MarkdownCommandHelp.md
-a---           8/26/2024  3:30 PM           5086 New-MarkdownModuleFile.md
-a---           8/26/2024  3:30 PM           3080 Test-MarkdownCommandHelp.md
-a---           8/26/2024  3:30 PM           3041 Update-CommandHelp.md
-a---           8/26/2024  3:30 PM           3981 Update-MarkdownCommandHelp.md
-a---           8/26/2024  3:30 PM           5548 Update-MarkdownModuleFile.md
```

## PARAMETERS

### -CommandHelp

The **CommandHelp** object to export. You can pass the **CommandHelp** object on the pipeline or by
using the **Command** parameter.

```yaml
Type: Microsoft.PowerShell.PlatyPS.Model.CommandHelp[]
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

Shows what would happen if the cmdlet runs. The cmdlet is not run.

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

### Microsoft.PowerShell.PlatyPS.Model.CommandHelp

## OUTPUTS

### System.IO.FileInfo

## NOTES

## RELATED LINKS

- [Export-YamlCommandHelp](Export-YamlCommandHelp.md)
- [Import-MarkdownCommandHelp](Import-MarkdownCommandHelp.md)
- [Import-YamlCommandHelp](Import-YamlCommandHelp.md)
