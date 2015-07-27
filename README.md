# Markdown.MAML
Prototype to transform Markdown to MAML and vice verse

# Hackathon status

This section would contain the current status. Please add feature descriptions here.

## Transform maml to markdown
```
# clone repo
git clone https://github.com/PowerShell/Markdown.MAML
# Import module
Import-Module .\Transform.psm1
# get maml as a string
$maml = cat .\SMA.help.xml -Raw
# run convertion
Convert-MamlToMarkdown -maml $maml
```
