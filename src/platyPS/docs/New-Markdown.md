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
New-Markdown -Module <Object> -OutputFolder <String> [-Encoding <String>] [<CommonParameters>]
```

### FromCommand
```
New-Markdown -Command <Object> [-Metadata <Hashtable>] [-OnlineVersionUrl <String>] -OutputFolder <String>
 [-Encoding <String>] [<CommonParameters>]
```

## DESCRIPTION
An easy way to start with platyPS: Generate help stub from:

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

Module should be loaded in the PS Session.

### ----------------------------- Example 3 (from maml file path) ------------------------------------
```
New-Markdown -maml 'C:\Program Files\WindowsPowerShell\Modules\PSReadline\1.1\en-US\Microsoft.PowerShell.PSReadline.dll-help.xml'
```

Create markdown help for inbox PSReadLine module.

### ----------------------------- Example 4 (from maml file content) ------------------------------------
```
New-Markdown -maml (cat -raw 'C:\Program Files\WindowsPowerShell\Modules\PSReadline\1.1\en-US\Microsoft.PowerShell.PSReadline.dll-help.xml')
```

Create markdown help for inbox PSReadLine module.

### ----------------------------- Example 4 (maml, OneFilePerCommand) ------------------------------------
```
New-Markdown -maml 'C:\Program Files\WindowsPowerShell\Modules\PSReadline\1.1\en-US\Microsoft.PowerShell.PSReadline.dll-help.xml' -OneFilePerCommand -OutputFolder PSReadLineHelp
```

Create markdown help for inbox PSReadLine module and output them into a PSReadLineHelp folder. Create one .md file per cmdlet.

## PARAMETERS

### Module
Name of the module for bulk help generation.

```yaml
Type: Object
Parameter Sets: FromModule
Aliases: 

Required: True
Position: named
Default value: 
Accept pipeline input: True (ByValue)
Accept wildcard characters: False
```

### OutputFolder
Path to a folder to output markdown files. This parameter requires -OneFilePerCommand.

```yaml
Type: String
Parameter Sets: FromModule, FromCommand
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
Parameter Sets: FromModule, FromCommand
Aliases: 

Required: False
Position: named
Default value: 
Accept pipeline input: False
Accept wildcard characters: False
```

### Command
Name of a command from your PowerShell session.

```yaml
Type: Object
Parameter Sets: FromCommand
Aliases: 

Required: True
Position: named
Default value: 
Accept pipeline input: True (ByValue)
Accept wildcard characters: False
```

### Metadata
```yaml
Type: Hashtable
Parameter Sets: FromCommand
Aliases: 

Required: False
Position: named
Default value: 
Accept pipeline input: False
Accept wildcard characters: False
```

### OnlineVersionUrl
```yaml
Type: String
Parameter Sets: FromCommand
Aliases: 

Required: False
Position: named
Default value: 
Accept pipeline input: False
Accept wildcard characters: False
```

## INPUTS

### Object

## OUTPUTS

### System.IO.FileInfo[]

## NOTES

## RELATED LINKS

[Online Version:]()


