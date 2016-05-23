---
external help file: platyPS-help.xml
schema: 2.0.0
---

# Update-MarkdownHelp
## SYNOPSIS
Update platyPS markdown help files in place.
## SYNTAX

```
Update-MarkdownHelp [-Path] <String[]> [[-Encoding] <Encoding>] [[-LogPath] <String>] [-LogAppend] [<CommonParameters>]
```

## DESCRIPTION
Update platyPS markdown help files in place.
Files content would be alternated.

Some parameters attributes changes over time, i.e. parameter sets, types, default value, required, etc.

Automatically propogate these changes to your markdown help file:

- Load new version of the module in your PowerShell session.
- Call Update-MarkdownHelp.
- Check new parameters metadata in the markdown files.

It also will contain placeholders for new parameters to speed-up your help-authoring expirience.

## EXAMPLES

### Example 1 (Update all files in a folder)
```
PS C:\> Update-MarkdownHelp .\docs

    Directory: D:\dev\platyPS\docs


Mode                LastWriteTime         Length Name
----                -------------         ------ ----
-a----        5/22/2016   6:54 PM           1496 Get-HelpPreview.md
-a----        5/22/2016   6:54 PM           3208 Get-MarkdownMetadata.md
-a----        5/22/2016   6:54 PM           3059 New-ExternalHelp.md
-a----        5/22/2016   6:54 PM           2702 New-ExternalHelpCab.md
-a----        5/22/2016   6:54 PM           6234 New-MarkdownHelp.md
-a----        5/22/2016   6:54 PM           2346 Update-MarkdownHelp.md
-a----        5/22/2016   6:54 PM           1633 Update-MarkdownHelpModule.md
-a----        5/22/2016   6:54 PM           1630 Update-MarkdownHelpSchema.md

```

Update all markdown files is a folder with information from the 'live' commands.

### Example 2 (Update one file and capture log)
```
PS C:\> Update-MarkdownHelp .\docs\Update-MarkdownHelp.md -LogPath .\my.log

    Directory: D:\dev\platyPS\docs


Mode                LastWriteTime         Length Name
----                -------------         ------ ----
-a----        5/22/2016   8:20 PM           9993 New-MarkdownHelp.md

```

Update markdown help file and write log to "my.log" file. 

## PARAMETERS

### -Encoding
Character encoding for your updated markdown help files.

It should be of the type \[System.Text.Encoding\].
You can control [precise details](https://msdn.microsoft.com/en-us/library/ms404377.aspx) about your encoding.
For [example](http://stackoverflow.com/questions/5596982/using-powershell-to-write-a-file-in-utf-8-without-the-bom), 
you can control BOM (Byte Order Mark) preferences with it.

```yaml
Type: Encoding
Parameter Sets: (All)
Aliases: 

Required: False
Position: 1
Default value: UTF8 without BOM
Accept pipeline input: False
Accept wildcard characters: False
```

### -LogAppend
Don't overwrite log file, instead append to it.

```yaml
Type: SwitchParameter
Parameter Sets: (All)
Aliases: 

Required: False
Position: Named
Default value: 
Accept pipeline input: False
Accept wildcard characters: False
```

### -LogPath
Put log information into a provided file path.
By default, VERBOSE stream is used for it.

```yaml
Type: String
Parameter Sets: (All)
Aliases: 

Required: False
Position: 2
Default value: 
Accept pipeline input: False
Accept wildcard characters: False
```

### -Path
Path to markdown files or folder.

```yaml
Type: String[]
Parameter Sets: (All)
Aliases: 

Required: True
Position: 0
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

### System.IO.FileInfo[]
This cmdlet returns a FileInfo[] object for updated files.

## NOTES

## RELATED LINKS

[Online Version:](https://github.com/PowerShell/platyPS/blob/master/docs/Update-MarkdownHelp.md)





