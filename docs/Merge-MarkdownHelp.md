---
external help file: platyPS-help.xml
Module Name: platyPS
online version:
schema: 2.0.0
---

# Merge-MarkdownHelp

## SYNOPSIS
Merge multiple markdown versions of the same cmdlet into a single markdown file.

## SYNTAX

```
Merge-MarkdownHelp [-Path] <String[]> [-OutputPath] <String> [-Encoding <Encoding>] [-ExplicitApplicableIfAll]
 [-Force] [[-MergeMarker] <String>] [<CommonParameters>]
```

## DESCRIPTION
Similar modules, or different versions of the same module, often contain duplicate content.

Merge-MarkdownHelp merges the multiple markdown files into a single markdown file.
It uses the `applicable:` yaml metadata field to identify what versions or tags are applicable.
It acts on two levels: for the whole cmdlet and for individual parameters.

The resulting markdown contains the `applicable:` tags as well as all of the content of the original markdown files.
Duplicate content is simply ignored.
Content that is unique to each file is merged using **merge markers**, followed by a comma-separated list of applicable tags.
A **merge marker** is a string of text that acts as a marker to describe the content that was merged.
The default **merge marker** text consists of three exclamation points !!! however this can be changed to any relevant text using the **-MergeMarker** flag.

## EXAMPLES

### Example 1
The Test-CsPhoneBootstrap.md cmdlet is included in both Lync Server 2010 and Lync Server 2013.
Much of the content is duplicated and thus we want to have a single file for the cmdlet with unique content merged from each individual file.

```
PS C:\> Merge-MarkdownHelp -Path @('Lync Server 2010\Test-CsPhoneBootstrap.md', 'Lync Server 2013\Test-CsPhoneBootstrap.md') -OutputPath lync
```

The resulting file will be located at lync\Test-CsPhoneBootstrap.md

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
This cmdlet supports the common parameters: -Debug, -ErrorAction, -ErrorVariable, -InformationAction, -InformationVariable, -OutVariable, -OutBuffer, -PipelineVariable, -Verbose, -WarningAction, and -WarningVariable. For more information, see [about_CommonParameters](http://go.microsoft.com/fwlink/?LinkID=113216).

## INPUTS

### System.String[]

## OUTPUTS

### System.IO.FileInfo[]

## NOTES

## RELATED LINKS
