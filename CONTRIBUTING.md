## Get the code

```
git clone https://github.com/PowerShell/platyPS
```

## Build

You would need Visual Studio and `msbuild` in PATH to build from command line.
Use helper `build.ps1` script

```
.\build.ps1
```
As part of build, module generate help for itself.
The result of the build would be in `out\platyPS` folder.

## Schema

If you have ideas or concerns about markdown schema feel free to open a GitHub issue to discuss it.

## Repo structure

 -  **src\platyPS** - sources to create the final PowerShell module.
 -  **src\Markdown.MAML, Markdown.MAML.sln** - source code for C# Markdown to MAML converter.
 -  **Examples** - misc files, showcases, etc.
 -  **test\EndToEndTests** - Pester tests that do the full converting MAML -> Markdown -> MAML.
 -  **[platyPS.schema.md](platyPS.schema.md)** - description of Markdown that platyPS expects.
 -  **[TagsMapping.md](TagsMapping.md)** - MAML schema description and its mapping to Markdown sections.
