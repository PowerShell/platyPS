---
document type: cmdlet
external help file: Microsoft.PowerShell.PlatyPS.dll-Help.xml
HelpUri: ''
Locale: en-US
Module Name: Microsoft.PowerShell.PlatyPS
ms.custom: OPS13
ms.date: 10/25/2024
PlatyPS schema version: 2024-05-01
title: New-MarkdownCommandHelp
---

# New-MarkdownCommandHelp

## SYNOPSIS

Creates Markdown help files for PowerShell modules and commands.

## SYNTAX

### __AllParameterSets

```
New-MarkdownCommandHelp -OutputFolder <string> [-CommandInfo <CommandInfo[]>] [-Encoding <Encoding>]
 [-Force] [-HelpUri <string>] [-HelpInfoUri <string>] [-HelpVersion <version>] [-Locale <string>]
 [-Metadata <hashtable>] [-ModuleInfo <psmoduleinfo[]>] [-WithModulePage]
 [-AbbreviateParameterTypename] [-WhatIf] [-Confirm] [<CommonParameters>]
```

## ALIASES

## DESCRIPTION

Creates Markdown help files for PowerShell modules and commands.

## EXAMPLES

### Example 1 - Create Markdown help files for a module

```powershell
$newMarkdownCommandHelpSplat = @{
    ModuleInfo = Get-Module Microsoft.PowerShell.PlatyPS
    OutputFolder = '.'
    HelpVersion = '1.0.0.0'
    WithModulePage = $true
}
New-MarkdownCommandHelp @newMarkdownCommandHelpSplat
```

### Example 2 - Create Markdown help files from a list of commands

```powershell
$newMarkdownCommandHelpSplat = @{
    CommandInfo = Get-Command -Module Microsoft.PowerShell.PlatyPS
    OutputFolder = '.'
    HelpVersion = '1.0.0.0'
    WithModulePage = $true
}
New-MarkdownCommandHelp @newMarkdownCommandHelpSplat
```

## PARAMETERS

### -AbbreviateParameterTypename

By default, this command uses full type names in the parameter metadata and for the input and output
types. When you use this parameter, the cmdlet outputs short type names.

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

### -CommandInfo

A list of one or more commands to create help for.

```yaml
Type: System.Management.Automation.CommandInfo[]
DefaultValue: ''
SupportsWildcards: false
ParameterValue: []
Aliases: []
ParameterSets:
- Name: (All)
  Position: Named
  IsRequired: false
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

The encoding used when creating the output files. If not specified, the cmdlet uses value specified
by `$OutputEncoding`.

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

By default, this command doesn't overwrite existing files. When you use this parameter, the cmdlet
overwrites existing files.

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

### -HelpInfoUri

This parameter allows you to specify the URI used for updateable help. By default, the cmdlet uses
the HelpInfoUri specified in the module manifest.

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

### -HelpUri

This parameter allows you to specify the URI used for online help. By default, the cmdlet uses the
URI defined in the `[CmdletBinding()]` attribute for the command.

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

### -HelpVersion

This parameter allows you to specify the version of the help. The default value is `1.0.0.0`. This
version is written to the `HelpInfo.xml` file that is used for updateable help.

```yaml
Type: System.Version
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

### -Locale

This parameter allows you to specify the language locale for the help files. By default, the cmdlet
uses the current **CultureInfo**. Use the `Get-Culture` cmdlet to see the current culture settings
on your system.

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

### -Metadata

The metadata to add to the frontmatter of the markdown file. The metadata is a hashtable where the
you specify the key and value pairs to add to the frontmatter. New key names are added to the
existing frontmatter. The values of existing keys are overwritten. You can't overwrite the values of
the `document type` or `PlatyPS schema version` keys. If these keys are present in the hashtable,
the cmdlet ignores the values and outputs a warning.

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

### -ModuleInfo

A list of one or more modules to create help for. The cmdlet creates Markdown help files for all
commands in the module. The cmdlet creates a folder matching the name of the module in the output
location. All Markdown files are written to the module folder.

```yaml
Type: System.Management.Automation.PSModuleInfo[]
DefaultValue: ''
SupportsWildcards: false
ParameterValue: []
Aliases: []
ParameterSets:
- Name: (All)
  Position: Named
  IsRequired: false
  ValueFromPipeline: true
  ValueFromPipelineByPropertyName: false
  ValueFromRemainingArguments: false
DontShow: false
AcceptedValues: []
HelpMessage: ''
```

### -OutputFolder

Specifies the location of where the Markdown help files are written. The cmdlet creates a folder for
each module being processed. If the target command isn't associated with a module, the cmdlet
creates a the Markdown file in the root of the output folder.

```yaml
Type: System.String
DefaultValue: ''
SupportsWildcards: false
ParameterValue: []
Aliases: []
ParameterSets:
- Name: (All)
  Position: Named
  IsRequired: true
  ValueFromPipeline: false
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

### -WithModulePage

By default, this cmdlet only creates Markdown files for commands. When you use this parameter, the
cmdlet creates a Markdown file for the module. This Markdown file contains a list of all commands in
the module and metadata used to create the HelpInfo.xml file.

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

### CommonParameters

This cmdlet supports the common parameters: -Debug, -ErrorAction, -ErrorVariable,
-InformationAction, -InformationVariable, -OutBuffer, -OutVariable, -PipelineVariable,
-ProgressAction, -Verbose, -WarningAction, and -WarningVariable. For more information, see
[about_CommonParameters](https://go.microsoft.com/fwlink/?LinkID=113216).

## INPUTS

### System.Management.Automation.CommandInfo

### System.Management.Automation.PSModuleInfo

## OUTPUTS

### System.IO.FileInfo

## NOTES

## RELATED LINKS

- [Export-MarkdownCommandHelp](Export-MarkdownCommandHelp.md)
