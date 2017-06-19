---
external help file: platyPS-help.xml
online version: 
schema: 2.0.0
---

# Merge-MarkdownHelp

## SYNOPSIS
Merge multiply markdown version of the same cmdlets into a single markdown file.

## SYNTAX

```
Merge-MarkdownHelp [-Path] <String[]> [-OutputPath] <String> [-Encoding <Encoding>] [-ExplicitApplicableIfAll]
 [-Force] [[-MergeMarker] <String>] [<CommonParameters>]
```

## DESCRIPTION
If we have similar modules, or different version of the same module,
we are likely to have a lot of duplicated markdown in them.

Merge-MarkdownHelp allows you to keep all of them into a single markdown files.
It uses `applicable:` yaml metadata field to identify what versions or tags are applicable.
It acts on two levels: for the whole cmlets and for individual parameters.

Besides the inserted `applicable:` tags, the result markdown will have all the content from 
the individual versions.
Duplicated content would be simple ignored, and different content would be merged using **merge markers**,
followed by a comma-separated list of applicable tags.

## EXAMPLES

### Example 1
```
PS C:\> Merge-MarkdownHelp -Path @('Lync server 2010\Test-CsPhoneBootstrap.md', 'Lync server 2013
\Test-CsPhoneBootstrap.md') -OutputPath lync
```

The result file will be located at lync\Test-CsPhoneBootstrap.md

### Example 1
```
PS C:\> Merge-MarkdownHelp -Path @('Lync server 2010\Test-CsPhoneBootstrap.md', 'Lync server 2013
\Test-CsPhoneBootstrap.md') -OutputPath lync
```

The result file will be located at lync\Test-CsPhoneBootstrap.md

## PARAMETERS

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

### -ExplicitApplicableIfAll
Always write out full list of applicable tags.
By default cmdlets and parameters that are present in all variations don't get an application tag.

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

### -MergeMarker
String to be used as a merge text indicator.
Applicable tag list would be included after the marker

```yaml
Type: String
Parameter Sets: (All)
Aliases: 

Required: False
Position: 3
Default value: '!!! '
Accept pipeline input: False
Accept wildcard characters: False
```

### -OutputPath
Specifies the path of the folder where this cmdlet creates the combined markdown help files.

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
Specifies an array of paths of markdown files or folders.
This cmdlet creates combined markdown help based on these files and folders.

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

### CommonParameters
This cmdlet supports the common parameters: -Debug, -ErrorAction, -ErrorVariable, -InformationAction, -InformationVariable, -OutVariable, -OutBuffer, -PipelineVariable, -Verbose, -WarningAction, and -WarningVariable. For more information, see about_CommonParameters (http://go.microsoft.com/fwlink/?LinkID=113216).

## INPUTS

### System.String[]

## OUTPUTS

### System.IO.FileInfo[]

## NOTES

## RELATED LINKS

