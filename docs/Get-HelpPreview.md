---
external help file: platyPS-help.xml
schema: 2.0.0
---

# Get-HelpPreview
## SYNOPSIS
Preview the output Get-Help would return from an external help file(s).
## SYNTAX

```
Get-HelpPreview -Path <String[]> [<CommonParameters>]
```

## DESCRIPTION
You can use PowerShell help engine to display the text help output for external help.
This cmdlet verifies how markdown-generated help will look in Get-Help output.

It simulates the output produced by Get-Help cmdlet.
## EXAMPLES

### Example 1
```
PS C:\> $help = Get-HelpPreview .\out\platyPS\en-US\platyPS-help.xml

PS C:\> $help.Name
Get-HelpPreview
Get-MarkdownMetadata
New-ExternalHelp
New-ExternalHelpCab
New-MarkdownHelp
Update-MarkdownHelp
Update-MarkdownHelpModule
Update-MarkdownHelpSchema
```

Returns a help object get-help preview from maml xml and assign it to the $help variable.
Gets the names of Cmdlet objects inside help.
## PARAMETERS

### -Path
Path to MAML help files.
You can pass several of them.

```yaml
Type: String[]
Parameter Sets: (All)
Aliases: 

Required: True
Position: Named
Default value: 
Accept pipeline input: True (ByValue)
Accept wildcard characters: False
```

### CommonParameters
This cmdlet supports the common parameters: -Debug, -ErrorAction, -ErrorVariable, -InformationAction, -InformationVariable, -OutVariable, -OutBuffer, -PipelineVariable, -Verbose, -WarningAction, and -WarningVariable. For more information, see about_CommonParameters (http://go.microsoft.com/fwlink/?LinkID=113216).
## INPUTS

### String[]
You can pipe a collection of paths to this cmdlet.
## OUTPUTS

### Help Object
Help object, which is the same as Get-Help provides.
## NOTES

## RELATED LINKS

[Online Version:](https://github.com/PowerShell/platyPS/blob/master/docs/Get-HelpPreview.md)






