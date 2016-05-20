---
external help file: platyPS-help.xml
schema: 2.0.0
---

# New-Markdown
## SYNOPSIS
Convert your existing external help into markdown or generate it from Help object.

## SYNTAX

### FromModule
```
New-Markdown -Module <Object> [-Metadata <Hashtable>] -OutputFolder <String> [-NoMetadata] [-Encoding <String>]
 [<CommonParameters>]
```

### FromCommand
```
New-Markdown -Command <Object> [-Metadata <Hashtable>] [-OnlineVersionUrl <String>] -OutputFolder <String> [-NoMetadata]
 [-Encoding <String>] [<CommonParameters>]
```

## DESCRIPTION
An easy way to start with platyPS.
This cmdlet generates help stub from:

-  Module.

-  Command.

-  External help xml \(aka maml\).

## EXAMPLES

### ----------------------------- Example 1 (from command) ------------------------------------
```
function foo {param([string]$bar)}
New-Markdown -command foo
```

Create stub markdown on the fly from the function foo.

### ----------------------------- Example 2 (from module) ------------------------------------
```
Import-Module platyPS
New-Markdown -module platyPS
```

The module should be loaded in the PS Session, markdown will be generated from the module.

### ----------------------------- Example 3 (from maml file path) ------------------------------------
```
New-Markdown -maml 'C:\Program Files\WindowsPowerShell\Modules\PSReadline\1.1\en-US\Microsoft.PowerShell.PSReadline.dll-help.xml'
```

Create markdown help for inbox PSReadLine module. 
This will use the help file only to generate the markdown.

### ----------------------------- Example 4 (from maml file content) ------------------------------------
```
New-Markdown -maml (cat -raw 'C:\Program Files\WindowsPowerShell\Modules\PSReadline\1.1\en-US\Microsoft.PowerShell.PSReadline.dll-help.xml')
```

## PARAMETERS

### -Module
Name of the module markdown help is being generated for.

```yaml
Type: Object
Parameter Sets: FromModule
Aliases: 

Required: True
Position: Named
Default value: 
Accept pipeline input: True (ByValue)
Accept wildcard characters: False
```

### -OutputFolder
Path to the directory to output markdown help files.

```yaml
Type: String
Parameter Sets: FromModule, FromCommand
Aliases: 

Required: True
Position: Named
Default value: 
Accept pipeline input: False
Accept wildcard characters: False
```

### -Encoding
The encoding to use in generating the markdown files.

```yaml
Type: String
Parameter Sets: FromModule, FromCommand
Aliases: 

Required: False
Position: Named
Default value: 
Accept pipeline input: False
Accept wildcard characters: False
```

### -Command
Name of a command from your PowerShell session.

```yaml
Type: Object
Parameter Sets: FromCommand
Aliases: 

Required: True
Position: Named
Default value: 
Accept pipeline input: True (ByValue)
Accept wildcard characters: False
```

### -Metadata
```yaml
Type: Hashtable
Parameter Sets: FromModule, FromCommand
Aliases: 

Required: False
Position: Named
Default value: 
Accept pipeline input: False
Accept wildcard characters: False
```

### -OnlineVersionUrl
The URL where help can be downloaded using the updatable help function in PowerShell.

```yaml
Type: String
Parameter Sets: FromCommand
Aliases: 

Required: False
Position: Named
Default value: 
Accept pipeline input: False
Accept wildcard characters: False
```

### -NoMetadata
{{Fill NoMetadata Description}}

```yaml
Type: SwitchParameter
Parameter Sets: FromModule, FromCommand
Aliases: 

Required: False
Position: Named
Default value: 
Accept pipeline input: False
Accept wildcard characters: False
```

### CommonParameters
This cmdlet supports the common parameters: -Debug, -ErrorAction, -ErrorVariable, -InformationAction, -InformationVariable, -OutVariable, -OutBuffer, -PipelineVariable, -Verbose, -WarningAction, and -WarningVariable. For more information, see about_CommonParameters \(http://go.microsoft.com/fwlink/?LinkID=113216\).

## INPUTS
### None
You cannot pipe objects into this cmdlet.

### Object

## OUTPUTS

### System.IO.FileInfo[]
This cmdlet returns a FileInfo[] object.

## NOTES

## RELATED LINKS

[Online Version:]()


