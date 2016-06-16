---
external help file: platyPS-help.xml
schema: 2.0.0
---

# New-MarkdownHelp
## SYNOPSIS
Creates help in markdown format.

## SYNTAX

### UNNAMED_PARAMETER_SET_1
```
New-MarkdownHelp -Command <String[]> [-Encoding <Encoding>] [-Force] [-Metadata <Hashtable>] [-NoMetadata]
 [-OnlineVersionUrl <String>] -OutputFolder <String>
```

### UNNAMED_PARAMETER_SET_2
```
New-MarkdownHelp [-Encoding <Encoding>] [-Force] [-FwLink <String>] [-HelpVersion <String>] [-Locale <String>]
 [-Metadata <Hashtable>] -Module <String[]> [-NoMetadata] -OutputFolder <String> [-WithModulePage]
```

### UNNAMED_PARAMETER_SET_3
```
New-MarkdownHelp [-Encoding <Encoding>] [-Force] [-FwLink <String>] [-HelpVersion <String>] [-Locale <String>]
 -MamlFile <String[]> [-Metadata <Hashtable>] [-ModuleGuid <String>] [-ModuleName <String>] [-NoMetadata]
 -OutputFolder <String> [-WithModulePage]
```

## DESCRIPTION
The **New-MarkdownHelp** cmdlet creates help in markdown format based on a module, a command, or a file in Microsoft Assistance Markup Language (MAML) format.
## EXAMPLES

### Example 1: Create help from a command
```
PS C:\> function Command03 {param([string]$Value)}
PS C:\> New-MarkdownHelp -Command "Command03" -OutputFolder ".\docs"


    Directory: D:\Working\docs


Mode                LastWriteTime         Length Name
----                -------------         ------ ----
-a----        5/22/2016   6:53 PM            664 Command03.md
```

The first command creates a function named Command03 by using standard Windows PowerShell syntax.

The second command creates help for that stub function in the .\docs folder.

### Example 2: Create help from a module
```
PS C:\> Import-Module -Module "PlatyPS"
PS C:\> New-MarkdownHelp -Module "PlatyPS" -OutputFolder ".\docs" -Force


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

The first command loads the PlatyPS module into the current session by using the **Import-Module** cmdlet.

The second command creates help for all the cmdlets in the PlatyPS module. It stores them in the .\docs folder. This command specifies the *Force* parameter. Therefore, it overwrites existing help markdown files that have the same name.  


### Example 3: Create help from an existing MAML file
```
PS C:\> New-MarkdownHelp -OutputFolder "D:\PSReadline\docs" -MamlFile 'C:\Program Files\WindowsPowerShell\Modules\PSReadline\1.1\en-US\Microsoft.PowerShell.PSReadline.dll-help.xml'

    Directory: D:\PSReadline\docs


Mode                LastWriteTime         Length Name
----                -------------         ------ ----
-a----        5/22/2016   6:56 PM           7443 Get-PSReadlineKeyHandler.md
-a----        5/22/2016   6:56 PM           3586 Get-PSReadlineOption.md
-a----        5/22/2016   6:56 PM           1549 Remove-PSReadlineKeyHandler.md
-a----        5/22/2016   6:56 PM           5947 Set-PSReadlineKeyHandler.md
-a----        5/22/2016   6:56 PM          15320 Set-PSReadlineOption.md
```

This command creates help in markdown format for the specified help MAML file. You do not have to load the module, as in the previous example. If the module is already loaded, this command creates help based on the MAML file, not on the currently installed module.


### Example 4: Create help from an existing MAML file for use in a CAB file
```
PS C:\> New-MarkdownHelp -OutputFolder "D:\PSReadline\docs" -MamlFile 'C:\Program Files\WindowsPowerShell\Modules\PSReadline\1.1\en-US\Microsoft.PowerShell.PSReadline.dll-help.xml' -WithModulePage  -Force -ModuleName "PSReadLine"


    Directory: D:\PSReadline\docs


Mode                LastWriteTime         Length Name
----                -------------         ------ ----
-a----        5/22/2016   6:59 PM           7443 Get-PSReadlineKeyHandler.md
-a----        5/22/2016   6:59 PM           3586 Get-PSReadlineOption.md
-a----        5/22/2016   6:59 PM           1549 Remove-PSReadlineKeyHandler.md
-a----        5/22/2016   6:59 PM           5947 Set-PSReadlineKeyHandler.md
-a----        5/22/2016   6:59 PM          15320 Set-PSReadlineOption.md
-a----        5/22/2016   6:59 PM            942 PSReadLine.md
```

This command creates help in markdown format for the specified help MAML file, as in the previous example. This command also specifies the *WithModulePage* parameter and the *ModuleName* parameter. The command creates a file named PSReadLine.md that contains links to the other markdown files in this module and metadata that can be used to create .cab files.

## PARAMETERS

### -Command
Specifies the name of a command in your current session. This can be any command supported by Windows PowerShell help, such as a cmdlet or a function.

```yaml
Type: String[]
Parameter Sets: UNNAMED_PARAMETER_SET_1
Aliases:

