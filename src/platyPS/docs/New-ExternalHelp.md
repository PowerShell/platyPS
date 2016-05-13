---
external help file: platyPS.psm1-help.xml
schema: 2.0.0
---

# New-ExternalHelp
## SYNOPSIS
Create External help file from platyPS markdown.

## SYNTAX

### FromFolder
```
New-ExternalHelp -MarkdownFolder <String> [-OutputFolder] <String> [<CommonParameters>]
```

### FromFile
```
New-ExternalHelp [-MarkdownFile] <FileInfo[]> [-OutputFolder] <String> [<CommonParameters>]
```

## DESCRIPTION
Create External help file from platyPS markdown.

You will get error messages, if provided input doesn't follow schema described in platyPS.schema.md

Store output of this command in the module directory in corresponding language folder, i.e en-US.

## EXAMPLES

### ----------------------------- Example 1 (platyPS) ------------------------------------
```
$maml = New-ExternalHelp -markdown (cat -raw .\src\platyPS\platyPS.md)
mkdir out\platyPS\en-US -ErrorAction SilentlyContinue > $null
Set-Content -path out\platyPS\en-US\platyPS.psm1-Help.xml -Value $maml -Encoding UTF8
```



### ----------------------------- Example 2 (skipPreambula) ------------------------------------
```
$markdown = New-Markdown New-Markdown | Out-String
New-ExternalHelp -markdown $markdown -skipPreambula | clip
```

Create $maml entry for one command and copy it to clip-board to copy-paste it into existing maml.

### ----------------------------- Example 3 (MarkdownFolder) ------------------------------------
```
$maml = New-ExternalHelp -MarkdownFolder .\src\platyPS
```

You can break help for the big module into several markdown files and put them into a folder. In this case, you may find -MarkdownFolder parameter more convinient.

## PARAMETERS

### MarkdownFolder
Path to a folder with "*.md" files. Their content would be extracted and used. It may be convinient for big modules. Also breaking markdown to several files speed-up convertion process.

```yaml
Type: String
Parameter Sets: FromFolder
Aliases: 

Required: True
Position: named
Default value: 
Accept pipeline input: False
Accept wildcard characters: False
```

### OutputFolder
```yaml
Type: String
Parameter Sets: FromFolder, FromFile
Aliases: 

Required: True
Position: 
Default value: 
Accept pipeline input: False
Accept wildcard characters: False
```

### MarkdownFile
```yaml
Type: FileInfo[]
Parameter Sets: FromFile
Aliases: 

Required: True
Position: 
Default value: 
Accept pipeline input: True (ByValue)
Accept wildcard characters: False
```

## INPUTS

### FileInfo[]
MarkdownFile

## OUTPUTS

### System.IO.FileInfo[]
## RELATED LINKS

[PowerShell V2 External MAML Help](https://blogs.msdn.microsoft.com/powershell/2008/12/24/powershell-v2-external-maml-help/)


