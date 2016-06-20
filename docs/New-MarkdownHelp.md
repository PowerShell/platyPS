---
external help file: platyPS-help.xml
schema: 2.0.0
online version: https://github.com/PowerShell/platyPS/blob/master/docs/New-MarkdownHelp.md
---

# New-MarkdownHelp
## SYNOPSIS
Convert your existing external help into markdown or generate it from Help object.
## SYNTAX

### FromModule
```
New-MarkdownHelp -Module <String[]> [-Force] [-Metadata <Hashtable>] -OutputFolder <String> [-NoMetadata]
 [-Encoding <Encoding>] [-WithModulePage] [-Locale <String>] [-HelpVersion <String>] [-FwLink <String>]
 [<CommonParameters>]
```

### FromCommand
```
New-MarkdownHelp -Command <String[]> [-Force] [-Metadata <Hashtable>] [-OnlineVersionUrl <String>]
 -OutputFolder <String> [-NoMetadata] [-Encoding <Encoding>] [<CommonParameters>]
```

### FromMaml
```
New-MarkdownHelp -MamlFile <String[]> [-ConvertNotesToList] [-ConvertDoubleDashLists] [-Force]
 [-Metadata <Hashtable>] -OutputFolder <String> [-NoMetadata] [-Encoding <Encoding>] [-WithModulePage]
 [-Locale <String>] [-HelpVersion <String>] [-FwLink <String>] [-ModuleName <String>] [-ModuleGuid <String>]
 [<CommonParameters>]
```

## DESCRIPTION
An easy way to start with platyPS.
This cmdlet generates help stub from:

-  Module.
-  Command.
-  External help xml (aka maml).
## EXAMPLES

### Example 1 (from command)
```
PS C:\> function foo {param([string]$bar)}
PS C:\> New-MarkdownHelp -command foo -OutputFolder .\docs


    Directory: D:\dev\platyPS\docs


Mode                LastWriteTime         Length Name
----                -------------         ------ ----
-a----        5/22/2016   6:53 PM            664 foo.md
```

Create stub markdown on the fly from the function foo.
### Example 2 (from module)
```
PS C:\> Import-Module platyPS
PS C:\> New-MarkdownHelp -module platyPS -OutputFolder .\docs-new -Force


    Directory: D:\dev\platyPS\docs-new


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

The module should be loaded in the PS Session first.
 Markdown generated for all commands in the module.
### Example 3 (from maml file path)
```
PS C:\> $mamlPath = 'C:\Program Files\WindowsPowerShell\Modules\PSReadline\1.1\en-US\Microsoft.PowerShell.PSReadline.dll-help.xml'
PS C:\> New-MarkdownHelp -OutputFolder .\psreadline-docs -MamlFile $mamlPath


    Directory: D:\dev\platyPS\psreadline-docs


Mode                LastWriteTime         Length Name
----                -------------         ------ ----
-a----        5/22/2016   6:56 PM           7443 Get-PSReadlineKeyHandler.md
-a----        5/22/2016   6:56 PM           3586 Get-PSReadlineOption.md
-a----        5/22/2016   6:56 PM           1549 Remove-PSReadlineKeyHandler.md
-a----        5/22/2016   6:56 PM           5947 Set-PSReadlineKeyHandler.md
-a----        5/22/2016   6:56 PM          15320 Set-PSReadlineOption.md
```

Create markdown help for inbox PSReadLine module.
You don't need to load the module itself to do it.
Only the help file would be used.
### Example 4 (from maml file with module page)
```
PS C:\> $mamlPath = 'C:\Program Files\WindowsPowerShell\Modules\PSReadline\1.1\en-US\Microsoft.PowerShell.PSReadline.dll-help.xml'
PS C:\> New-MarkdownHelp -OutputFolder .\psreadline-docs -MamlFile $mamlPath -WithModulePage  -Force -ModuleName PSReadLine


    Directory: D:\dev\platyPS\psreadline-docs


Mode                LastWriteTime         Length Name
----                -------------         ------ ----
-a----        5/22/2016   6:59 PM           7443 Get-PSReadlineKeyHandler.md
-a----        5/22/2016   6:59 PM           3586 Get-PSReadlineOption.md
-a----        5/22/2016   6:59 PM           1549 Remove-PSReadlineKeyHandler.md
-a----        5/22/2016   6:59 PM           5947 Set-PSReadlineKeyHandler.md
-a----        5/22/2016   6:59 PM          15320 Set-PSReadlineOption.md
-a----        5/22/2016   6:59 PM            942 PSReadLine.md
```

Create markdown help for inbox PSReadLine module.
Create a "ModulePage" with name PSReadLine.md, links to other help files
and metadata needed for creating cab files.
## PARAMETERS

### -Command
Name of a command from your PowerShell session.






```yaml
Type: String[]
Parameter Sets: FromCommand
Aliases:

