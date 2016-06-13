---
external help file: platyPS-help.xml
schema: 2.0.0
online version: https://github.com/PowerShell/platyPS/blob/master/docs/New-ExternalHelpCab.md
---

# New-ExternalHelpCab
## SYNOPSIS
Generates a cabinet file, compressing the provided files.
## SYNTAX

```
New-ExternalHelpCab -CabFilesFolder <String> -LandingPagePath <String> -OutputFolder <String>
 [<CommonParameters>]
```

## DESCRIPTION
The New-ExternalHelpCab cmdlet generates a cabinet file containing all of the non-recursive content in a provided folder.

It is reccomended to use content only provided as AboutTopics.Txt and the output from [New-ExternalHelp](New-ExternalHelp.MD)

Using Metadata provided in the Module MD file, the out put cab file is correctly named.
This naming aligns it to the pattern required by the PowerShell help engine to use as updatable help.
This metadeta is part of the module file created by [New-Markdown](New-MarkdownHelp.md) with the -WithModulePage switch. 

A helpinfo.xml is also generated, or updated if existing.
This helpinfo.xml provides help verioning and locale details to the PowerShell help engine.
## EXAMPLES

### Example 1
```
PS C:\> New-ExternalHelpCab -CabFilesFolder 'C:\Module\ExternalHelpContent'  -LandingPagePath 'C:\Module\SomeModuleName.md' -OutputPath 'C:\Module\Cab\'
```

Generates the cab file, containing the content folder files and correctlty named for updatable help, and places in the output path directory.
## PARAMETERS

### -CabFilesFolder
The folder containing all of the help content that should be placed into the cab file.


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

### -LandingPagePath
The path and name of the Module Markdown file containing all of the metadata required to name the cab. 
See the top of the [New-MarkdownHelp -WithLandingPage](New-MarkdownHelp.md) output for a list of all required metadata.


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

### -OutputFolder
This is the location of the cab file and helpinfo.xml created by New-ExternalHelpCab


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

### CommonParameters
This cmdlet supports the common parameters: -Debug, -ErrorAction, -ErrorVariable, -InformationAction, -InformationVariable, -OutVariable, -OutBuffer, -PipelineVariable, -Verbose, -WarningAction, and -WarningVariable. For more information, see about_CommonParameters (http://go.microsoft.com/fwlink/?LinkID=113216).
## INPUTS

### None
This cmdlet does not take in input over the pipeline.
## OUTPUTS

### None
This cmdlet does not output to the console. The only output is in the output folder specificed by the -OutputPath parameter.
## NOTES

## RELATED LINKS

