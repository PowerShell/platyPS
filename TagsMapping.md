#Tag Mapping: MAML to MarkDown
Outlines the mapping of MAML required elements to their MarkDown counterparts.

#Elements

Note about Para Elements
The para elements can have multiple entries. Comand.Details.Description is the container for them all.

##Module Name
The name of the help.xml file corresponds to the name of the cmdlet providing module of assembly. In version (?) 4 of PowerShell the help.xml can be named after the module instead of the assembly that grants cmdlets.

##Cmdlet Name
Name = Command.Details.Name

```
<command:details>
    <command:name> Value </command:name>
    <command:verb> Verb </command:verb>
    <command:noun> Noun </command:noun>
</command:details>
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
Contained in Command.Details.Description

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

```
<maml:NavigationLink>
    <maml:linkText> Value </maml:linkText>
    <maml:URI> Value </maml:URI>
<maml:NavigationLink>
```
##Inputs

```
<maml:NavigationLink>
    <maml:linkText> Value </maml:linkText>
    <maml:URI> Value </maml:URI>
<maml:NavigationLink>
```
##Outputs

```
<maml:NavigationLink>
    <maml:linkText> Value </maml:linkText>
    <maml:URI> Value </maml:URI>
<maml:NavigationLink>
```
##Notes

```
<maml:NavigationLink>
    <maml:linkText> Value </maml:linkText>
    <maml:URI> Value </maml:URI>
<maml:NavigationLink>
```
##Examples

```
<maml:NavigationLink>
    <maml:linkText> Value </maml:linkText>
    <maml:URI> Value </maml:URI>
<maml:NavigationLink>
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