Required: True
Position: Named
Default value:
Accept pipeline input: False
Accept wildcard characters: False
```

### -Encoding
Character encoding for your markdown help files.

It should be of the type \[System.Text.Encoding\].
You can control [precise details](https://msdn.microsoft.com/en-us/library/ms404377.aspx) about your encoding.
For [example](http://stackoverflow.com/questions/5596982/using-powershell-to-write-a-file-in-utf-8-without-the-bom),
you can control BOM (Byte Order Mark) preferences with it.






```yaml
Type: Encoding
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: UTF8 without BOM
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

### -FwLink
Metadata for module page, to enable cab creation.
It would be used as markdown header metadata in the module page.






```yaml
Type: String
Parameter Sets: FromModule, FromMaml
Aliases:

Required: False
Position: Named
Default value:
Accept pipeline input: False
Accept wildcard characters: False
```

### -HelpVersion
Metadata for module page, to enable cab creation.
It would be used as markdown header metadata in the module page.






```yaml
Type: String
Parameter Sets: FromModule, FromMaml
Aliases:

Required: False
Position: Named
Default value:
Accept pipeline input: False
Accept wildcard characters: False
```

### -Locale
Metadata for module page, to enable cab creation.
It would be used as markdown header metadata in the module page.






```yaml
Type: String
Parameter Sets: FromModule, FromMaml
Aliases:

Required: False
Position: Named
Default value:
Accept pipeline input: False
Accept wildcard characters: False
```

### -MamlFile
Path to a MAML xml external help file.






```yaml
Type: String[]
Parameter Sets: FromMaml
Aliases:

Required: True
Position: Named
Default value:
Accept pipeline input: False
Accept wildcard characters: False
```

### -Metadata
String-to-string hashtable.
It would be writed in a header of every markdown help file.
It would be ignored by New-ExternalHelp, but can be used by external tools.






```yaml
Type: Hashtable
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value:
Accept pipeline input: False
Accept wildcard characters: False
```

### -Module
Name of the modules to be used in markdown help generation.






```yaml
Type: String[]
Parameter Sets: FromModule
Aliases:

Required: True
Position: Named
Default value:
Accept pipeline input: True (ByValue)
Accept wildcard characters: False
```

### -ModuleGuid
Metadata for module page, to enable cab creation.
It would be used as markdown header metadata in the module page.






```yaml
Type: String
Parameter Sets: FromMaml
Aliases:

Required: False
Position: Named
Default value:
Accept pipeline input: False
Accept wildcard characters: False
```

### -ModuleName
Metadata for module page, to enable cab creation.
It would be used to name module page.






```yaml
Type: String
Parameter Sets: FromMaml
Aliases:

Required: False
Position: Named
Default value:
Accept pipeline input: False
Accept wildcard characters: False
```

### -NoMetadata
Don't emit any metadata in generated markdown.





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

### -OnlineVersionUrl
The URL where help can be downloaded using the updatable help function in PowerShell.
Empty string would be used, if no value provided.




```yaml
Type: String
Parameter Sets: FromCommand
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
Position: Named
Default value:
Accept pipeline input: False
Accept wildcard characters: False
```

### -WithModulePage
Generate module page in the output directory.




```yaml
Type: SwitchParameter
Parameter Sets: FromModule, FromMaml
Aliases:

Required: False
Position: Named
Default value:
Accept pipeline input: False
Accept wildcard characters: False
```

### -ConvertNotesToList
Add bullet list into notes section to match TechNet format.```yaml
Type: SwitchParameter
Parameter Sets: FromMaml
Aliases:

Required: False
Position: Named
Default value: False
Accept pipeline input: False
Accept wildcard characters: False
```

### -ConvertDoubleDashLists
Convert two-hypen lists (common in MS-authored MAML) into one-hypen list (accepted in markdown).```yaml
Type: SwitchParameter
Parameter Sets: FromMaml
Aliases:

Required: False
Position: Named
Default value: False
Accept pipeline input: False
Accept wildcard characters: False
```

### CommonParameters
This cmdlet supports the common parameters: -Debug, -ErrorAction, -ErrorVariable, -InformationAction, -InformationVariable, -OutVariable, -OutBuffer, -PipelineVariable, -Verbose, -WarningAction, and -WarningVariable. For more information, see about_CommonParameters (http://go.microsoft.com/fwlink/?LinkID=113216).
## INPUTS

### String[]
You can pipe module names to this cmdlet.
## OUTPUTS

### System.IO.FileInfo[]
This cmdlet returns a FileInfo[] object for created files.
## NOTES

## RELATED LINKS

[Online Version:](https://github.com/PowerShell/platyPS/blob/master/docs/New-MarkdownHelp.md)
