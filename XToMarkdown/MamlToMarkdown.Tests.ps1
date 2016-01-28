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
          $paramMarkdown = Get-ParameterMarkdown -parameter ($paramXml.root.parameter) -paramSets @{
              $paramXml.root.parameter.name = @{
                  '*' = $paramXml.root.parameter
              }
          } | Out-String
          $paramMarkdown | Should Be @'
#### Force [switch]

```powershell
[Parameter(Mandatory = $true)]
```

Adds a new member even the object has a custom member with the same name.
You cannot use the Force parameter to replace a standard member of a type.


'@
        }
    }

    Context 'Get-ParameterSetMapping' {
        $syntaxXml = [xml]@'
<?xml version="1.0" encoding="utf-8"?>
<root xmlns:maml="http://schemas.microsoft.com/maml/2004/10" xmlns:command="http://schemas.microsoft.com/maml/dev/command/2004/10" xmlns:dev="http://schemas.microsoft.com/maml/dev/2004/10" xmlns:MSHelp="http://msdn.microsoft.com/mshelp">
<command:syntax>
      <command:syntaxItem>
        <maml:name>Add-Member</maml:name>
        <command:parameter required="false" variableLength="false" globbing="false" pipelineInput="false" position="named" aliases="ia">
          <maml:name>InformationAction</maml:name>
          <maml:description>
            <maml:para>
            </maml:para>
          </maml:description>
          <command:parameterValueGroup>
            <command:parameterValue required="false" variableLength="false">SilentlyContinue</command:parameterValue>
            <command:parameterValue required="false" variableLength="false">Stop</command:parameterValue>
            <command:parameterValue required="false" variableLength="false">Continue</command:parameterValue>
            <command:parameterValue required="false" variableLength="false">Inquire</command:parameterValue>
            <command:parameterValue required="false" variableLength="false">Ignore</command:parameterValue>
            <command:parameterValue required="false" variableLength="false">Suspend</command:parameterValue>
          </command:parameterValueGroup>
        </command:parameter>
        <command:parameter required="false" variableLength="false" globbing="false" pipelineInput="false" position="named" aliases="iv">
          <maml:name>InformationVariable</maml:name>
          <maml:description>
            <maml:para>
            </maml:para>
          </maml:description>
          <command:parameterValue required="false" variableLength="false">System.String</command:parameterValue>
        </command:parameter>
        <command:parameter required="false" variableLength="false" globbing="false" pipelineInput="false" position="named" aliases="none">
          <maml:name>PassThru</maml:name>
          <maml:description>
            <maml:para>Returns the newly extended object. By default, this cmdlet does not generate any output.</maml:para>
            <maml:para>For most objects, Add-Member adds the new members to the input object. However, when the input object is a string, Add-Member cannot add the member to the input object. For these objects, use the PassThru parameter to create an output object.</maml:para>
            <maml:para>In Windows PowerShell 2.0, Add-Member added members only to the PSObject wrapper of objects, not to the object. Use the PassThru parameter to create an output object for any object that has a PSObject wrapper.</maml:para>
          </maml:description>
        </command:parameter>
        <command:parameter required="true" variableLength="false" globbing="false" pipelineInput="True (ByValue)" position="named" aliases="none">
          <maml:name>InputObject</maml:name>
          <maml:description>
            <maml:para>Specifies the object to which the new member is added. Enter a variable that contains the objects, or type a command or expression that gets the objects.</maml:para>
          </maml:description>
          <command:parameterValue required="true" variableLength="false">PSObject</command:parameterValue>
        </command:parameter>
        <command:parameter required="true" variableLength="false" globbing="false" pipelineInput="false" position="named" aliases="none">
          <maml:name>TypeName</maml:name>
          <maml:description>
            <maml:para>Specifies a name for the type.</maml:para>
            <maml:para>When the type is a class in the System namespace or a type that has a type accelerator, you can enter the short name of the type. Otherwise, the full type name is required.  This parameter is effective only when the input object is a PSObject.</maml:para>
            <maml:para>This parameter is introduced in Windows PowerShell 3.0.</maml:para>
          </maml:description>
          <command:parameterValue required="true" variableLength="false">String</command:parameterValue>
        </command:parameter>
      </command:syntaxItem>
      <command:syntaxItem>
        <maml:name>Add-Member</maml:name>
        <command:parameter required="true" variableLength="false" globbing="false" pipelineInput="false" position="1" aliases="none">
          <maml:name>NotePropertyName</maml:name>
          <maml:description>
            <maml:para>Adds a note property with the specified name.</maml:para>
            <maml:para>Use this parameter with the NotePropertyValue parameter. The parameter name (-NotePropertyName) is optional.</maml:para>
            <maml:para>This parameter is introduced in Windows PowerShell 3.0.</maml:para>
          </maml:description>
          <command:parameterValue required="true" variableLength="false">String</command:parameterValue>
        </command:parameter>
        <command:parameter required="true" variableLength="false" globbing="false" pipelineInput="false" position="2" aliases="none">
          <maml:name>NotePropertyValue</maml:name>
          <maml:description>
            <maml:para>Adds a note property with the specified value.</maml:para>
            <maml:para>Use this parameter with the NotePropertyName parameter. The parameter name (-NotePropertyValue) is optional.</maml:para>
            <maml:para>This parameter is introduced in Windows PowerShell 3.0.</maml:para>
          </maml:description>
          <command:parameterValue required="true" variableLength="false">Object</command:parameterValue>
        </command:parameter>
        <command:parameter required="false" variableLength="false" globbing="false" pipelineInput="false" position="named" aliases="none">
          <maml:name>Force</maml:name>
          <maml:description>
            <maml:para>Adds a new member even the object has a custom member with the same name. You cannot use the Force parameter to replace a standard member of a type.</maml:para>
          </maml:description>
        </command:parameter>
        <command:parameter required="false" variableLength="false" globbing="false" pipelineInput="false" position="named" aliases="ia">
          <maml:name>InformationAction</maml:name>
          <maml:description>
            <maml:para>
            </maml:para>
          </maml:description>
          <command:parameterValueGroup>
            <command:parameterValue required="false" variableLength="false">SilentlyContinue</command:parameterValue>
            <command:parameterValue required="false" variableLength="false">Stop</command:parameterValue>
            <command:parameterValue required="false" variableLength="false">Continue</command:parameterValue>
            <command:parameterValue required="false" variableLength="false">Inquire</command:parameterValue>
            <command:parameterValue required="false" variableLength="false">Ignore</command:parameterValue>
            <command:parameterValue required="false" variableLength="false">Suspend</command:parameterValue>
          </command:parameterValueGroup>
        </command:parameter>
        <command:parameter required="false" variableLength="false" globbing="false" pipelineInput="false" position="named" aliases="iv">
          <maml:name>InformationVariable</maml:name>
          <maml:description>
            <maml:para>
            </maml:para>
          </maml:description>
          <command:parameterValue required="false" variableLength="false">System.String</command:parameterValue>
        </command:parameter>
        <command:parameter required="false" variableLength="false" globbing="false" pipelineInput="false" position="named" aliases="none">
          <maml:name>PassThru</maml:name>
          <maml:description>
            <maml:para>Returns the newly extended object. By default, this cmdlet does not generate any output.</maml:para>
            <maml:para>For most objects, Add-Member adds the new members to the input object. However, when the input object is a string, Add-Member cannot add the member to the input object. For these objects, use the PassThru parameter to create an output object.</maml:para>
            <maml:para>In Windows PowerShell 2.0, Add-Member added members only to the PSObject wrapper of objects, not to the object. Use the PassThru parameter to create an output object for any object that has a PSObject wrapper.</maml:para>
          </maml:description>
        </command:parameter>
        <command:parameter required="false" variableLength="false" globbing="false" pipelineInput="false" position="named" aliases="none">
          <maml:name>TypeName</maml:name>
          <maml:description>
            <maml:para>Specifies a name for the type.</maml:para>
            <maml:para>When the type is a class in the System namespace or a type that has a type accelerator, you can enter the short name of the type. Otherwise, the full type name is required.  This parameter is effective only when the input object is a PSObject.</maml:para>
            <maml:para>This parameter is introduced in Windows PowerShell 3.0.</maml:para>
          </maml:description>
          <command:parameterValue required="true" variableLength="false">String</command:parameterValue>
        </command:parameter>
        <command:parameter required="true" variableLength="false" globbing="false" pipelineInput="True (ByValue)" position="named" aliases="none">
          <maml:name>InputObject</maml:name>
          <maml:description>
            <maml:para>Specifies the object to which the new member is added. Enter a variable that contains the objects, or type a command or expression that gets the objects.</maml:para>
          </maml:description>
          <command:parameterValue required="true" variableLength="false">PSObject</command:parameterValue>
        </command:parameter>
      </command:syntaxItem>
      <command:syntaxItem>
        <maml:name>Add-Member</maml:name>
        <command:parameter required="true" variableLength="false" globbing="false" pipelineInput="false" position="1" aliases="none">
          <maml:name>NotePropertyMembers</maml:name>
          <maml:description>
            <maml:para>Specifies a hash table or ordered dictionary of note property names and values. Type a hash table or dictionary in which the keys are note property names and the values are note property values.</maml:para>
            <maml:para>For more information about hash tables and ordered dictionaries in Windows PowerShell, see about_Hash_Tables (http://go.microsoft.com/fwlink/?LinkID=135175).</maml:para>
            <maml:para>This parameter is introduced in Windows PowerShell 3.0.</maml:para>
          </maml:description>
          <command:parameterValue required="true" variableLength="false">IDictionary</command:parameterValue>
        </command:parameter>
        <command:parameter required="false" variableLength="false" globbing="false" pipelineInput="false" position="named" aliases="none">
          <maml:name>Force</maml:name>
          <maml:description>
            <maml:para>Adds a new member even the object has a custom member with the same name. You cannot use the Force parameter to replace a standard member of a type.</maml:para>
          </maml:description>
        </command:parameter>
        <command:parameter required="false" variableLength="false" globbing="false" pipelineInput="false" position="named" aliases="ia">
          <maml:name>InformationAction</maml:name>
          <maml:description>
            <maml:para>
            </maml:para>
          </maml:description>
          <command:parameterValueGroup>
            <command:parameterValue required="false" variableLength="false">SilentlyContinue</command:parameterValue>
            <command:parameterValue required="false" variableLength="false">Stop</command:parameterValue>
            <command:parameterValue required="false" variableLength="false">Continue</command:parameterValue>
            <command:parameterValue required="false" variableLength="false">Inquire</command:parameterValue>
            <command:parameterValue required="false" variableLength="false">Ignore</command:parameterValue>
            <command:parameterValue required="false" variableLength="false">Suspend</command:parameterValue>
          </command:parameterValueGroup>
        </command:parameter>
        <command:parameter required="false" variableLength="false" globbing="false" pipelineInput="false" position="named" aliases="iv">
          <maml:name>InformationVariable</maml:name>
          <maml:description>
            <maml:para>
            </maml:para>
          </maml:description>
          <command:parameterValue required="false" variableLength="false">System.String</command:parameterValue>
        </command:parameter>
        <command:parameter required="false" variableLength="false" globbing="false" pipelineInput="false" position="named" aliases="none">
          <maml:name>PassThru</maml:name>
          <maml:description>
            <maml:para>Returns the newly extended object. By default, this cmdlet does not generate any output.</maml:para>
            <maml:para>For most objects, Add-Member adds the new members to the input object. However, when the input object is a string, Add-Member cannot add the member to the input object. For these objects, use the PassThru parameter to create an output object.</maml:para>
            <maml:para>In Windows PowerShell 2.0, Add-Member added members only to the PSObject wrapper of objects, not to the object. Use the PassThru parameter to create an output object for any object that has a PSObject wrapper.</maml:para>
          </maml:description>
        </command:parameter>
        <command:parameter required="false" variableLength="false" globbing="false" pipelineInput="false" position="named" aliases="none">
          <maml:name>TypeName</maml:name>
          <maml:description>
            <maml:para>Specifies a name for the type.</maml:para>
            <maml:para>When the type is a class in the System namespace or a type that has a type accelerator, you can enter the short name of the type. Otherwise, the full type name is required.  This parameter is effective only when the input object is a PSObject.</maml:para>
            <maml:para>This parameter is introduced in Windows PowerShell 3.0.</maml:para>
          </maml:description>
          <command:parameterValue required="true" variableLength="false">String</command:parameterValue>
        </command:parameter>
        <command:parameter required="true" variableLength="false" globbing="false" pipelineInput="True (ByValue)" position="named" aliases="none">
          <maml:name>InputObject</maml:name>
          <maml:description>
            <maml:para>Specifies the object to which the new member is added. Enter a variable that contains the objects, or type a command or expression that gets the objects.</maml:para>
          </maml:description>
          <command:parameterValue required="true" variableLength="false">PSObject</command:parameterValue>
        </command:parameter>
      </command:syntaxItem>
      <command:syntaxItem>
        <maml:name>Add-Member</maml:name>
        <command:parameter required="true" variableLength="false" globbing="false" pipelineInput="false" position="1" aliases="Type">
          <maml:name>MemberType</maml:name>
          <maml:description>
            <maml:para>Specifies the type of the member to add.  This parameter is mandatory.</maml:para>
            <maml:para>The valid values for this parameter are: "NoteProperty,AliasProperty,ScriptProperty,CodeProperty,ScriptMethod,CodeMethod" AliasProperty, CodeMethod, CodeProperty, Noteproperty, ScriptMethod, and ScriptProperty.</maml:para>
            <maml:para>For information about these values, see "PSMemberTypes Enumeration" in MSDN at <maml:navigationLink><maml:linkText>http://msdn.microsoft.com/en-us/library/windows/desktop/system.management.automation.psmembertypes(v=vs.85).aspx</maml:linkText><maml:uri></maml:uri></maml:navigationLink>. </maml:para>
            <maml:para>Not all objects have every type of member. If you specify a member type that the object does not have, Windows PowerShell returns an error.</maml:para>
          </maml:description>
          <command:parameterValueGroup>
            <command:parameterValue required="true" variableLength="false">AliasProperty</command:parameterValue>
            <command:parameterValue required="true" variableLength="false">CodeProperty</command:parameterValue>
            <command:parameterValue required="true" variableLength="false">Property</command:parameterValue>
            <command:parameterValue required="true" variableLength="false">NoteProperty</command:parameterValue>
            <command:parameterValue required="true" variableLength="false">ScriptProperty</command:parameterValue>
            <command:parameterValue required="true" variableLength="false">Properties</command:parameterValue>
            <command:parameterValue required="true" variableLength="false">PropertySet</command:parameterValue>
            <command:parameterValue required="true" variableLength="false">Method</command:parameterValue>
            <command:parameterValue required="true" variableLength="false">CodeMethod</command:parameterValue>
            <command:parameterValue required="true" variableLength="false">ScriptMethod</command:parameterValue>
            <command:parameterValue required="true" variableLength="false">Methods</command:parameterValue>
            <command:parameterValue required="true" variableLength="false">ParameterizedProperty</command:parameterValue>
            <command:parameterValue required="true" variableLength="false">MemberSet</command:parameterValue>
            <command:parameterValue required="true" variableLength="false">Event</command:parameterValue>
            <command:parameterValue required="true" variableLength="false">Dynamic</command:parameterValue>
            <command:parameterValue required="true" variableLength="false">All</command:parameterValue>
          </command:parameterValueGroup>
        </command:parameter>
        <command:parameter required="true" variableLength="false" globbing="false" pipelineInput="false" position="2" aliases="none">
          <maml:name>Name</maml:name>
          <maml:description>
            <maml:para>Specifies the name of the member to be added.</maml:para>
          </maml:description>
          <command:parameterValue required="true" variableLength="false">String</command:parameterValue>
        </command:parameter>
        <command:parameter required="false" variableLength="false" globbing="false" pipelineInput="false" position="3" aliases="none">
          <maml:name>Value</maml:name>
          <maml:description>
            <maml:para>Specifies the initial value of the added member. If you add an AliasProperty, CodeProperty, ScriptProperty or CodeMethod member, you can supply optional, additional information by using the SecondValue parameter.</maml:para>
          </maml:description>
          <command:parameterValue required="false" variableLength="false">Object</command:parameterValue>
        </command:parameter>
        <command:parameter required="false" variableLength="false" globbing="false" pipelineInput="false" position="4" aliases="none">
          <maml:name>SecondValue</maml:name>
          <maml:description>
            <maml:para>Specifies optional additional information about AliasProperty, ScriptProperty, CodeProperty, or CodeMethod members. If used when adding an AliasProperty, this parameter must be a data type. A conversion (cast) to the specified data type is added to the value of the AliasProperty. For example, if you add an AliasProperty that provides an alternate name for a string property, you can also specify a SecondValue parameter of System.Int32 to indicate that the value of that string property should be converted to an integer when accessed by using the corresponding AliasProperty.</maml:para>
            <maml:para>You can use the SecondValue parameter to specify an additional ScriptBlock when adding a ScriptProperty member. In that case, the first ScriptBlock, specified in the Value parameter, is used to get the value of a variable. The second ScriptBlock, specified in the SecondValue parameter, is used to set the value of a variable.</maml:para>
          </maml:description>
          <command:parameterValue required="false" variableLength="false">Object</command:parameterValue>
        </command:parameter>
        <command:parameter required="false" variableLength="false" globbing="false" pipelineInput="false" position="named" aliases="none">
          <maml:name>Force</maml:name>
          <maml:description>
            <maml:para>Adds a new member even the object has a custom member with the same name. You cannot use the Force parameter to replace a standard member of a type.</maml:para>
          </maml:description>
        </command:parameter>
        <command:parameter required="false" variableLength="false" globbing="false" pipelineInput="false" position="named" aliases="ia">
          <maml:name>InformationAction</maml:name>
          <maml:description>
            <maml:para>
            </maml:para>
          </maml:description>
          <command:parameterValueGroup>
            <command:parameterValue required="false" variableLength="false">SilentlyContinue</command:parameterValue>
            <command:parameterValue required="false" variableLength="false">Stop</command:parameterValue>
            <command:parameterValue required="false" variableLength="false">Continue</command:parameterValue>
            <command:parameterValue required="false" variableLength="false">Inquire</command:parameterValue>
            <command:parameterValue required="false" variableLength="false">Ignore</command:parameterValue>
            <command:parameterValue required="false" variableLength="false">Suspend</command:parameterValue>
          </command:parameterValueGroup>
        </command:parameter>
        <command:parameter required="false" variableLength="false" globbing="false" pipelineInput="false" position="named" aliases="iv">
          <maml:name>InformationVariable</maml:name>
          <maml:description>
            <maml:para>
            </maml:para>
          </maml:description>
          <command:parameterValue required="false" variableLength="false">System.String</command:parameterValue>
        </command:parameter>
        <command:parameter required="false" variableLength="false" globbing="false" pipelineInput="false" position="named" aliases="none">
          <maml:name>PassThru</maml:name>
          <maml:description>
            <maml:para>Returns the newly extended object. By default, this cmdlet does not generate any output.</maml:para>
            <maml:para>For most objects, Add-Member adds the new members to the input object. However, when the input object is a string, Add-Member cannot add the member to the input object. For these objects, use the PassThru parameter to create an output object.</maml:para>
            <maml:para>In Windows PowerShell 2.0, Add-Member added members only to the PSObject wrapper of objects, not to the object. Use the PassThru parameter to create an output object for any object that has a PSObject wrapper.</maml:para>
          </maml:description>
        </command:parameter>
        <command:parameter required="false" variableLength="false" globbing="false" pipelineInput="false" position="named" aliases="none">
          <maml:name>TypeName</maml:name>
          <maml:description>
            <maml:para>Specifies a name for the type.</maml:para>
            <maml:para>When the type is a class in the System namespace or a type that has a type accelerator, you can enter the short name of the type. Otherwise, the full type name is required.  This parameter is effective only when the input object is a PSObject.</maml:para>
            <maml:para>This parameter is introduced in Windows PowerShell 3.0.</maml:para>
          </maml:description>
          <command:parameterValue required="true" variableLength="false">String</command:parameterValue>
        </command:parameter>
        <command:parameter required="true" variableLength="false" globbing="false" pipelineInput="True (ByValue)" position="named" aliases="none">
          <maml:name>InputObject</maml:name>
          <maml:description>
            <maml:para>Specifies the object to which the new member is added. Enter a variable that contains the objects, or type a command or expression that gets the objects.</maml:para>
          </maml:description>
          <command:parameterValue required="true" variableLength="false">PSObject</command:parameterValue>
        </command:parameter>
      </command:syntaxItem>
    </command:syntax>
</root>
'@
        It 'create the right paramsets mapping' {
            $paramSets = Get-ParameterSetMapping ($syntaxXml.root)
            $paramSets.Count | Should Be 13
            $paramSets['MemberType'].Keys | Should Be 'Set 4'
            $paramSets['InputObject'].Keys | Should Be '*'
            $paramSets['NotePropertyName'].Keys | Should Be 'Set 2'
            $paramSets['Force'].Keys.Count | Should Be 3
        }
    }
}