[![Build status](https://ci.appveyor.com/api/projects/status/u65tnar0cfkmqywl/branch/master?svg=true)](https://ci.appveyor.com/project/PowerShell/markdown-maml/branch/master)

# PlatyPS
Generates PowerShell External Help XML files (aka MAML) from Markdown.


# Repo structure

 -  **src\platyPS** - sources to create the final PowerShell module.
 -  **src\Markdown.MAML, Markdown.MAML.sln** - source code for C# Markdown to MAML converter.
 -  **Examples** - misc files, showcases, etc.
 -  **test\EndToEndTests** - Pester tests that do the full converting MAML -> Markdown -> MAML.
 -  **[platyPS.schema.md](platyPS.schema.md)** - description of Markdown that platyPS expects.
 -  **[TagsMapping.md](TagsMapping.md)** - MAML schema description and its mapping to Markdown sections.
 

## [Usage](src/platyPS/platyPS.md)

*  Create Markdown from existing external help files (MAML schema, XML).

*  Create Markdown using reflection.

*  Create external help files (MAML) from platyPS Markdown.

*  Preview help from generated maml file.
