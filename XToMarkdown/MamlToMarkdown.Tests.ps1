Set-StrictMode -Version latest
$ErrorActionPreference = 'Stop'

Import-Module $PSScriptRoot\MamlToMarkdown.psm1 -Force

Describe 'MamlToMarkdown.psm1' {
    Context 'Get-ParameterMarkdown' {

      $paramXml = [xml]@'
<?xml version="1.0" encoding="utf-8"?>
<root xmlns:maml="http://schemas.microsoft.com/maml/2004/10" xmlns:command="http://schemas.microsoft.com/maml/dev/command/2004/10" xmlns:dev="http://schemas.microsoft.com/maml/dev/2004/10" xmlns:MSHelp="http://msdn.microsoft.com/mshelp">
<command:parameter 

required="true" 
variableLength="false" 
globbing="false" 
pipelineInput="false" 
position="named" 
aliases="none"

>
<maml:name>Force</maml:name>
<maml:description>
<maml:para>Adds a new member even the object has a custom member with the same name. You cannot use the Force parameter to replace a standard member of a type.</maml:para>
</maml:description>
<command:parameterValue required="false" variableLength="false">SwitchParameter</command:parameterValue>
<dev:type>
<maml:name>SwitchParameter</maml:name>
<maml:uri />
</dev:type>
<dev:defaultValue>none</dev:defaultValue>
</command:parameter>
</root>
'@

      It 'we can generate [Parameter] attribute and [switch]' {
          $paramMarkdown = Get-ParameterMarkdown ($paramXml.root.parameter) | Out-String
          $paramMarkdown | Should Be @'
#### Force [switch]

```powershell
[Parameter(Mandatory = $true)]
```

Adds a new member even the object has a custom member with the same name. You cannot use the Force parameter to replace a standard member of a type.

'@
        }
    }
}