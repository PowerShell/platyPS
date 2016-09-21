---
external help file: platyPS-help.xml
online version: https://github.com/PowerShell/platyPS/blob/master/docs/New-ExternalHelp.md
schema: 2.0.0
---

# New-ExternalHelp

## SYNOPSIS
Creates external help file based on markdown supported by PlatyPS.

## SYNTAX

```
New-ExternalHelp -Path <String[]> -OutputPath <String> [-Encoding <Encoding>] [-Force] [<CommonParameters>]
```

## DESCRIPTION
The **New-ExternalHelp** cmdlet creates an external help file based on markdown help files supported by PlatyPS.
You can ship this with a module to provide help by using the **Get-Help** cmdlet.

If the markdown files that you specify do not follow the PlatyPS [Schema](https://github.com/PowerShell/platyPS/blob/master/platyPS.schema.md), this cmdlet returns error messages.

## EXAMPLES

### Example 1: Create external help based on the contents of a folder
```
PS C:\> New-ExternalHelp -Path ".\docs" -OutputPath "out\platyPS\en-US"

    Directory: D:\Working\PlatyPS\out\platyPS\en-US


Mode                LastWriteTime         Length Name
----                -------------         ------ ----
-a----        5/19/2016  12:32 PM          46776 platyPS-help.xml
```

This command creates an external help file in the specified location.
This command uses the best practice that the folder name includes the locale.

### Example 2: Create help that uses custom encoding
```
PS C:\> New-ExternalHelp -Path ".\docs" -OutputPath "out\PlatyPS\en-US" -Force -Encoding ([System.Text.Encoding]::Unicode)


    Directory: D:\Working\PlatyPS\out\PlatyPS\en-US


Mode                LastWriteTime         Length Name
----                -------------         ------ ----
-a----        5/22/2016   6:34 PM         132942 platyPS-help.xml
```

This command creates an external help file in the specified location.
This command specifies the *Force* parameter, therefore, it overwrites an existing file.
The command specifies Unicode encoding for the created file.

## PARAMETERS

### -OutputPath
Specifies the path of a folder where this cmdlet saves your external help file.
The folder name should end with a locale folder, as in the following example: `.\out\PlatyPS\en-US\`.

```yaml
Type: String
Parameter Sets: (All)
Aliases: 

Required: True
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -Encoding
Specifies the character encoding for your external help file.
Specify a **System.Text.Encoding** object.
For more information, see [Character Encoding in the .NET Framework](https://msdn.microsoft.com/en-us/library/ms404377.aspx) in the Microsoft Developer Network.
For example, you can control Byte Order Mark (BOM) preferences.
For more information, see [Using PowerShell to write a file in UTF-8 without the BOM](http://stackoverflow.com/questions/5596982/using-powershell-to-write-a-file-in-utf-8-without-the-bom) at the Stack Overflow community.


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
Indicates that this cmdlet overwrites an existing file that has the same name.

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

### -Path
Specifies an array of paths of markdown files or folders.
This cmdlet creates external help based on these files and folders.

```yaml
Type: String[]
Parameter Sets: (All)
Aliases: 

Required: True
Position: Named
Default value: None
Accept pipeline input: True (ByPropertyName, ByValue)
Accept wildcard characters: False
```

### CommonParameters
This cmdlet supports the common parameters: -Debug, -ErrorAction, -ErrorVariable, -InformationAction, -InformationVariable, -OutVariable, -OutBuffer, -PipelineVariable, -Verbose, -WarningAction, and -WarningVariable. For more information, see about_CommonParameters (http://go.microsoft.com/fwlink/?LinkID=113216).

## INPUTS

### String[]
You can pipe an array of paths to this cmdlet.

## OUTPUTS

### System.IO.FileInfo[]
This cmdlet returns a **FileInfo[]** object for created files.

## NOTES

## RELATED LINKS

[PowerShell V2 External MAML Help](https://blogs.msdn.microsoft.com/powershell/2008/12/24/powershell-v2-external-maml-help/)

[Schema](https://github.com/PowerShell/platyPS/blob/master/platyPS.schema.md)
