## Get the code

```
git clone https://github.com/PowerShell/platyPS
```

## Understand code layout

There are two parts: 

- .NET library `Markdown.MAML.dll` written in C#. It does heavy lifting, like parsing markdown, transforming it into xml and so on.
You can open `.\Markdown.MAML.sln` in Visual Studio 2015.
- PowerShell scripts in `.\src\platyPS`. They provide user interface.

## First-time setup

Restore nuget packages.
You can do it from visual studio, or from command line

```
.\.nuget\NuGet.exe restore
```

## Build

To build the whole project, use helper `build.ps1` script

```
.\build.ps1
```
As part of build, module generate help for itself.
The result of the build would be in `out\platyPS` folder.

`build.ps1` also imports the module from `out\platyPS` and generates help for the tool itself.

**Note**: if you changed C# code, script will try to overwrite a dll in-use. 
You would need to re-open your powershell session. If you know a better workflow, please suggest it in the issues.

## Tests

There are two part of projects and two test sets.

- C# part with xUnit tests. You can run them with XUnit runner from the visual studio.
- PowerShell part with [Pester](https://github.com/pester/Pester) tests

```
Invoke-Pester
```

**Note**: Pester tests always force-import module from the output location of `.\build.ps1`.

## Schema

If you have ideas or concerns about markdown schema feel free to open a GitHub issue to discuss it.

## Repo structure

 -  **src\platyPS** - sources to create the final PowerShell module.
 -  **src\Markdown.MAML, Markdown.MAML.sln** - source code for C# Markdown to MAML converter.
 -  **Examples** - misc files, showcases, etc.
 -  **[platyPS.schema.md](platyPS.schema.md)** - description of Markdown that platyPS expects.
 -  **[TagsMapping.md](TagsMapping.md)** - MAML schema description and its mapping to Markdown sections.

## Data transformations

Data transformations are the core of platyPS.
User has content in some form and she wants to transform it into another form.
I.e. transform existing module help to markdown and use it in future to generate the external help and static html for online references.

PlatyPS PowerShell module provide APIs in the form of cmdlets for the end-user scenarios.
This scenarios are assembled from the simple transformations. Chart below describes this simple transformations.

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

User creates a platyPS markdown for the first time for the command

```
New-MarkdownHelp -command New-MyCommandHelp
```

Under the hood, following tranformations happens

[MAML XML file] --> [Help Object + Get-Command object] --> [MAML Model] --> [Markdown file]


# Making a release

1. Make sure that `CHANGELOG.md` is up-to-date, move section from `UNRELEASED` to new section `<release name>`.
1. Make sure platyPS help itself (content in .\docs folder) is up to date. 
   `Update-MarkdownHelp -Path .\docs` should result in no changes.
1. From master, tag the release.
1. Push tag to GitHub.
1. Find the corresponding build on AppVeyor
1. Download zip archive with the module from artifacts tab.
1. Unblock the zip archive, copy content to one of the `$env:PSMODULEPATH` locations, so it's available to `Publish-Module`.
1. Use `Publish-Module -RequiredVersion <version> -Verbose -NuGetApiKey $apiKey`
10. Check that https://www.powershellgallery.com/packages/platyPS/ updated.

Congratulations! You just made a release.
