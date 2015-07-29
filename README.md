[![Build status](https://ci.appveyor.com/api/projects/status/u65tnar0cfkmqywl/branch/master?svg=true)](https://ci.appveyor.com/project/PowerShell/markdown-maml/branch/master)

# Markdown.MAML
Prototype to transform Markdown to MAML and vice verse

# Contributing

It's a hackathon and we have wild free for all party on this repo!
Feel free to do direct checking in master, unless you want a codereview or some other specific goal.
Keep updating **Hackathon status** section to reflect your porgress and communicate it to others.
Include short demo / snippets in the source code and readme to quicky onboard others.

# Repo structure

 -  **src, Markdown.MAML.sln**  - source code for C# markdown to MAML converter.
 -  **MamlToMarkdown.psm1** - powershell MAML to markdown converter
 -  **Examples** - misc files, showcases, etc.
 -  **EndToEndTests** - pester tests that do the full converting MAML -> markdown -> MAML.

# Hackathon status

This section would contain the current status. Please add feature descriptions here.

## Markdown schema

The tag mapping document is here:

https://github.com/PowerShell/Markdown.MAML/blob/master/TagsMapping.md

## Transform maml to markdown

```powershell
Import-Module .\Transform.psm1
# get maml as a string
$maml = cat Examples\SMA.help.xml -Raw
# run convertion
Convert-MamlToMarkdown -maml $maml
```


## Transorm markdown to maml

```powershell
[Markdown.MAML.Renderer.MamlRenderer]::MarkdownStringToMamlString
```