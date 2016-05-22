[![Build status](https://ci.appveyor.com/api/projects/status/u65tnar0cfkmqywl/branch/master?svg=true)](https://ci.appveyor.com/project/PowerShell/markdown-maml/branch/master)

[![Join the chat at https://gitter.im/PowerShell/platyPS](https://badges.gitter.im/PowerShell/platyPS.svg)](https://gitter.im/PowerShell/platyPS?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge&utm_content=badge)

# PlatyPS

Generate Markdown based help documentation for PowerShell Cmdlets. These help docs can be generated from old external help files (AKA: MAML styled Help.xml), the cmdlet object, or both.  

PlatyPS can also be used to generate external Help XML files from Markdown, and the cab file that can be used as part of updatable help, the Update-Help feature of PowerShell.

## How to use it?

* You can install platyPS as module from the [PowerShell Gallery](https://powershellgallery.com):

```powershell
Install-Module -Name platyPS -Scope CurrentUser
```

* Create an initial Markdown:

```powershell
New-MarkdownHelp -Command Get-MyCommandName -OutputFolder .\docs
```

* Create external help from markdown

```powershell
$maml = New-ExternalHelp -MarkdownFolder .\docs
Set-Content -Path en-US\MyModule.psm1-Help.xml -Value $maml -Encoding UTF8
```

## Problem

Traditionally PowerShell external help files have been authored by hand or using complex tool chains and rendered as MAML XML for use as console help.
MAML is cumbersome to edit by hand, and common tools and editors don't support it for complex scenarios like they do with Markdown. PlatyPS is provided as a solution for allow documenting PowerShell help in any editor or tool that supports Markdown.

An additional challange PlatyPS tackles, is to handle PowerShell documentation for complex scenarios (e.g. very large, closed source, and/or C#/binary modules) where it may be desirable to have documentation abstracted away from the codebase. PlatyPS does not need source access to generate documentation.

## Solution

Markdown is designed to be human-readable, without rendering. This makes writing and editing easy and efficient. 
Many editors support it ([Visual Studio Code](https://code.visualstudio.com/), [Sublime Text](http://www.sublimetext.com/), etc), and many tools and collaboration platforms (GitHub, Visual Studio Online) render the Markdown nicely.

### platyPS markdown schema

Unfortunately, you cannot just write any Markdown, as platyPS expects Markdown to be authored in a **particular way**.
We have defined a [**schema**](platyPS.schema.md) to determine how parameters are described, where scripts examples are shown and, so on through out the structure of cmdlet documentation.

**ANY AUTHORING** cannot not break this formatting, or the MAML generated will not be usable by console help.
The schema closely resembles the existing output format of the `Get-Help` cmdlet in PowerShell. 
```PowerShell
#View the output format of get-help with this
Get-Help Get-Command -Full
```

## [Usage](src/platyPS/docs)

Supported scenarios:

*  Create Markdown
    *  Using existing external help files (MAML schema, XML).
    *  Using reflection
    *  Using reflection and existing internal external help files.
    *  For a single cmdlet
    *  For an entire module

*  Update existing Markdown with changes made to the structure of the cmdlet through reflection

*  Create a module landing page <ModuleName>.md 

*  Allow Metadata front matter in the top of the Markdown files (cmdlet and module landing)

*  Allow Metadata to be populated to support cabbing, which requires: 
    *  Module Name
    *  GUID, Locale
    *  Help Version
    *  Cmdlet Source File name
    *  PlayPS Schema Version
    *  Download Link (FwLink)

*  Return the metadata stored in markdown file

*  Create external help.xml files (MAML) from platyPS Markdown.

*  Create external help file cab

*  Preview help from generated maml file.

*  Update the schema of existing PlatyPS generated markdown files. Currently from version 1.0.0 to version 2.0.0 

## Build

For information about building from sources and contributing see [contrubting guidelinces](CONTRIBUTING.md).
