z maml
$getProcessMaml = cat -RAW .\Get-Process.help.maml
$maml = (cat -Raw C:\WINDOWS\system32\WindowsPowerShell\v1.0\en-US\Microsoft.PowerShell.Commands.Management.dll-help.xml)
$xml = [xml]$maml
$getProcessXml = ([xml]$getProcessMaml).command
Import-Module -Force .\Transform.psm1
