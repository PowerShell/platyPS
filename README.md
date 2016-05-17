[![Build status](https://ci.appveyor.com/api/projects/status/u65tnar0cfkmqywl/branch/master?svg=true)](https://ci.appveyor.com/project/PowerShell/markdown-maml/branch/master)

[![Join the chat at https://gitter.im/PowerShell/platyPS](https://badges.gitter.im/PowerShell/platyPS.svg)](https://gitter.im/PowerShell/platyPS?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge&utm_content=badge)

# PlatyPS

Generate PowerShell External Help XML files (aka MAML) from Markdown.

## Problem

Traditionally PowerShell external help files have been authored as MAML XML.
MAML is cumbersome to edit by hand, and traditional tools and editors don't support it for complex scenarios like they do with Markdown. 
And while comment-based help is nice for writing scripts and functions, especially where the source is available, 
for more complex scenarios (e.g. very large and/or C#/binary modules), it can be desirable to have documentation abstracted away from the codebase.

## Solution

Markdown was designed to be human-readable without rendering which makes it easy to write and edit. 
Many editors support it ([Visual Studio Code](https://code.visualstudio.com/), [Sublime Text](http://www.sublimetext.com/), and others), and many tools and collaboration platforms (GitHub, Visual Studio Online) render the Markdown nicely.

### platyPS markdown schema

Unfortunately, you cannot just write any Markdown, as platyPS expects Markdown to be authored in a **particular way**.
We have defined a [**schema**](platyPS.schema.md) to determine how parameters are described, where scripts examples are shown and, so on.

Any authoring must not break this formatting or the MAML will not be generated correctly.
The schema closely resembles the existing output format of `Get-Help`.

## How to use it?

* You can install platyPS as module from the [PowerShell Gallery](https://powershellgallery.com):

```powershell
Install-Module -Name platyPS -Scope CurrentUser
```

* Create an initial Markdown:

```powershell
New-Markdown -Command Get-MyCommandName -OutputFolder .\docs
```

* Create external help from markdown

```powershell
$maml = New-ExternalHelp -MarkdownFolder .\docs
Set-Content -Path en-US\MyModule.psm1-Help.xml -Value $maml -Encoding UTF8
```

## [Usage](src/platyPS/docs)

Supported scenarios:

*  Create Markdown from existing external help files (MAML schema, XML).

*  Create Markdown using reflection.

*  Create external help files (MAML) from platyPS Markdown.

*  Preview help from generated maml file.

## Build

For information about building from sources and contributing see [contrubting guidelinces](CONTRIBUTING.md).
