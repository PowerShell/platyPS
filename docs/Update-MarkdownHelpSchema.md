---
external help file: platyPS-help.xml
schema: 2.0.0
online version: https://github.com/PowerShell/platyPS/blob/master/docs/Update-MarkdownHelpSchema.md
---

# Update-MarkdownHelpSchema
## SYNOPSIS
Upgrade markdown help from version 1.0.0 to the latest one (2.0.0).
## SYNTAX

```
Update-MarkdownHelpSchema [-Path] <String[]> [-OutputFolder] <String> [[-Encoding] <Encoding>] [-Force]
 [<CommonParameters>]
```

## DESCRIPTION
Migrate to the latest markdown help schema.

As a release 0.4.0, there are two schemas: 1.0.0 and 2.0.0
We highly encourage you to migrate to the latest schema.

It's easlier and provides visually apealing expirience.
## EXAMPLES

### Example 1
```
PS C:\> Update-MarkdownHelpSchema .\Examples\PSReadLine.dll-help.md -OutputFolder .\PSReadLine


    Directory: D:\dev\platyPS\PSReadLine


Mode                LastWriteTime         Length Name
----                -------------         ------ ----
-a----        5/22/2016   8:47 PM           1410 Get-PSReadlineKeyHandler.md
-a----        5/22/2016   8:47 PM            648 Get-PSReadlineOption.md
-a----        5/22/2016   8:47 PM           4091 Set-PSReadlineKeyHandler.md
-a----        5/22/2016   8:47 PM          10964 Set-PSReadlineOption.md
```

Upgrade PSReadLine platyPS markdown from version 1.0.0 to the latest one (2.0.0).
## PARAMETERS

### -Encoding
Character encoding for created markdown help files.

It should be of the type \[System.Text.Encoding\].
You can control [precise details](https://msdn.microsoft.com/en-us/library/ms404377.aspx) about your encoding.
For [example](http://stackoverflow.com/questions/5596982/using-powershell-to-write-a-file-in-utf-8-without-the-bom),
you can control BOM (Byte Order Mark) preferences with it.




```yaml
Type: Encoding
Parameter Sets: (All)
Aliases:

Required: False
Position: 2
Default value:
Accept pipeline input: False
Accept wildcard characters: False
```

### -Force
Override existing files.




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

### -OutputFolder
Path to the directory to output markdown help files.




```yaml
Type: String
Parameter Sets: (All)
Aliases:

Required: True
Position: 1
Default value:
Accept pipeline input: False
Accept wildcard characters: False
```

### -Path
Path to markdown files or directories.




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
This cmdlet returns a FileInfo[] object for created files.
## NOTES

## RELATED LINKS

[Online Version:](https://github.com/PowerShell/platyPS/blob/master/docs/Update-MarkdownHelpSchema.md)
