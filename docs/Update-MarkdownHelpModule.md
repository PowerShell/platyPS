---
external help file: platyPS-help.xml
schema: 2.0.0
---

# Update-MarkdownHelpModule
## SYNOPSIS
Update all files in a markdown help module folder.
## SYNTAX

```
Update-MarkdownHelpModule [-Path] <String[]> [[-Encoding] <Encoding>] [[-LogPath] <String>] [-LogAppend]
 [<CommonParameters>]
```

## DESCRIPTION
This cmdlet provides a way to update existing help and create new markdown for newly created commands.

It combines Update-MarkdownHelp and New-MarkdownHelp and provides a convinient way to do a bulk update.
## EXAMPLES

### Example 1
```
PS C:\> Update-MarkdownHelpModule .\docs

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
Create help markdown for commands, that didn't have them before.
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
Path to markdown help module folder.
Folder should contain module page to retrive module name.


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

### System.String[]
You can pipe a collection of paths to this cmdlet.
## OUTPUTS

### System.IO.FileInfo[]
This cmdlet returns a FileInfo[] object for updated and newly created files.
## NOTES

## RELATED LINKS

[Online Version:](https://github.com/PowerShell/platyPS/blob/master/docs/Update-MarkdownHelpModule.md)



