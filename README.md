# Microsoft.PowerShell.PlatyPS

**PlatyPS** is the tool that Microsoft uses to create the PowerShell content you get from `Get-Help`
and build the content published as [PowerShell documentation][03] on Microsoft Learn.

PowerShell help files are stored as [Microsoft Assistance Markup Language][05] (MAML), an XML
format. **PlatyPS** simplifies the authoring process by allowing you to write the help files in
Markdown, then convert to MAML. [Markdown][04] is widely used in the software industry,
supported by many editors including **Visual Studio Code**, and easier to author.

**Microsoft.PowerShell.PlatyPS** includes several improvements:

- Re-write in C# leveraging [markdig][02] for parsing Markdown (the same library used by
  Microsoft Learn to render Markdown)
- Provides a more accurate description of a PowerShell cmdlet and its parameters and includes
  information that was previously unavailable
- Creates an object model of the help file that you can manipulate and supports chaining cmdlets for
  complex operations
- Increased performance - processes 1000s of Markdown files in seconds

## Install Microsoft.PowerShell.PlatyPS

PlatyPS runs on:

- Windows PowerShell 5.1+
- PowerShell 7+ on Windows, Linux, and macOS

Install the module from [PSGallery][06].

```powershell
Install-PSResource -Name Microsoft.PowerShell.PlatyPS
```

## Getting started using PlatyPS

Creating help files with `PlatyPS` is a multi-step process:

1. [**Create new**][09] or [**update existing**][10] Markdown help files
2. [**Edit the Markdown help**][11] to add descriptions and examples
3. [**Test the Markdown help**][12] to ensure the files render correctly
4. [**Convert and publish**][13] the help files

For detailed guidance, see the [PlatyPS documentation on Microsoft Learn][08].

## Layout of this repository

### Branches

- `main`: Contains the source code for the new version of PlatyPS named
  **Microsoft.PowerShell.PlatyPS**. This is the current supported version of PlatyPS. You can find
  this version listed as [Microsoft.PowerShell.PlatyPS][06] in the PowerShell Gallery.
- `v1`: Contains the source code for the legacy version of PlatyPS named **platyPS**. This version
  is no longer actively maintained, but it's available for reference and legacy support. The latest
  version of this code is 0.14.2. You can find this version listed as [platyPS][07] in the
  PowerShell Gallery.

### Folder structure

- `docs` - contains design documentation for the PlatyPS project
- `src` - contains the source code for the module
- `test` - contains the tests files used to validate the code at build time
- `tools` - contains tools used to build the module

## Contributing to the project

If you are interested in contributing to the PlatyPS project, please see the
[contribution guide][01].

### Build the code

Prerequisites for build:

- PS7
- .NET 8 SDK

Prerequisites for test:

- Pester 4.x (Pester 5.x is not supported)

To build the source code, run the following command in the root of the repository:

```powershell
.\build.ps1 -Clean
```

<!-- link references -->
[01]: ./CONTRIBUTING.md
[02]: https://github.com/xoofx/markdig
[03]: https://learn.microsoft.com/powershell/scripting
[04]: https://wikipedia.org/wiki/Markdown
[05]: https://wikipedia.org/wiki/Microsoft_Assistance_Markup_Language
[06]: https://www.powershellgallery.com/packages/Microsoft.PowerShell.PlatyPS
[07]: https://www.powershellgallery.com/packages/platyPS
[08]: https://learn.microsoft.com/powershell/utility-modules/platyps/overview
[09]: https://learn.microsoft.com/powershell/utility-modules/platyps/step-1-create-new-markdown-help
[10]: https://learn.microsoft.com/powershell/utility-modules/platyps/step-1-update-markdown-help
[11]: https://learn.microsoft.com/powershell/utility-modules/platyps/step-2-edit-markdown-help
[12]: https://learn.microsoft.com/powershell/utility-modules/platyps/step-3-test-markdown-help
[13]: https://learn.microsoft.com/powershell/utility-modules/platyps/step-4-convert-publish-help
