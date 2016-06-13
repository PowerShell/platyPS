---
external help file: platyPS-help.xml
schema: 2.0.0
---

# Get-HelpPreview
## SYNOPSIS
Displays your generated external help as **Get-Help** output.

## SYNTAX

```
Get-HelpPreview -Path <String[]>
```

## DESCRIPTION
The **Get-HelpPreview** cmdlet displays your generated external help as [Get-Help](https://msdn.microsoft.com/en-us/library/dd878343.aspx) output. Specify one or more files in Microsoft Assistance Markup Language (MAML) format.

## EXAMPLES

### Example 1: Preview the PlatyPS help
```
PS C:\> $Help = Get-HelpPreview -Path ".\out\platyPS\en-US\PlatyPS-help.xml"

PS C:\> $Help.Name

Get-HelpPreview
Get-MarkdownMetadata
New-ExternalHelp
New-ExternalHelpCab
New-MarkdownHelp
Update-MarkdownHelp
Update-MarkdownHelpModule
Update-MarkdownHelpSchema
```

The first command creates a **Help** object for the the specified MAML file. The command stores it in the $Help variable.

The second command displays the **Name** property for each of the objects in $Help.

## PARAMETERS

### -Path
Specifies an array of paths of MAML external help files.

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

## INPUTS

### String[]
You can pipe an array of paths to this cmdlet.

## OUTPUTS

### Help Object
This cmdlet returns a **Help** object, which is the same output as **Get-Help**.

## NOTES

## RELATED LINKS

[Online Version:](https://github.com/PowerShell/platyPS/blob/master/docs/Get-HelpPreview.md)

[New-ExternalHelp](New-ExternalHelp.md)

[Get-Help](https://msdn.microsoft.com/en-us/library/dd878343.aspx)
