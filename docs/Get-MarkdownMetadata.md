---
external help file: platyPS-help.xml
schema: 2.0.0
---

# Get-MarkdownMetadata
## SYNOPSIS
Gets the markdown header metadata in the form of a hashtable.

## SYNTAX

### FromPath
```
Get-MarkdownMetadata -Path <String[]> [<CommonParameters>]
```

### FromMarkdownString
```
Get-MarkdownMetadata -Markdown <String> [<CommonParameters>]
```

## DESCRIPTION
PlatyPS stores metadata information in the header block of the markdown file.
It's stored as key-value string pairs.

By default, platyPS stores help file name and markdown schema version.

The metadata section can contain user-provided key-value string pairs to be used by external tools.
These pairs would be ignored by [New-ExternalHelp](New-ExternalHelp.md).

[Get-MarkdownMetadata](Get-MarkdownMetadata.md) provides a consistent way to retrieve these key-value pairs.
The cmdlet returns a key-value \<Dictionary\[String, String\]\> object.

## EXAMPLES

### Example 1 (Get metadata by Path)
```
PS C:\> Get-MarkdownMetadata -Path .\docs\Get-MarkdownMetadata.md

Key                Value
---                -----
external help file platyPS-help.xml
schema             2.0.0
```

Retrives metadata from a markdown file at the path provided.

### Example 2 (Get metadata from a markdown string)
```
PS C:\> $markdown = cat -Raw .\docs\Get-MarkdownMetadata.md 
PS C:\> Get-MarkdownMetadata -Markdown $markdown

Key                Value
---                -----
external help file platyPS-help.xml
schema             2.0.0
```

Retrives metadata from a markdown string.

## PARAMETERS

### -Path
Path to markdown file.
Markdown files typically use extension .md



```yaml
Type: String[]
Parameter Sets: FromPath
Aliases: 

Required: True
Position: Named
Default value: 
Accept pipeline input: True (ByPropertyName, ByValue)
Accept wildcard characters: False
```

### -Markdown
String object containing markdown.



```yaml
Type: String
Parameter Sets: FromMarkdownString
Aliases: 

Required: True
Position: Named
Default value: 
Accept pipeline input: False
Accept wildcard characters: False
```

### CommonParameters
This cmdlet supports the common parameters: -Debug, -ErrorAction, -ErrorVariable, -InformationAction, -InformationVariable, -OutVariable, -OutBuffer, -PipelineVariable, -Verbose, -WarningAction, and -WarningVariable. For more information, see about_CommonParameters (http://go.microsoft.com/fwlink/?LinkID=113216).

## INPUTS

### None
You cannot pipe objects into this cmdlet.

## OUTPUTS

### Dictionary[String, String]
The dictionary contains key-value pairs found in the markdown metadata block.

## NOTES

## RELATED LINKS

[Online Version:](https://github.com/PowerShell/platyPS/blob/master/docs/Get-MarkdownMetadata.md)





