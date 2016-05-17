---
external help file: platyPS-help.xml
schema: 2.0.0
---

# Update-Markdown
## SYNOPSIS

## SYNTAX

### SchemaUpgrade
```
Update-Markdown -MarkdownFile <Object[]> -OutputFolder <String> [-Encoding <String>] [<CommonParameters>]
```

### Reflection
```
Update-Markdown -MarkdownFile <Object[]> [-Encoding <String>] [-UseReflection] [<CommonParameters>]
```

## DESCRIPTION
Update platyPS markdown help files in place.

Two supported scenarios:

- update schema version

- update markdown with information from the 'live' command

## EXAMPLES

### Example 1
```
PS C:\> {{ Add example code here }}
```

{{ Add example description here }}

## PARAMETERS

### MarkdownFile
```yaml
Type: Object[]
Parameter Sets: SchemaUpgrade, Reflection
Aliases: 

Required: True
Position: named
Default value: 
Accept pipeline input: True (ByValue)
Accept wildcard characters: False
```

### OutputFolder
```yaml
Type: String
Parameter Sets: SchemaUpgrade
Aliases: 

Required: True
Position: named
Default value: 
Accept pipeline input: False
Accept wildcard characters: False
```

### Encoding
```yaml
Type: String
Parameter Sets: SchemaUpgrade, Reflection
Aliases: 

Required: False
Position: named
Default value: UTF8 with no BOM
Accept pipeline input: False
Accept wildcard characters: False
```

### UseReflection
```yaml
Type: SwitchParameter
Parameter Sets: Reflection
Aliases: 

Required: True
Position: named
Default value: False
Accept pipeline input: False
Accept wildcard characters: False
```

## INPUTS

## OUTPUTS

### System.IO.FileInfo[]

## NOTES

## RELATED LINKS

[Online Version:]()


