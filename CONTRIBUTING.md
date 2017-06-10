## Get the code

```
git clone https://github.com/PowerShell/platyPS
```

## Understand code layout

There are two parts: 

- `Markdown.MAML.dll`, a .NET library written in C#.
  It does the heavy lifting, like parsing Markdown, transforming it into XML, and so on.
  You can open `.\Markdown.MAML.sln` in Visual Studio 2015.
- A PowerShell module in `.\src\platyPS`.
  This module provides the user CLI.

## First-time setup

Restore NuGet packages.
You can do this from Visual Studio, or from the command line.

```
.\.nuget\NuGet.exe restore
```

## Build

To build the whole project, use the `build.ps1` helper script.

```
.\build.ps1
```
As part of the build, platyPS generates help for itself.
The output of the build is placed in `out\platyPS`.

`build.ps1` also imports the module from `out\platyPS` and generates help itself.

**Note**: if you changed C# code, `build.ps1` will try to overwrite a DLL in use.
You will then need to re-open your PowerShell session.
If you know a better workflow, please suggest it in the Issues.

## Tests

Each part of the project has a test set:

- The C# part has xUnit tests.
  You can run them with xUnit from Visual Studio.
- The PowerShell part has [Pester](https://github.com/pester/Pester) tests.
  You can run them using:
  ```
  Invoke-Pester
  ```

**Note**: Pester tests always force-import the module from the output location of `.\build.ps1`.

## Schema

If you have ideas or concerns about the Markdown schema, feel free to open a GitHub Issue to discuss it.

## Repo structure

 -  **src\platyPS** - sources to create the final PowerShell module.
 -  **src\Markdown.MAML, Markdown.MAML.sln** - source code for C# Markdown to MAML converter.
 -  **[platyPS.schema.md](platyPS.schema.md)** - description of Markdown that platyPS expects.

## Data transformations

Data transformations are the core of platyPS.
A user has content in some form and she wants to transform it into another form.
E.g. transform existing module help (in MAML) to Markdown and use it in the future to generate the external help (MAML) and static HTML for online references.

platyPS provides APIs in the form of cmdlets for end-user scenarios.
These scenarios are assembled from simple transformations.
This chart describes these simple transformations:

```
 +----------+
 |          |
 |   HTML   |
 |          |
 +------^---+
        |
 +------+------------+           +-----------------+
 |  Markdown v1 file |           |  Markdown Model |
 |                   +----------->                 |
 |  Markdown v2 file |           +-+---------------+
 |                   |             |
 +---------------^---+             |
                 |                 |
                 |                 |
                 |                 |
              +--+-----------------v--+
              |      MAML Model       |
              | (= Generic Help model)|
              |                       |
              +--+-------------------^+
                 |                   |
                 |                   |
                 |                   |
+----------------v-----+            ++--------------------------+
|  MAML XML file       |            | Help Object in PowerShell |
| (External help file) +------------> (+ Get-Command object)    |
+----------------------+            +---------------------------+
```

##### Example `New-MarkdownHelp`

A user creates a platyPS Markdown for the first time with `New-MarkdownHelp`: 

```
New-MarkdownHelp -Command New-MyCommandHelp
```

Under the hood, the following tranformations happen:

[MAML XML file] --> [Help Object + Get-Command object] --> [MAML Model] --> [Markdown file]

# Making a new release

1. Make sure that `CHANGELOG.md` is up-to-date, move section from `UNRELEASED` to new section `<release name>`.
1. Make sure platyPS help itself (content in .\docs folder) is up to date. 
   `Update-MarkdownHelp -Path .\docs` should result in no changes.
1. Do not change the version in platyps.psd1. Git tag will update this version for release.
1. From master, tag the release.
1. Push tag to GitHub.
1. Find the corresponding build on AppVeyor.
1. Download ZIP archive with the module from Appveyor's Artifacts tab.
1. Unblock the ZIP archive (`Unblock-File foo.zip`), and copy the ZIP's contents to `$env:PSMODULEPATH` so it's available to `Publish-Module`.
1. Publish the module to the Gallery: `Publish-Module -RequiredVersion <version> -Verbose -NuGetApiKey $apiKey`.
10. Check that https://www.powershellgallery.com/packages/platyPS/ updated.

Congratulations! You just made a release.