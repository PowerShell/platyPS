# Markdown.MAML
Prototype to transform Markdown to MAML and vice verse

# Contributing

It's a hackathon and we have wild free for all party on this repo!
Feel free to do direct checking in master, unless you want a codereview or some other specific goal.
Keep updating **Hackathon status** section to reflect your porgress and communicate it to others.
Include short demo / snippets in the source code and readme to quicky onboard others.

# Hackathon status

This section would contain the current status. Please add feature descriptions here.

## Currently porposed md schema
Markdown is very loosely typed. 
We need some sort of schema that looks similar to `Get-Help` output (to make it familiar).
You can find Current proposition here:

https://github.com/PowerShell/Markdown.MAML/blob/master/Example.Short.md

The whole `System.Management.Automation` help converted:
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
