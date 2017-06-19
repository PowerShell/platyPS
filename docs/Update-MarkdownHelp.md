---
external help file: platyPS-help.xml
Module Name: platyPS
online version: https://github.com/PowerShell/platyPS/blob/master/docs/Update-MarkdownHelp.md
schema: 2.0.0
---

# Update-MarkdownHelp

## SYNOPSIS
Update PlatyPS markdown help files.

## SYNTAX

```
Update-MarkdownHelp [-Path] <String[]> [[-Encoding] <Encoding>] [[-LogPath] <String>] [-LogAppend]
 [-AlphabeticParamsOrder] [-UseFullTypeName] [<CommonParameters>]
```

## DESCRIPTION
The **Update-MarkdownHelp** cmdlet updates PlatyPS markdown help files without completely replacing the content of the files.

Some parameter attributes change over time.
For instance, parameter sets, types, default value, and required.
This cmdlet updates markdown help to reflect those changes.
It also adds placeholder text to the markdown file for any new parameter.

To propagate changes to your markdown help files, do the following:

- Load the new version of the module into your Windows PowerShell session.
- Run the **Update-MarkdownHelp** cmdlet to update the files.
- Check new parameters metadata in the markdown files.

## EXAMPLES

### Example 1: Update all files in a folder
```
PS C:\> Update-MarkdownHelp -Path ".\docs"

    Directory: D:\working\PlatyPS\docs


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

This command updates all markdown help files in the specified path to match the current cmdlets.

### Example 2: Update one file and capture log
```
PS C:\> Update-MarkdownHelp -Path ".\docs\Update-MarkdownHelp.md" -LogPath ".\markdown.log"

    Directory: D:\Working\PlatyPS\docs


Mode                LastWriteTime         Length Name
----                -------------         ------ ----
-a----        5/22/2016   8:20 PM           9993 New-MarkdownHelp.md
```

This command updates a markdown help file.
It writes log information to the markdown.log file.

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
Position: 1
Default value: UTF8 without BOM
Accept pipeline input: False
Accept wildcard characters: False
```

### -LogAppend
Indicates that this cmdlet appends information to the log instead overwriting it.


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

### -LogPath
Specifies a file path for log information.
The cmdlet writes the VERBOSE stream to the log.
If you specify the *Verbose* parameter, this cmdlet also writes that information to the console. 


```yaml
Type: String
Parameter Sets: (All)
Aliases: 

Required: False
Position: 2
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
Accept wildcard characters: True
```

### -AlphabeticParamsOrder
Order parameters alphabetically by name in PARAMETERS section.
There are 2 exceptions: -Confirm and -WhatIf parameters will be the last. 
These parameters are common and hence have well-defined behavior.

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

### -UseFullTypeName
Indicates that the target document will use a full type name instead of a short name for parameters.

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

