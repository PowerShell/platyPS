---
external help file: platyPS-help.xml
schema: 2.0.0
---

# Show-HelpPreview
## SYNOPSIS
Preview the output Get-Help would return from an external help file(s).

## SYNTAX

### FileOutput
```
Show-HelpPreview -MamlFilePath <String[]> -TextOutputPath <String> [-Encoding <String>] [<CommonParameters>]
```

### AsObject
```
Show-HelpPreview -MamlFilePath <String[]> [-AsObject] [<CommonParameters>]
```

## DESCRIPTION
You can use PowerShell help engine to display the text help output for external help.
This cmdlet verifies how markdown-generated help will look in Get-Help output.

It can be output in a form of help object or as a text.

## EXAMPLES

### Example 1: TextOutputPath
```
PS C:\> Show-HelpPreview -MamlFilePath .\out\platyPS\en-US\platyPS-help.xml -TextOutputPath .\help.txt

    Directory: D:\dev\platyPS


Mode                LastWriteTime         Length Name
----                -------------         ------ ----
-a----        5/19/2016  12:56 PM          20072 help.txt

```

Outputs help preview from maml xml in help.txt into text form.

### Example 2
```
PS C:\> $help = Show-HelpPreview -MamlFilePath .\out\platyPS\en-US\platyPS-help.xml -AsObject
PS C:\> $help.Name
Get-MarkdownMetadata
New-ExternalHelp
New-ExternalHelpCab
New-Markdown
Show-HelpPreview
Update-Markdown
```

Returns a help object get-help preview from maml xml and assign it to the $help variable.
Gets the names of Cmdlet objects inside help.

## PARAMETERS

### -MamlFilePath
File path to maml-xml files.
You can pass several of them.

```yaml
Type: String[]
Parameter Sets: FileOutput, AsObject
Aliases: 

Required: True
Position: Named
Default value: 
Accept pipeline input: False
Accept wildcard characters: False
```

### -TextOutputPath
File path where to output the preview as a text.

```yaml
Type: String
Parameter Sets: FileOutput
Aliases: 

Required: True
Position: Named
Default value: 
Accept pipeline input: False
Accept wildcard characters: False
```

### -Encoding
Encoding to be used to write a text file with help.

```yaml
Type: String
Parameter Sets: FileOutput
Aliases: 

Required: False
Position: Named
Default value: UTF8 without BOM
Accept pipeline input: False
Accept wildcard characters: False
```

### -AsObject
Return output as a PowerShell help object.

```yaml
Type: SwitchParameter
Parameter Sets: AsObject
Aliases: 

Required: True
Position: Named
Default value: False
Accept pipeline input: False
Accept wildcard characters: False
```

### CommonParameters
This cmdlet supports the common parameters: -Debug, -ErrorAction, -ErrorVariable, -InformationAction, -InformationVariable, -OutVariable, -OutBuffer, -PipelineVariable, -Verbose, -WarningAction, and -WarningVariable.
For more information, see about_CommonParameters \(http://go.microsoft.com/fwlink/?LinkID=113216\).

## INPUTS
### None
You cannot pipe objects into this cmdlet.

## OUTPUTS
### None

## NOTES

## RELATED LINKS

[Online Version:]()


