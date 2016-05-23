[![Build status](https://ci.appveyor.com/api/projects/status/u65tnar0cfkmqywl/branch/master?svg=true)](https://ci.appveyor.com/project/PowerShell/markdown-maml/branch/master)

[![Join the chat at https://gitter.im/PowerShell/platyPS](https://badges.gitter.im/PowerShell/platyPS.svg)](https://gitter.im/PowerShell/platyPS?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge&utm_content=badge)

# PlatyPS

PlatyPS provides a way to write PowerShell External Help in Markdown.
And generate markdown help for your existing modules.

Markdown help docs can be generated from old external help files (also known as MAML-xml help), the command objects (reflection), or both.  

PlatyPS can also generate cab file for updatable help.
Updatable help is a PowerShell feature, that you use, when you run `Update-Help`.

## Problem

Traditionally PowerShell external help files have been authored by hand or using complex tool chains and rendered as MAML XML for use as console help.
MAML is cumbersome to edit by hand, and common tools and editors don't support it for complex scenarios like they do with Markdown. PlatyPS is provided as a solution for allow documenting PowerShell help in any editor or tool that supports Markdown.

An additional challange PlatyPS tackles, is to handle PowerShell documentation for complex scenarios (e.g. very large, closed source, and/or C#/binary modules) where it may be desirable to have documentation abstracted away from the codebase. PlatyPS does not need source access to generate documentation.

## Solution

Markdown is designed to be human-readable, without rendering. This makes writing and editing easy and efficient. 
Many editors support it ([Visual Studio Code](https://code.visualstudio.com/), [Sublime Text](http://www.sublimetext.com/), etc), and many tools and collaboration platforms (GitHub, Visual Studio Online) render the Markdown nicely.


## Quick start

* Install platyPS module from the [PowerShell Gallery](https://powershellgallery.com):

```powershell
Install-Module -Name platyPS -Scope CurrentUser
Import-Module platyPS
```

* Create initial Markdown help for `MyAwesomeModule` module:

```powershell
New-MarkdownHelp -Module MyAwesomeModule -OutputFolder .\docs
```

* Edit markdown files in `.\docs` folder and populate `{{ ... }}` placeholder with missed help content.

* Create external help from markdown help

```powershell
New-ExternalHelp .\docs -OutputPath en-US\
```

* Congratulations, now you can keep your help files in markdown!

### platyPS markdown schema

Unfortunately, you cannot just write any Markdown, as platyPS expects Markdown to be authored in a **particular way**.
We have defined a [**schema**](platyPS.schema.md) to determine how parameters are described, where scripts examples are shown and, so on through out the structure of cmdlet documentation.

If you break the schema in your markdown, you will get error message from `New-ExternalHelp` and would not be able to generate extrenal help.
It may be fine for some scenarios, i.e. you want to have online-only version of your help.

The schema closely resembles the existing output format of the `Get-Help` cmdlet in PowerShell. 

## [Usage](.\docs)

Supported scenarios:

*  Create Markdown
    *  Using existing external help files (MAML schema, XML).
    *  Using reflection
    *  Using reflection and existing internal external help files.
    *  For a single cmdlet
    *  For an entire module

*  Update existing Markdown through reflection.

*  Create a module page <ModuleName>.md with summary. It will also allow you to create updatable help cab.

*  Retrieve markdown metadata from markdown file.

*  Create external help xml files (MAML) from platyPS Markdown.

*  Create external help file cab

*  Preview help from generated maml file.

*  Update the [schema](platyPS.schema.md) of existing PlatyPS generated markdown files (Currently version 1.0.0 to version 2.0.0)

## Build

For information about building from sources and contributing see [contrubting guidelinces](CONTRIBUTING.md).
