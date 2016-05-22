---
external help file: platyPS-help.xml
schema: 2.0.0
---

# New-ExternalHelp
## SYNOPSIS
Create External help xml file from platyPS markdown, which can be interpreted by the PowerShell help engine.

## SYNTAX

```
New-ExternalHelp -Path <String[]> -OutputPath <String> [-Encoding <Encoding>] [-Force] [<CommonParameters>]
```

## DESCRIPTION
Create External help file from platyPS markdown.

You will get error messages if the markdown files do not follow the schema described in platyPS.schema.md

## EXAMPLES

### Example 1 (MarkdownFile)
```
PS C:\> New-ExternalHelp -MarkdownFile (ls .\docs) -OutputPath out\platyPS\en-US

    Directory: D:\dev\platyPS\out\platyPS\en-US


Mode                LastWriteTime         Length Name
----                -------------         ------ ----
-a----        5/19/2016  12:32 PM          46776 platyPS-help.xml
```

Create external help file in output path directory (directory includes language name).
Uses set of markdown files as input.

### Example 2 (MarkdownFolder)
```
PS C:\> New-ExternalHelp -MarkdownFolder .\docs -OutputPath out\platyPS\en-US
```

Create external help file in output path directory (directory includes language name).
Uses folder containing markdowns as input.

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
Encoding to be used by the output external help file.



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
{{Fill Force Description}}

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
File objects with markdown content in them.

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

### FileInfo[]
You can pipe FileInfo[] objects into this cmdlet.

## OUTPUTS

### System.IO.FileInfo[]
This cmdlet returns a FileInfo[] object.

## NOTES

## RELATED LINKS

[PowerShell V2 External MAML Help](https://blogs.msdn.microsoft.com/powershell/2008/12/24/powershell-v2-external-maml-help/)

[Online Version:]()





