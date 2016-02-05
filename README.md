[![Build status](https://ci.appveyor.com/api/projects/status/u65tnar0cfkmqywl/branch/master?svg=true)](https://ci.appveyor.com/project/PowerShell/markdown-maml/branch/master)

# PlatyPS
Generate PowerShell External Help files (aka maml) from Markdown.

# Contributing

Please include short demo / snippets in README.md to quicky onboard others.

# Repo structure

 -  **src\platyPS** - sources to create the final PowerShell module.
 -  **src\Markdown.MAML, Markdown.MAML.sln**  - source code for C# markdown to MAML converter.
 -  **Examples** - misc files, showcases, etc.
 -  **test\EndToEndTests** - pester tests that do the full converting MAML -> markdown -> MAML.
 -  **[platyPS.schema.md](platyPS.schema.md)** - description of markdown that platyPS expects.
 -  **[TagsMapping.md](TagsMapping.md)** - maml schema description.
 

## [Usage](src\platyPS\platyPS.md)

*  Create markdown from existing external help files.

*  Create markdown using reflection.

*  Create external help files from platyPS markdown.

