---
external help file: platyPS-help.xml
Module Name: platyPS
online version: https://github.com/PowerShell/platyPS/blob/master/docs/Get-MarkdownMetadata.md
schema: 2.0.0
---

# Get-MarkdownMetadata

## SYNOPSIS
Gets metadata from the header of a markdown file.

## SYNTAX

### FromPath (Default)
```
Get-MarkdownMetadata -Path <String[]> [<CommonParameters>]
```

### FromMarkdownString
```
Get-MarkdownMetadata -Markdown <String> [<CommonParameters>]
```

## DESCRIPTION
The **Get-MarkdownMetadata** cmdlet gets the metadata from the header of a markdown file that is supported by PlatyPS.
The command returns the metadata as a hash table.

PlatyPS stores metadata in the header block of a markdown file as key-value pairs of strings.
By default, PlatyPS stores help file name and markdown schema version.

Metadata section can contain user-provided values for use with external tools.
The [New-ExternalHelp](New-ExternalHelp.md) cmdlet ignores this metadata.

## EXAMPLES

### Example 1: Get metadata from a file
```
PS C:\> Get-MarkdownMetadata -Path ".\docs\Get-MarkdownMetadata.md"

Key                Value
---                -----
external help file platyPS-help.xml
schema             2.0.0
```

This command retrieves metadata from a markdown file.

### Example 2: Get metadata from a markdown string
```
PS C:\> $Markdown = Get-Content -Path ".\docs\Get-MarkdownMetadata.md" -Raw
PS C:\> Get-MarkdownMetadata -Markdown $Markdown

Key                Value
---                -----
external help file platyPS-help.xml
schema             2.0.0
```

The first command gets the contents of a file, and stores them in the $Markdown variable.

The second command retrieves metadata from the string in $Metadata.

### Example 3: Get metadata from all files in a folder
```
PS C:\> Get-MarkdownMetadata -Path ".\docs"

Key                Value
---                -----
external help file platyPS-help.xml
schema             2.0.0
external help file platyPS-help.xml
schema             2.0.0
external help file platyPS-help.xml
schema             2.0.0
external help file platyPS-help.xml
schema             2.0.0
external help file platyPS-help.xml
schema             2.0.0
external help file platyPS-help.xml
schema             2.0.0
external help file platyPS-help.xml
schema             2.0.0
external help file platyPS-help.xml
schema             2.0.0
```

This command gets metadata from each of the markdown files in the .\docs folder.

## PARAMETERS

### -Path
Specifies an array of paths of markdown files or folders.

```yaml
Type: String[]
Parameter Sets: FromPath
Aliases:

Required: True
Position: Named
Default value: None
Accept pipeline input: True (ByPropertyName, ByValue)
Accept wildcard characters: True
```

### -Markdown
Specifies a string that contains markdown formatted text.

```yaml
Type: String
Parameter Sets: FromMarkdownString
Aliases:

Required: True
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### CommonParameters
This cmdlet supports the common parameters: -Debug, -ErrorAction, -ErrorVariable, -InformationAction, -InformationVariable, -OutVariable, -OutBuffer, -PipelineVariable, -Verbose, -WarningAction, and -WarningVariable. For more information, see [about_CommonParameters](http://go.microsoft.com/fwlink/?LinkID=113216).

## INPUTS

### String[]
You can pipe an array of paths to this cmdlet.

## OUTPUTS

### Dictionary[String, String]
The cmdlet returns a **Dictionary\[String, String\]** object.
The dictionary contains key-value pairs found in the markdown metadata block.

## NOTES

## RELATED LINKS
