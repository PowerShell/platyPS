---
external help file: platyPS-help.xml
schema: 2.0.0
---

# New-MarkdownHelp
## SYNOPSIS
Convert your existing external help into markdown or generate it from Help object.

## SYNTAX

### FromModule
```
New-MarkdownHelp -Module <String> [-Force] [-Metadata <Hashtable>] -OutputFolder <String> [-NoMetadata]
 [-Encoding <Encoding>] [-WithModulePage] [-Locale <String>] [-HelpVersion <String>] [-FwLink <String>]
 [<CommonParameters>]
```

### FromCommand
```
New-MarkdownHelp -Command <String> [-Force] [-Metadata <Hashtable>] [-OnlineVersionUrl <String>] -OutputFolder <String>
 [-NoMetadata] [-Encoding <Encoding>] [<CommonParameters>]
```

### FromMaml
```
New-MarkdownHelp -MamlFile <String> [-Force] [-Metadata <Hashtable>] -OutputFolder <String> [-NoMetadata]
 [-Encoding <Encoding>] [-WithModulePage] [-Locale <String>] [-HelpVersion <String>] [-FwLink <String>]
 [-ModuleName <String>] [-ModuleGuid <String>] [<CommonParameters>]
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
New-MarkdownHelp -command foo
```

Create stub markdown on the fly from the function foo.

### ----------------------------- Example 2 (from module) ------------------------------------
```
Import-Module platyPS
New-MarkdownHelp -module platyPS
```

The module should be loaded in the PS Session, markdown will be generated from the module.

### ----------------------------- Example 3 (from maml file path) ------------------------------------
```
New-MarkdownHelp -maml 'C:\Program Files\WindowsPowerShell\Modules\PSReadline\1.1\en-US\Microsoft.PowerShell.PSReadline.dll-help.xml'
```

Create markdown help for inbox PSReadLine module. 
This will use the help file only to generate the markdown.

### ----------------------------- Example 4 (from maml file content) ------------------------------------
```
New-MarkdownHelp -maml (cat -raw 'C:\Program Files\WindowsPowerShell\Modules\PSReadline\1.1\en-US\Microsoft.PowerShell.PSReadline.dll-help.xml')
```

## PARAMETERS

### -Command
Name of a command from your PowerShell session.



```yaml
Type: String
Parameter Sets: FromCommand
Aliases: 

Required: True
Position: Named
Default value: 
Accept pipeline input: True (ByValue)
Accept wildcard characters: False
```

### -Encoding
The encoding to use in generating the markdown files.



```yaml
Type: Encoding
Parameter Sets: (All)
Aliases: 

Required: False
Position: Named
Default value: UTF8 with no BOM
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

### -FwLink
{{Fill FwLink Description}}

```yaml
Type: String
Parameter Sets: FromModule, FromMaml
Aliases: 

Required: False
Position: Named
Default value: 
Accept pipeline input: False
Accept wildcard characters: False
```

### -HelpVersion
{{Fill HelpVersion Description}}

```yaml
Type: String
Parameter Sets: FromModule, FromMaml
Aliases: 

Required: False
Position: Named
Default value: 
Accept pipeline input: False
Accept wildcard characters: False
```

### -Locale
{{Fill Locale Description}}

```yaml
Type: String
Parameter Sets: FromModule, FromMaml
Aliases: 

Required: False
Position: Named
Default value: 
Accept pipeline input: False
Accept wildcard characters: False
```

### -MamlFile
{{Fill MamlFile Description}}

```yaml
Type: String
Parameter Sets: FromMaml
Aliases: 

Required: True
Position: Named
Default value: 
Accept pipeline input: True (ByValue)
Accept wildcard characters: False
```

### -Metadata
{{ ADD HELP HERE !!!}}

```yaml
Type: Hashtable
Parameter Sets: (All)
Aliases: 

Required: False
Position: Named
Default value: 
Accept pipeline input: False
Accept wildcard characters: False
```

### -Module
Name of the module markdown help is being generated for.

```yaml
Type: String
Parameter Sets: FromModule
Aliases: 

Required: True
Position: Named
Default value: 
Accept pipeline input: True (ByValue)
Accept wildcard characters: False
```

### -ModuleGuid
{{Fill ModuleGuid Description}}

```yaml
Type: String
Parameter Sets: FromMaml
Aliases: 

Required: False
Position: Named
Default value: 
Accept pipeline input: False
Accept wildcard characters: False
```

### -ModuleName
{{Fill ModuleName Description}}

```yaml
Type: String
Parameter Sets: FromMaml
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
Parameter Sets: (All)
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

### -OutputFolder
Path to the directory to output markdown help files.



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

### -WithModulePage
{{Fill WithModulePage Description}}

```yaml
Type: SwitchParameter
Parameter Sets: FromModule, FromMaml
Aliases: 

Required: False
Position: Named
Default value: 
Accept pipeline input: False
Accept wildcard characters: False
```

### CommonParameters
This cmdlet supports the common parameters: -Debug, -ErrorAction, -ErrorVariable, -InformationAction, -InformationVariable, -OutVariable, -OutBuffer, -PipelineVariable, -Verbose, -WarningAction, and -WarningVariable. For more information, see about_CommonParameters (http://go.microsoft.com/fwlink/?LinkID=113216).

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





