---
external help file: platyPS-help.xml
schema: 2.0.0
---

# Update-MarkdownHelpSchema
## SYNOPSIS
Upgrade markdown help from version 1.0.0 to the latest one (2.0.0).

## SYNTAX

```
Update-MarkdownHelpSchema [-Path] <String[]> [-OutputFolder] <String> [[-Encoding] <Encoding>] [-Force]
```

## DESCRIPTION
{{Fill in the Description}}

## EXAMPLES

### Example 1
```
PS C:\> Update-MarkdownHelpSchema -MarkdownFile .\Examples\PSReadLine.dll-help.md -OutputFolder .\PSReadLine
```

Upgrade PSReadLine platyPS markdown from version 1.0.0 to the latest one (2.0.0).

## PARAMETERS

### -Encoding
{{Fill Encoding Description}}

```yaml
Type: Encoding
Parameter Sets: (All)
Aliases: 

Required: False
Position: 2
Default value: 
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

### -OutputFolder
{{Fill OutputFolder Description}}

```yaml
Type: String
Parameter Sets: (All)
Aliases: 

Required: True
Position: 1
Default value: 
Accept pipeline input: False
Accept wildcard characters: False
```

### -Path
{{Fill Path Description}}

```yaml
Type: String[]
Parameter Sets: (All)
Aliases: 

Required: True
Position: 0
Default value: 
Accept pipeline input: True (ByValue)
Accept wildcard characters: False
```

## INPUTS

### System.String[]


## OUTPUTS

### System.IO.FileInfo[]


## NOTES

## RELATED LINKS

[Online Version:]()


