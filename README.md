# Markdown.MAML
Prototype to transform Markdown to MAML and vice verse

# Hackathon status

This section would contain the current status. Please add feature descriptions here.

## Currently porposed md schema
Markdown is very loosely typed. 
We need some sort of schema that looks similar to `Get-Help` output (to make it familiar).
You can find Current proposition here:

https://github.com/PowerShell/Markdown.MAML/blob/master/SMA.Help.md

## Transform maml to markdown

```powershell
# clone repo
git clone https://github.com/PowerShell/Markdown.MAML
# Import module
Import-Module .\Transform.psm1
# get maml as a string
$maml = cat .\SMA.help.xml -Raw
# run convertion
Convert-MamlToMarkdown -maml $maml
```
