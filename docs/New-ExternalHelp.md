---
external help file: platyPS-help.xml
schema: 2.0.0
online version: https://github.com/PowerShell/platyPS/blob/master/docs/New-ExternalHelp.md
---

# New-ExternalHelp
## SYNOPSIS
Create External help xml file from platyPS markdown.
Ship it with your module to provide [Get-Help](https://msdn.microsoft.com/en-us/library/dd878343.aspx) capability.
## SYNTAX

```
New-ExternalHelp -Path <String[]> -OutputPath <String> [-Encoding <Encoding>] [-Force] [<CommonParameters>]
```

## DESCRIPTION
Create External help file from platyPS markdown.

You will get error messages if the markdown files do not follow the schema described in
[platyPS.schema.md](https://github.com/PowerShell/platyPS/blob/master/platyPS.schema.md).
## EXAMPLES

### Example 1 (Markdown folder)
```
PS C:\> New-ExternalHelp -Path .\docs -OutputPath out\platyPS\en-US

    Directory: D:\dev\platyPS\out\platyPS\en-US


Mode                LastWriteTime         Length Name
----                -------------         ------ ----
-a----        5/19/2016  12:32 PM          46776 platyPS-help.xml
```

Create external help file in output path directory.
Note that directory should include language name.
### Example 1 (With -Force and custom encoding)
```
PS C:\> New-ExternalHelp .\docs -OutputPath out\platyPS\en-US -Force -Encoding ([System.Text.Encoding]::Unicode)


    Directory: D:\dev\platyPS\out\platyPS\en-US


Mode                LastWriteTime         Length Name
----                -------------         ------ ----
-a----        5/22/2016   6:34 PM         132942 platyPS-help.xml
```

Create and overwrite existing external help file in output path directory.
Use Unicode Encoding for output file.
## PARAMETERS

### -OutputPath
Path to a folder where you want to put your external help file(s).
The name should end with a locale folder, i.e. ".\out\platyPS\en-US".




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

### -Encoding
Character encoding for your external help file.

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

### -Path
Path to markdown files or directories.




```yaml
Type: String[]
Parameter Sets: (All)
Aliases:

Required: True
Position: Named
Default value:
Accept pipeline input: True (ByPropertyName, ByValue)
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

[PowerShell V2 External MAML Help](https://blogs.msdn.microsoft.com/powershell/2008/12/24/powershell-v2-external-maml-help/)

[Online Version:](https://github.com/PowerShell/platyPS/blob/master/docs/New-ExternalHelp.md)
