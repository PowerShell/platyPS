# Get-PlatyPSExternalHelp

## SYNOPSIS
Create External help file from platyPS markdown.

## DESCRIPTION

Create External help file from platyPS markdown.

You will get error messages, if provided input doesn't follow schema described in platyPS.schema.md

Store output of this command in the module directory in corresponding language folder, i.e en-US.

## PARAMETERS

### MarkdownFolder [string]

```powershell
[Parameter(
  Mandatory = $true,
  ParameterSetName = 'MarkdownFolder')]
```

Path to a folder with "*.md" files.
Their content would be extracted and used.
It may be convinient for big modules.
Also breaking markdown to several files speed-up convertion process.

### markdown [string[]]

```powershell
[Parameter(
  Mandatory = $true,
  ParameterSetName = 'MarkdownString')]
```

This parameter takes array of string, so you can simply pipe output of [Get-Content]().

### skipPreambula [switch]

Switch to avoid emmiting xml preambula and \<helpitems\> tag.

## OUTPUTS
### System.String

Xml document that should be stored to the module folder in the corresponding language folder.

I.e. en-US\platyPS.psm1-Help.xml

## NOTES


## EXAMPLES

### ----------------------------- Example 1 (platyPS) ------------------------------------
```powershell
$maml = Get-PlatyPSExternalHelp -markdown (cat -raw .\src\platyPS\platyPS.md)
mkdir out\platyPS\en-US -ErrorAction SilentlyContinue > $null
Set-Content -path out\platyPS\en-US\platyPS.psm1-Help.xml -Value $maml -Encoding UTF8
```

### ----------------------------- Example 2 (skipPreambula) ------------------------------------
```powershell
$markdown = Get-PlatyPSMarkdown Get-PlatyPSMarkdown | Out-String
Get-PlatyPSExternalHelp $markdown -skipPreambula | clip
```

Create $maml entry for one command and copy it to clip-board to copy-paste it into existing maml.


### ----------------------------- Example 3 (MarkdownFolder) ------------------------------------
```powershell
$maml = Get-PlatyPSExternalHelp -MarkdownFolder .\src\platyPS
```

You can break help for the big module into several markdown files and put them into a folder.
In this case, you may find -MarkdownFolder parameter more convinient.

## RELATED LINKS
[PowerShell V2 External MAML Help](https://blogs.msdn.microsoft.com/powershell/2008/12/24/powershell-v2-external-maml-help/)


# Get-PlatyPSMarkdown

## SYNOPSIS
Convert your existing external help into markdown or generate it from Help object.

## DESCRIPTION
An easy way to start with platyPS:
Generate help stub from:

-  Module.

-  Command.

-  External help xml (aka maml).

## PARAMETERS

### command [Object]

```powershell
[Parameter(
  Mandatory = $true,
  ValueFromPipeline = $true,
  ParameterSetName = 'Command')]
```

Name of a command from your PowerShell session.


### maml [string]

```powershell
[Parameter(
  Mandatory = $true,
  ValueFromPipeline = $true,
  ParameterSetName = 'Maml')]
```

Path or content of file with existing External Help (aka maml).


### module [Object]

```powershell
[Parameter(
  Mandatory = $true,
  ValueFromPipeline = $true,
  ParameterSetName = 'Module')]
```

Name of the module for bulk help generation.

### OneFilePerCommand [switch]

Use this switch to create a folder with one .md file per cmdlet.
This parameter requires specifing -OutputFolder.

### OutputFolder [string]

Path to a folder to output markdown files.
This parameter requires -OneFilePerCommand.


## INPUTS
### System.Object
### System.String

## OUTPUTS
### System.String[]

## NOTES


## EXAMPLES
### ----------------------------- Example 1 (from command) ------------------------------------

```powershell
function foo {param([string]$bar)}
Get-PlatyPSMarkdown -command foo
```

Create stub markdown on the fly from the function foo.

### ----------------------------- Example 2 (from module) ------------------------------------

```powershell
Import-Module platyPS
Get-PlatyPSMarkdown -module platyPS
```

Module should be loaded in the PS Session.

### ----------------------------- Example 3 (from maml file path) ------------------------------------

```powershell
Get-PlatyPSMarkdown -maml 'C:\Program Files\WindowsPowerShell\Modules\PSReadline\1.1\en-US\Microsoft.PowerShell.PSReadline.dll-help.xml'
```

Create markdown help for inbox PSReadLine module.

### ----------------------------- Example 4 (from maml file content) ------------------------------------

```powershell
Get-PlatyPSMarkdown -maml (cat -raw 'C:\Program Files\WindowsPowerShell\Modules\PSReadline\1.1\en-US\Microsoft.PowerShell.PSReadline.dll-help.xml')
```

Create markdown help for inbox PSReadLine module.

### ----------------------------- Example 4 (maml, OneFilePerCommand) ------------------------------------

```powershell
Get-PlatyPSMarkdown -maml 'C:\Program Files\WindowsPowerShell\Modules\PSReadline\1.1\en-US\Microsoft.PowerShell.PSReadline.dll-help.xml' -OneFilePerCommand -OutputFolder PSReadLineHelp
```

Create markdown help for inbox PSReadLine module and output them into a PSReadLineHelp folder.
Create one .md file per cmdlet.

## RELATED LINKS




# New-PlatyPSModuleFromMaml

## SYNOPSIS
Takes a MAML file and generates a script module with corresponding help at the given location. 

## DESCRIPTION

Command generates dummy module with particular External Help file.
You can use it to see, how generated help will look like.

This cmdlet generates an object which contains the module name, module path and the list of cmdlets for the generated module.

## PARAMETERS

### DestinationPath [string]

```powershell
[Parameter(
  Position = 1)]
```

Path to a folder, where module should be generated.


### MamlFilePath [string]

```powershell
[Parameter(
  Mandatory = $true,
  Position = 0)]
```

Path to the External Help file.


## INPUTS
### None

## OUTPUTS
### System.Object

## NOTES


## EXAMPLES
### ----------------------------- Example 1 (test the module) ------------------------------------
```powershell
$generatedModule = New-PlatyPSModuleFromMaml -MamlFilePath $outMamlFilePath
$generatedModule.Cmdlets | % { Get-Help -Name "$($generatedModule.Name)\$_" -Full | Out-String }
```

Show generated help for the whole module as an output of Help engine system.

## RELATED LINKS

