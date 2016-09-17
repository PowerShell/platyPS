---
external help file: platyPS-help.xml
online version: https://github.com/PowerShell/platyPS/blob/master/docs/Update-MarkdownHelpSchema.md
schema: 2.0.0
---

# Update-MarkdownHelpSchema

## SYNOPSIS
Migrates markdown help files to the latest markdown help schema.

## SYNTAX

```
Update-MarkdownHelpSchema [-Path] <String[]> [-OutputFolder] <String> [[-Encoding] <Encoding>] [-Force]
 [<CommonParameters>]
```

## DESCRIPTION
The **Update-MarkdownHelpSchema** cmdlet migrates markdown help files to the latest PlatyPS markdown help schema.
We recommend you update to the latest schema.

Currently, there are two schemas: 1.0.0 and 2.0.0.

## EXAMPLES

### Example 1: Update to current schema
```
PS C:\> Update-MarkdownHelpSchema -Path ".\Examples\PSReadLine.dll-help.md" -OutputFolder ".\PSReadLine"


    Directory: D:\Working\PlatyPS\PSReadLine


Mode                LastWriteTime         Length Name
----                -------------         ------ ----
-a----        5/22/2016   8:47 PM           1410 Get-PSReadlineKeyHandler.md
-a----        5/22/2016   8:47 PM            648 Get-PSReadlineOption.md
-a----        5/22/2016   8:47 PM           4091 Set-PSReadlineKeyHandler.md
-a----        5/22/2016   8:47 PM          10964 Set-PSReadlineOption.md
```

This commad upgrades the PSReadLine platyPS markdown to the latest version, which is currently 2.0.0.

## PARAMETERS

### -Encoding
Specifies the character encoding for your markdown help files.
Specify a **System.Text.Encoding** object.
For more information, see [Character Encoding in the .NET Framework](https://msdn.microsoft.com/en-us/library/ms404377.aspx) in the Microsoft Developer Network.
For example, you can control Byte Order Mark (BOM) preferences.
For more information, see [Using PowerShell to write a file in UTF-8 without the BOM](http://stackoverflow.com/questions/5596982/using-powershell-to-write-a-file-in-utf-8-without-the-bom) at the Stack Overflow community.


```yaml
Type: Encoding
Parameter Sets: (All)
Aliases: 

Required: False
Position: 2
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -Force
Indicates that this cmdlet overwrites existing files that have the same names.


```yaml
Type: SwitchParameter
Parameter Sets: (All)
Aliases: 

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -OutputFolder
Specifies the folder where this cmdlet saves updated markdown help files.


```yaml
Type: String
Parameter Sets: (All)
Aliases: 

Required: True
Position: 1
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -Path
Specifies an array of paths of markdown files and folders to update.


```yaml
Type: String[]
Parameter Sets: (All)
Aliases: 

Required: True
Position: 0
Default value: None
Accept pipeline input: True (ByValue)
Accept wildcard characters: False
```

### CommonParameters
This cmdlet supports the common parameters: -Debug, -ErrorAction, -ErrorVariable, -InformationAction, -InformationVariable, -OutVariable, -OutBuffer, -PipelineVariable, -Verbose, -WarningAction, and -WarningVariable. For more information, see about_CommonParameters (http://go.microsoft.com/fwlink/?LinkID=113216).

## INPUTS

### String[]
You can pipe an array of paths to this cmdlet. 

## OUTPUTS

### System.IO.FileInfo[]
This cmdlet returns a **FileInfo[]** object for updated files. 

## NOTES

## RELATED LINKS

[Character Encoding in the .NET Framework](https://msdn.microsoft.com/en-us/library/ms404377.aspx)

[Using PowerShell to write a file in UTF-8 without the BOM](http://stackoverflow.com/questions/5596982/using-powershell-to-write-a-file-in-utf-8-without-the-bom)
