## Get the code

```
git clone https://github.com/PowerShell/platyPS
```

## First-time setup

Restore nuget packages.
You can do it from visual studio, or from command line

```
.\.nuget\NuGet.exe restore
```

## Build

You would need Visual Studio and `msbuild` in PATH to build from command line.
Use helper `build.ps1` script

```
.\build.ps1
```
As part of build, module generate help for itself.
The result of the build would be in `out\platyPS` folder.

## Tests

There are two part of projects and two test sets.

- C# part with xUnit tests
- PowerShell part with [Pester](https://github.com/pester/Pester) tests

```
Invoke-Pester
```

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

##### Example `New-Markdown`

User creates a platyPS markdown for the first time for the command

```
New-MarkdownHelp -command New-MyCommand
```

Under the hood, following tranformations happens

[MAML XML file] --> [Help Object + Get-Command object] --> [MAML Model] --> [Markdown file]
