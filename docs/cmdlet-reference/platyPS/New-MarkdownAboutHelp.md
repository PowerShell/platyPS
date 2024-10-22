---
external help file: platyPS-help.xml
Module Name: platyPS
online version: https://github.com/PowerShell/platyPS/blob/master/docs/New-MarkdownAboutHelp.md
schema: 2.0.0
---

# New-MarkdownAboutHelp

## SYNOPSIS
Generates a new About Topic MD file from template.

## SYNTAX

```
New-MarkdownAboutHelp [-OutputFolder] <String> [[-AboutName] <String>] [<CommonParameters>]
```

## DESCRIPTION
The **New-MarkdownAboutHelp** cmdlet generates a Markdown file that is prepopulated with the standard elements of an About Topic.
The cmdlet copies the template MD, renames headers and file name according to the **AboutName** parameter,
and deposits the file in the directory designated by the **OutputFoler** parameter.

The About Topic can be converted to Txt format.
About topics must be in txt format or the PowerShell Help engine will not be able to parse the document.
Use the [New-ExternalHelp](New-ExternalHelp.md) cmdlet to convert About Topic markdown files into About Topic txt files.

## EXAMPLES

### Example 1
```
PS C:\> New-MarkdownAboutHelp -OutputFolder C:\Test -AboutName
PS C:\> Get-ChildItem C:\Test

    Directory: C:\Test


Mode                LastWriteTime         Length Name
----                -------------         ------ ----
-a----        7/13/2016   2:12 PM           1491 TestAboutTopic.md
```

Create and display file info for PowerShell About Topic Markdown File.

### Example 2
```
PS C:\> New-ExternalHelp -Path C:\Test\ -OutputPath C:\Test


    Directory: C:\Test


Mode                LastWriteTime         Length Name
----                -------------         ------ ----
-a----        7/13/2016   2:15 PM           1550 TestAboutTopic.txt
```

Create PowerShell About Topic Txt file from existing Markdown About file.

## PARAMETERS

### -AboutName
The name of the about topic.

```yaml
Type: String
Parameter Sets: (All)
Aliases:

Required: False
Position: 1
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -OutputFolder
The directory to create the about topic in.

```yaml
Type: String
Parameter Sets: (All)
Aliases:

Required: True
Position: 0
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### CommonParameters
This cmdlet supports the common parameters: -Debug, -ErrorAction, -ErrorVariable, -InformationAction, -InformationVariable, -OutVariable, -OutBuffer, -PipelineVariable, -Verbose, -WarningAction, and -WarningVariable. For more information, see [about_CommonParameters](http://go.microsoft.com/fwlink/?LinkID=113216).

## INPUTS

### None

## OUTPUTS

### System.Object
This cmdlet returns a object for created files.

## NOTES
The about topics will need to be added to a cab file to leverage updatable help.

## RELATED LINKS

[New-ExternalHelp](New-ExternalHelp.md)

[New-ExternalHelpCab](New-ExternalHelpCab.md)
