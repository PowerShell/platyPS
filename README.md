[![Build status](https://ci.appveyor.com/api/projects/status/u65tnar0cfkmqywl/branch/master?svg=true)](https://ci.appveyor.com/project/PowerShell/markdown-maml/branch/master)

# Markdown.MAML
Prototype to transform Markdown to MAML and vice verse

# Contributing

Please include short demo / snippets in README.md to quicky onboard others.

# Repo structure

 -  **src, Markdown.MAML.sln**  - source code for C# markdown to MAML converter.
 -  **XToMarkdown** - tools to generate Markdown from something (i.e. MAML, Get-Help object)
 -  **XToMarkdown\MamlToMarkdown.psm1** - powershell MAML to markdown converter
 -  **Examples** - misc files, showcases, etc.
 -  **EndToEndTests** - pester tests that do the full converting MAML -> markdown -> MAML.
 -  **TagsMapping.md** - maml schema description.

## Markdown schema

The best way to figure out up-to-date schema is grab an `*.md` files from build server artifacts.
We need to capture a formal schema, when everything is settled.

## Transform maml to markdown

```powershell
Import-Module XToMarkdown\MamlToMarkdown.psm1
# get maml as a string
$maml = cat Examples\SMA.help.xml -Raw
# run convertion
Convert-MamlToMarkdown -maml $maml
```


## Transorm markdown to maml

```powershell
[Markdown.MAML.Renderer.MamlRenderer]::MarkdownStringToMamlString
```