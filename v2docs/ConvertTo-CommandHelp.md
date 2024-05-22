---
external help file: Microsoft.PowerShell.PlatyPS.dll-Help.xml
Module Name: Microsoft.PowerShell.PlatyPS
HelpUri:
ms.date: 05/21/2024
PlatyPS schema version: 2024-05-01
content type: cmdlet
---

# ConvertTo-CommandHelp

## SYNOPSIS

Creates **CommandHelp** objects for a PowerShell command that's loaded in the session.

## SYNTAX

### Default (Default)

```
ConvertTo-CommandHelp [-CommandInfo] <String[]> [<CommonParameters>]
```

## ALIASES

## DESCRIPTION

This cmdlet takes a list of one or more PowerShell commands and creates **CommandHelp** objects for
each command. The **CommandHelp** object is a structured representation of the help content that can
be used to export to different formats.

## EXAMPLES

### Example 1 - Creates a **CommandHelp** object for a cmdlet

The following example creates a **CommandHelp** object for the `New-PSSession` cmdlet. The
`Get-Member` cmdlet is used to display the properties and methods of the **CommandHelp** object.

```powershell
ConvertTo-CommandHelp New-PSSession | Get-Member
```

```Output
   TypeName: Microsoft.PowerShell.PlatyPS.Model.CommandHelp

Name                        MemberType Definition
----                        ---------- ----------
Equals                      Method     bool Equals(Microsoft.PowerShell.PlatyPS.Model.CommandHelp other), bool Equals(Syst…
GetHashCode                 Method     int GetHashCode()
GetType                     Method     type GetType()
ToString                    Method     string ToString()
TryGetParameter             Method     bool TryGetParameter(string name, [ref] Microsoft.PowerShell.PlatyPS.Model.Paramete…
Aliases                     Property   System.Collections.Generic.List[string] Aliases {get;}
Description                 Property   string Description {get;set;}
Diagnostics                 Property   Microsoft.PowerShell.PlatyPS.Model.Diagnostics Diagnostics {get;set;}
Examples                    Property   System.Collections.Generic.List[Microsoft.PowerShell.PlatyPS.Model.Example] Example…
ExternalHelpFile            Property   string ExternalHelpFile {get;set;}
HasCmdletBinding            Property   bool HasCmdletBinding {get;set;}
HasWorkflowCommonParameters Property   bool HasWorkflowCommonParameters {get;set;}
Inputs                      Property   System.Collections.Generic.List[Microsoft.PowerShell.PlatyPS.Model.InputOutput] Inp…
Locale                      Property   cultureinfo Locale {get;set;}
Metadata                    Property   ordered Metadata {get;set;}
ModuleGuid                  Property   System.Nullable[guid] ModuleGuid {get;set;}
ModuleName                  Property   string ModuleName {get;set;}
Notes                       Property   string Notes {get;set;}
OnlineVersionUrl            Property   string OnlineVersionUrl {get;set;}
Outputs                     Property   System.Collections.Generic.List[Microsoft.PowerShell.PlatyPS.Model.InputOutput] Out…
Parameters                  Property   System.Collections.Generic.List[Microsoft.PowerShell.PlatyPS.Model.Parameter] Param…
RelatedLinks                Property   System.Collections.Generic.List[Microsoft.PowerShell.PlatyPS.Model.Links] RelatedLi…
SchemaVersion               Property   string SchemaVersion {get;set;}
Synopsis                    Property   string Synopsis {get;set;}
Syntax                      Property   System.Collections.Generic.List[Microsoft.PowerShell.PlatyPS.Model.SyntaxItem] Synt…
Title                       Property   string Title {get;set;}
```

## PARAMETERS

### -CommandInfo

A list of one or more PowerShell commands (cmdlets, functions, scripts). The cmdlet creates a
**CommandHelp** object for each command.

```yaml
Type: System.String[]
DefaultValue: None
VariableLength: true
Globbing: true
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

- [Export-MarkdownCommandHelp](Export-MarkdownCommandHelp.md)
- [Export-YamlCommandHelp](Export-YamlCommandHelp.md)
