#Tag Mapping: MAML to MarkDown
Outlines the mapping of MAML required elements to their MarkDown counterparts.

#Elements

Note about Para Elements
The para elements can have multiple entries. Comand.Details.Description is the container for them all.

##Module Name
The name of the help.xml file corresponds to the name of the cmdlet providing module of assembly. In version (?) 4 of PowerShell the help.xml can be named after the module instead of the assembly that grants cmdlets.

##Cmdlet Name
Name = Command.Details.Name
This also needs to be added to the SyntaxItem.Name node, in the paramters section below.

```
<command:command>
    <command:details>
        <command:name> Value </command:name>
        <command:verb> Verb </command:verb>
        <command:noun> Noun </command:noun>
    </command:details>
</command:command>
```
##Synopsis
Synopsis = Command.Details.Description.Para
Contained in Command.Details.Description

```
<maml:Description>
    <maml:para> Short Description </maml:para>
    <maml:para> Short Description Part 2 </maml:para>
    etc, etc
</maml:Description>
```
##Description
Description = Command.Description.Para
Contained in Command.Description

```
<command:command>
    <maml:description>
    <maml:para> Long Description </maml:para>
    <maml:para> Long Description Part 2 </maml:para>
    <maml:para> Long Description Part 3 </maml:para>
    <maml:para> Long Description Part 4 </maml:para>
    etc, etc
    </maml:description>
<command>
```
##Parameters
Parameter Name = Syntax.SyntaxItem.Parameter.Name
Contained in Command.Syntax

There is a lot of data that needs to be provided to generate the proper maml, shown in the code snip below.


```
<command:command>
    <command:syntax>
        <command:syntaxItem>
            <maml:Name>Cmdlet Name</maml:Name>
            <command:parameters>
                <command:parameter
                        required="Bool" 
                        variableLength="Bool" 
                        globbing="Bool"
                        pipelineInput="Bool" 
                        position="Int" 
                      aliases="Alias string list, Comma Delimited">
                    <maml:Name>Parameter Name</maml:Name>
                    <maml:Description>
                        <maml:para>Description Part 1</maml:para>
                        <maml:para>Description Part 2</maml:para>
                        etc,etc
                    </maml:Description>
                     <command:parameterValue 
                        required="Bool" 
                        variableLength="Bool">
                     Data Type (commonly string)
                     </command:parameterValue>
                </command:parameter>
                <command:parameter>
                    Data for second parameter, see above.
                </command:parameter>
            </command:parameters>
        </command:syntaxItem>
    </command:syntax>
</command:command>
```

Out current approach is a code snippet that containts `[Parameter(...)]` and `[type]` for the parameter.
For example:

```
[Parameter(Mandatory = $true, ValueFromPipeline = $true)]
[switch]
```

##Inputs
Input Name = Command.InputTypes.InputType.Type.Name
Contained in Command.InputTypes

```
<command:command>
    <command:InputTypes>
        <command:InputType>
            <dev:Type>
                <maml:name>Data Type (System.String)</maml:name>
            </dev:Type>
            <maml:Description>
                <maml:Para>Description Part 1</maml:Para>
                <maml:Para>Description Part 2</maml:Para>
                etc,etc
            </maml:Description>
        </command:InputType>
    </command:InputTypes>
</command:command>
```
##Outputs
Output Name = Command.ReturnValues.ReturnValue.Type.Name
Contained in = Command.ReturnValues.ReturnValue

```
<command:command>
    <command:returnValues>
        <command:returnValue>
            <dev:Type>
                <maml:name>Data Type 
                    (Microsoft.PowerShell.Commands.ComputerChangeInfo)
                </maml:name>
            </dev:Type>
            <maml:Description>
                <maml:Para>Description Part 1</maml:Para>
                <maml:Para>Description Part 2</maml:Para>
                etc,etc
            </maml:Description>
        </command:returnValue>
    </command:returnValues>
</command:command>
```
##Notes
Notes = Command.AlertSet.Alert.Para
Contained in Command.AlertSet.Alert

```
<command:command>
 <maml:alertSet>
      <maml:title />
      <maml:alert>
            <maml:Para>Description Part 1</maml:Para>
            <maml:Para>Description Part 2</maml:Para>
            etc,etc
      </maml:alert>
    </maml:alertSet>
</command:command>
```
##Examples
Example Name = Command.Examples.Example.Title
Example Code Snip = Command.Examples.Example.Code
Contained In Command.Examples.Example

```
<command:command>
  <command:examples>
      <command:example>
        <maml:title> 

        -------------------------- EXAMPLE X -------------------------- 

        </maml:title>
        <maml:introduction>
          <maml:para></maml:para>
        </maml:introduction>
        <dev:code>
        PS C:\&gt;Add-Computer -DomainName Domain01 -Restart
        </dev:code>
        <dev:remarks>
            <maml:Para>Description Part 1</maml:Para>
            <maml:Para>Description Part 2</maml:Para>
            etc,etc
        </dev:remarks>
        <command:commandLines>
          <command:commandLine>
            <command:commandText />
          </command:commandLine>
        </command:commandLines>
      </command:example>
    </command:examples>
</command:command>
```
##Related Links
Text seen for the link = Command.relatedLinks.navigationLink.linkText
Actual Link valeu = Command.relatedLinks.navigationLink.URI
```
<maml:NavigationLink>
    <maml:linkText> Value </maml:linkText>
    <maml:URI> Value </maml:URI>
<maml:NavigationLink>
```