Required: True
Position: Named
Default value:
Accept pipeline input: False
Accept wildcard characters: False
```

### -Encoding
Specifies the character encoding for your markdown help files. Specify a **System.Text.Encoding** object. For more information, see [Character Encoding in the .NET Framework](https://msdn.microsoft.com/en-us/library/ms404377.aspx) in the Microsoft Developer Network. For example, you can control Byte Order Mark (BOM) preferences. For more information, see [Using PowerShell to write a file in UTF-8 without the BOM](http://stackoverflow.com/questions/5596982/using-powershell-to-write-a-file-in-utf-8-without-the-bom) at the Stack Overflow community.


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
Indicates that this cmdlet overwrites existing files that have the same names.

```yaml
Type: SwitchParameter
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: False
Accept pipeline input: False
Accept wildcard characters: False
```

### -FwLink
Specifies the forward link for the module page. This value is required for .cab file creation. This value is used as markdown header metadata in the module page.

```yaml
Type: String
Parameter Sets: UNNAMED_PARAMETER_SET_2, UNNAMED_PARAMETER_SET_3
Aliases:

Required: False
Position: Named
Default value:
Accept pipeline input: False
Accept wildcard characters: False
```

### -HelpVersion
Specifies the version of your help. This value is required for .cab file creation. This value is used as markdown header metadata in the module page.

```yaml
Type: String
Parameter Sets: UNNAMED_PARAMETER_SET_2, UNNAMED_PARAMETER_SET_3
Aliases:

Required: False
Position: Named
Default value:
Accept pipeline input: False
Accept wildcard characters: False
```

### -Locale
Specifies the locale of your help. This value is required for .cab file creation. This value is used as markdown header metadata in the module page.

```yaml
Type: String
Parameter Sets: UNNAMED_PARAMETER_SET_2, UNNAMED_PARAMETER_SET_3
Aliases:

Required: False
Position: Named
Default value:
Accept pipeline input: False
Accept wildcard characters: False
```

### -MamlFile
Specifies an array of paths path of MAML xml help files.

```yaml
Type: String[]
Parameter Sets: UNNAMED_PARAMETER_SET_3
Aliases:

Required: True
Position: Named
Default value:
Accept pipeline input: False
Accept wildcard characters: False
```

### -Metadata
Specifies metadata that this cmdlet includes in the help markdown files as a hash table of string-to-sting key-value pairs. This cmdlet writes the metadata in the header of each markdown help file.

The **New-ExternalHelp** cmdlet does not use this metadata. External tools can use this metadata.

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
Specifies an array of names of modules for which this cmdlet creates help in markdown format.

```yaml
Type: String[]
Parameter Sets: UNNAMED_PARAMETER_SET_2
Aliases:

Required: True
Position: Named
Default value:
Accept pipeline input: True (ByValue)
Accept wildcard characters: False
```

### -ModuleGuid
Specifies the GUID of the module of your help. This value is required for .cab file creation. This value is used as markdown header metadata in the module page.


```yaml
Type: String
Parameter Sets: UNNAMED_PARAMETER_SET_3
Aliases:

Required: False
Position: Named
Default value:
Accept pipeline input: False
Accept wildcard characters: False
```

### -ModuleName
Specifies the name of the module of your help. This value is required for .cab file creation. This value is used as markdown header metadata in the module page.


```yaml
Type: String
Parameter Sets: UNNAMED_PARAMETER_SET_3
Aliases:

Required: False
Position: Named
Default value:
Accept pipeline input: False
Accept wildcard characters: False
```

### -NoMetadata
Indicates that this cmdlet does not write any metadata in the generated markdown.

```yaml
Type: SwitchParameter
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: False
Accept pipeline input: False
Accept wildcard characters: False
```

### -OnlineVersionUrl
Specifies the URL where the updatable help function downloads updated help. If you do not specify a value, the cmdlet uses an empty string.

```yaml
Type: String
Parameter Sets: UNNAMED_PARAMETER_SET_1
Aliases:

Required: False
Position: Named
Default value:
Accept pipeline input: False
Accept wildcard characters: False
```

### -OutputFolder
Specifies the folder where this cmdlet creates markdown help files.

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
Indicates that this cmdlet creates a module page in the output folder. This file has the name that the *ModuleName* parameter specifies. If you did not specify that parameter, the cmdlet supplies the default name MamlModule.

```yaml
Type: SwitchParameter
Parameter Sets: UNNAMED_PARAMETER_SET_2, UNNAMED_PARAMETER_SET_3
Aliases:

Required: False
Position: Named
Default value: False
Accept pipeline input: False
Accept wildcard characters: False
```

## INPUTS

### String[]
You can pipe module names to this cmdlet. These are the modules from which this cmdlet creates help markdown.

## OUTPUTS

### System.IO.FileInfo[]
This cmdlet returns a **FileInfo\[\]** object for created files.

## NOTES

## RELATED LINKS

[Online Version:](https://github.com/PowerShell/platyPS/blob/master/docs/New-MarkdownHelp.md)

[Import-Module]

[New-ExternalHelp](New-ExternalHelp.md)

[Update-MarkdownHelp](Update-MarkdownHelp.md)

[Character Encoding in the .NET Framework](https://msdn.microsoft.com/en-us/library/ms404377.aspx)

[Using PowerShell to write a file in UTF-8 without the BOM](http://stackoverflow.com/questions/5596982/using-powershell-to-write-a-file-in-utf-8-without-the-bom)
