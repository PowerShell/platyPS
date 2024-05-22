---
external help file: Microsoft.PowerShell.PlatyPS.dll-Help.xml
Module Name: Microsoft.PowerShell.PlatyPS
HelpUri:
ms.date: 05/20/2024
PlatyPS schema version: 2024-05-01
content type: cmdlet
---

# Export-MarkdownCommandHelp

## SYNOPSIS

Exports a **CommandHelp** object to a markdown file.

## SYNTAX

### Default (Default)

```
Export-MarkdownCommandHelp [-Command] <CommandHelp[]> [-Encoding <Encoding>] [-Force]
 [-OutputFolder <String>] [-Metadata <Hashtable>] [<CommonParameters>]
```

## ALIASES

## DESCRIPTION

This command exports a **CommandHelp** object to a markdown file. You can add metadata frontmatter
to the markdown file by using the **Metadata** parameter. You can get a **CommandHelp** object by
using the `ConvertTo-CommandHelp` cmdlet or one of the `Import-*` cmdlets.

## EXAMPLES

### Example 1 - Export a CommandHelp object of a single command to a markdown file

In this example, the `ConvertTo-CommandHelp` creates a **CommandHelp** object for the `Get-Date`
cmdlet. That object is then exported to a markdown file using the `Export-MarkdownCommandHelp`.

```powershell
ConvertTo-CommandHelp -CommandInfo Get-Date | Export-MarkdownCommandHelp -OutputFolder .
```

```Output
    Directory: D:\Git\PS-Src\platyPS\v2docs

Mode                 LastWriteTime         Length Name
----                 -------------         ------ ----
-a---           5/20/2024  3:26 PM          21669 Get-Date.md
```

### Example 2 - Convert old Markdown help content to the new format

This example imports Markdown help in the old format from the `.\v1` folder and exports it to the
`.\v2` folder in the new format.

```powershell
$chobj = Import-MarkdownCommandHelp -Path .\v1\Export-YamlModuleFile.md
Export-MarkdownCommandHelp -Command $chobj -OutputFolder .\v2
```

```Output
    Directory: D:\Git\PS-Src\platyPS\v2docs\v2

Mode                 LastWriteTime         Length Name
----                 -------------         ------ ----
-a---           5/20/2024  3:33 PM           2892 Export-YamlModuleFile.md
```

## PARAMETERS

### -Command

The **CommandHelp** object to export. You can pass the **CommandHelp** object on the pipeline or by
using the **Command** parameter.

```yaml
Type: Microsoft.PowerShell.PlatyPS.Model.CommandHelp[]
DefaultValue: None
VariableLength: true
Globbing: false
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

### -Encoding

The encoding to use when writing the markdown file. If no value is specified, encoding defaults to
the value of the `$OutputEncoding` preference variable.

```yaml
Type: System.Text.Encoding
DefaultValue: None
VariableLength: true
Globbing: false
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

### -Force

Use the **Force** parameter to overwrite the output file if it already exists.

```yaml
Type: System.Management.Automation.SwitchParameter
DefaultValue: None
VariableLength: true
Globbing: false
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

### -Metadata

The metadata to add to the frontmatter of the markdown file. The metadata is a hashtable where the
you specify the key and value pairs to add to the frontmatter. New key names are added to the
existing frontmatter. The values of existing keys are overwritten.

```yaml
Type: System.Collections.Hashtable
DefaultValue: None
VariableLength: true
Globbing: false
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

### -OutputFolder

The folder where the markdown file is saved. If the folder doesn't exist, it's created.

```yaml
Type: System.String
DefaultValue: None
VariableLength: true
Globbing: false
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

### Microsoft.PowerShell.PlatyPS.Model.CommandHelp

## OUTPUTS

### System.IO.FileInfo

## NOTES

## RELATED LINKS

- [Export-YamlCommandHelp](Export-YamlCommandHelp.md)
- [Import-MarkdownCommandHelp](Import-MarkdownCommandHelp.md)
- [Import-YamlCommandHelp](Import-YamlCommandHelp.md)
