---
external help file: platyPS-help.xml
schema: 2.0.0
---

# Get-HelpPreview
## SYNOPSIS
Preview the output Get-Help would return from an external help file(s).

## SYNTAX

### FileOutput
```
Get-HelpPreview -MamlFilePath <String[]> -TextOutputPath <String> [-Encoding <String>] [<CommonParameters>]
```

### AsObject
```
Get-HelpPreview -MamlFilePath <String[]> [-AsObject] [<CommonParameters>]
```

## DESCRIPTION
You can use PowerShell help engine to display the text help output for external help.
This cmdlet verifies how markdown-generated help will look in Get-Help output.

It can be output in a form of help object or as a text.

## EXAMPLES

### Example 1
```
PS C:\> $help = Get-HelpPreview -File .\out\platyPS\en-US\platyPS-help.xml
PS C:\> $help.Name
Get-MarkdownMetadata
New-ExternalHelp
New-ExternalHelpCab
New-MarkdownHelp
Get-HelpPreview
Update-Markdown
```

Returns a help object get-help preview from maml xml and assign it to the $help variable.
Gets the names of Cmdlet objects inside help.

## PARAMETERS

### -File
File path to maml-xml files.
You can pass several of them.

```yaml
Type: String[]
Parameter Sets: FileOutput, AsObject
Aliases: 

Required: True
Position: Named
Default value: 
Accept pipeline input: False
Accept wildcard characters: False
```

### CommonParameters
This cmdlet supports the common parameters: -Debug, -ErrorAction, -ErrorVariable, -InformationAction, -InformationVariable, -OutVariable, -OutBuffer, -PipelineVariable, -Verbose, -WarningAction, and -WarningVariable.
For more information, see about_CommonParameters \(http://go.microsoft.com/fwlink/?LinkID=113216\).

## INPUTS
### String
You cannot pipe objects into this cmdlet.

## OUTPUTS
### HelpInfo

Help object. Same as Get-Help command provides.

## NOTES

## RELATED LINKS

[Online Version:]()


