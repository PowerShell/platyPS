---
external help file: platyPS-help.xml
schema: 2.0.0
---

# New-ExternalHelp
## SYNOPSIS
Create External help file from platyPS markdown.

## SYNTAX

### FromFolder
```
New-ExternalHelp -MarkdownFolder <String> -OutputPath <String> [-Encoding <String>] [<CommonParameters>]
```

### FromFile
```
New-ExternalHelp -MarkdownFile <FileInfo[]> -OutputPath <String> [-Encoding <String>] [<CommonParameters>]
```

## DESCRIPTION
Create External help file from platyPS markdown.

You will get error messages, if provided input doesn't follow schema described in platyPS.schema.md

## EXAMPLES

### Example 1 (MarkdownFile)
```
PS C:\> New-ExternalHelp -MarkdownFile (ls .\docs) -OutputPath out\platyPS\en-US

    Directory: D:\dev\platyPS\out\platyPS\en-US


Mode                LastWriteTime         Length Name
----                -------------         ------ ----
-a----        5/19/2016  12:32 PM          46776 platyPS-help.xml
```

Create external help file in specified directory (directory includes language name).
Uses set of files as input.

### Example 2 (MarkdownFolder)
```
PS C:\> New-ExternalHelp -MarkdownFolder .\docs -OutputPath out\platyPS\en-US
```

Create external help file in specified directory (directory includes language name).
Uses folder with markdowns as an input.


## PARAMETERS

### -MarkdownFolder
Path to a folder with "*.md" files.

```yaml
Type: String
Parameter Sets: FromFolder
Aliases: 

Required: True
Position: Named
Default value: 
Accept pipeline input: False
Accept wildcard characters: False
```

### -OutputPath
Path to a folder, where you want to put your external help file(s).
The name should ends with locale folder, i.e. ".\out\platyPS\en-US".

```yaml
Type: String
Parameter Sets: FromFolder, FromFile
Aliases: 

Required: True
Position: Named
Default value: 
Accept pipeline input: False
Accept wildcard characters: False
```

### -Encoding
Encoding to be used in the output file.

```yaml
Type: String
Parameter Sets: FromFolder, FromFile
Aliases: 

Required: False
Position: Named
Default value: UTF8
Accept pipeline input: False
Accept wildcard characters: False
```

### -MarkdownFile
File objects with markdown content

```yaml
Type: FileInfo[]
Parameter Sets: FromFile
Aliases: 

Required: True
Position: Named
Default value: 
Accept pipeline input: True (ByValue)
Accept wildcard characters: False
```

### CommonParameters
This cmdlet supports the common parameters: -Debug, -ErrorAction, -ErrorVariable, -InformationAction, -InformationVariable, -OutVariable, -OutBuffer, -PipelineVariable, -Verbose, -WarningAction, and -WarningVariable.
For more information, see about_CommonParameters \(http://go.microsoft.com/fwlink/?LinkID=113216\).

## INPUTS

### FileInfo[]

## OUTPUTS

### System.IO.FileInfo[]

## NOTES

## RELATED LINKS

[PowerShell V2 External MAML Help](https://blogs.msdn.microsoft.com/powershell/2008/12/24/powershell-v2-external-maml-help/)

[Online Version:]()


