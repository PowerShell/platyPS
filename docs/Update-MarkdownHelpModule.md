---
external help file: platyPS-help.xml
Module Name: platyPS
online version: https://github.com/PowerShell/platyPS/blob/master/docs/Update-MarkdownHelpModule.md
schema: 2.0.0
---

# Update-MarkdownHelpModule

## SYNOPSIS
Update all files in a markdown help module folder.

## SYNTAX

```
Update-MarkdownHelpModule [-Path] <String[]> [[-Encoding] <Encoding>] [-RefreshModulePage]
 [-ModulePagePath <String>] [[-LogPath] <String>] [-LogAppend] [-AlphabeticParamsOrder] [-UseFullTypeName]
 [-UpdateInputOutput] [-Force] [-Session <PSSession>] [-ExcludeDontShow] [<CommonParameters>]
```

## DESCRIPTION
The **Update-MarkdownHelpModule** cmdlet updates existing help markdown files and creates markdown files for new cmdlets in a module.
This cmdlet combines functionality of the [Update-MarkdownHelp](Update-MarkdownHelp.md) and [New-MarkdownHelp](New-MarkdownHelp.md) cmdlets to perform a bulk update.

## EXAMPLES

### Example 1: Update a markdown help module
```
PS C:\> Update-MarkdownHelpModule -Path ".\docs"

    Directory: D:\Working\PlatyPS\docs


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

This command updates all the files in the specified folder based on the cmdlets as loaded into your current session.
The command creates markdown help topics for any cmdlets that are not already included in the .\docs folder.

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
Specifies an array of paths of markdown folders to update.
The folder must contain a module page from which this cmdlet can get the module name.

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

### -RefreshModulePage
Update module page when updating the help module.

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

### -AlphabeticParamsOrder
Order parameters alphabetically by name in PARAMETERS section.
There are 5 exceptions: -Confirm, -WhatIf, -IncludeTotalCount, -Skip, and -First parameters will be the last.
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

### -Session
Provides support for remote commands.
Pass the session that you used to create the commands with `Import-PSSession`.
This is required to get accurate parameters metadata from the remote session.

```yaml
Type: PSSession
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

### -UpdateInputOutput
Refreshes the Input and Output sections to reflect the current state of the cmdlet.  WARNING: this parameter will remove any manual additions to these sections.

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

### -ModulePagePath
When -RefreshModulePage is used by default it puts .md file in same location as all other docs. With this parameter you can specify new name/location providing better placement options.

```yaml
Type: String
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -Force
Remove help files that no longer exists within sessions (for example if function was deleted)

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

### -ExcludeDontShow
Exclude the parameters marked with `DontShow` in the parameter attribute from the help content.

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
This cmdlet supports the common parameters: -Debug, -ErrorAction, -ErrorVariable, -InformationAction, -InformationVariable, -OutVariable, -OutBuffer, -PipelineVariable, -Verbose, -WarningAction, and -WarningVariable. For more information, see [about_CommonParameters](http://go.microsoft.com/fwlink/?LinkID=113216).

## INPUTS

### System.String[]
You can pipe an array of paths to this cmdlet.

## OUTPUTS

### System.IO.FileInfo[]
This cmdlet returns a **FileInfo[]** object for updated and new files.

## NOTES
The module for which you want to update the help should first be imported from the location containing the previous version of the help.
 If this condition is not met, the parameter order will be alphabetical in the updated help, even if the parameter *AlphabeticParamsOrder* has not been used.

## RELATED LINKS

[Character Encoding in the .NET Framework](https://msdn.microsoft.com/en-us/library/ms404377.aspx)

[Using PowerShell to write a file in UTF-8 without the BOM](http://stackoverflow.com/questions/5596982/using-powershell-to-write-a-file-in-utf-8-without-the-bom)
