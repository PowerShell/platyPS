using System.Collections.Generic;
using System.Xml;
using System.Xml.XPath;
using Markdown.MAML.Renderer;
using Xunit;
using Markdown.MAML.Parser;
using Markdown.MAML.Transformer;
using System.Linq;

namespace Markdown.MAML.Test.EndToEnd
{
    public class EndToEndTests
    {
        [Fact]
        public void ProduceMamlFromMarkdown()
        {
            string maml = MarkdownStringToMamlString(@"
# Get-Foo
## Synopsis
This is Synopsis
## Examples
### Example 1
```
PS C:\> Update-MarkdownHelp
```

This is example 1 remark.

### Example 2: With a long title
This is an example description.

```
PS C:\> Update-MarkdownHelp
```

This is example 2 remark.
");
            string[] name = GetXmlContent(maml, "/msh:helpItems/command:command/command:details/command:name");
            Assert.Single(name);
            Assert.Equal("Get-Foo", name[0]);

            string[] synopsis = GetXmlContent(maml, "/msh:helpItems/command:command/command:details/maml:description/maml:para");
            Assert.Single(synopsis);
            Assert.Equal("This is Synopsis", synopsis[0]);

            // Check that example title is reproduced with dash (-) padding
            string[] example = EndToEndTests.GetXmlContent(maml, "/msh:helpItems/command:command/command:examples/command:example/maml:title");
            Assert.Equal(63, example[0].Length);
            Assert.Equal(64, example[1].Length);
            Assert.Matches($"^-+ Example 1 -+$", example[0]);
            Assert.Matches($"^-+ Example 2: With a long title -+$", example[1]);
        }

        [Fact]
        public void ProduceMultilineDescription()
        {
            string maml = MarkdownStringToMamlString(@"
# Get-Foo
## Synopsis
This is Synopsis, but it doesn't matter in this test

## DESCRIPTION
Hello,

I'm a multiline description.

And this is my last line.
");

            string[] description = GetXmlContent(maml, "/msh:helpItems/command:command/maml:description/maml:para");
            Assert.Equal(3, description.Length);
        }

        [Fact]
        public void PreserveMarkdownWhenUpdatingMarkdownHelp()
        {
            var expected = @"# Update-MarkdownHelp

## SYNOPSIS

Example markdown to test that markdown is preserved.

## SYNTAX

```
Update-MarkdownHelp [-Name] <String> [-Path <String>]
```

## DESCRIPTION
When calling Update-MarkdownHelp line breaks should be preserved.

## EXAMPLES

### Example 1: With no line break or description
```powershell
PS C:\> Write-Host 'This is output.'
```

```
This is output.
```

This is example 1 remark.

### Example 2: With no line break
This is an example description.

```powershell
PS C:\> Update-MarkdownHelp
```

This is example 2 remark.

### Example 3: With line break and no description

```powershell
PS C:\> Update-MarkdownHelp
```

This is example 3 remark.

### Example 4: With line break and description

This is an example description.

```preserve
PS C:\> Update-MarkdownHelp
```

```text
Output
```

This is example 4 remark.

## PARAMETERS

### -Name

Parameter name description with line break.

```yaml
Type: String
Parameter Sets: (All)
Aliases:

Required: True
Position: 1
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -Path
Parameter path description with no line break.

```yaml
Type: String
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

## INPUTS

### String[]

This is an input description.

## OUTPUTS

### System.Object

This is an output description.

## NOTES

## RELATED LINKS
";
            
            // Parse markdown and convert back to markdown to make sure there are no changes
            var actualFull = MarkdownStringToMarkdownString(expected, ParserMode.Full);
            var actualFormattingPreserve = MarkdownStringToMarkdownString(expected, ParserMode.FormattingPreserve);

            Common.AssertMultilineEqual(expected, actualFull);
            Common.AssertMultilineEqual(expected, actualFormattingPreserve);
        }

        [Fact]
        public void CanUseSharpInsideParagraphs()
        {
            string maml = MarkdownStringToMamlString(@"
# Get-Foo
## Synopsis
This is Synopsis #hashtagNotAHeader.

## DESCRIPTION
I'm description
");

            string[] description = GetXmlContent(maml, "/msh:helpItems/command:command/maml:description/maml:para");
            Assert.Single(description);
            Assert.Equal("I'm description", description[0]);

            string[] synopsis = GetXmlContent(maml, "/msh:helpItems/command:command/command:details/maml:description/maml:para");
            Assert.Single(synopsis);
            Assert.Equal("This is Synopsis #hashtagNotAHeader.", synopsis[0]);
        }

        [Fact]
        public void CanUseRelativeLinksInRelatedLinksSection()
        {
            // more details: https://github.com/PowerShell/platyPS/issues/164

            string maml = MarkdownStringToMamlString(@"
# Get-Foo

## RELATED LINKS

[foo](foo.md)
[bar](bar://bar.md)
");

            string[] linkText = GetXmlContent(maml, "/msh:helpItems/command:command/command:relatedLinks/maml:navigationLink/maml:linkText");
            Assert.Equal(2, linkText.Length);
            Assert.Equal("foo", linkText[0]);
            Assert.Equal("bar", linkText[1]);

            string[] linkUri = GetXmlContent(maml, "/msh:helpItems/command:command/command:relatedLinks/maml:navigationLink/maml:uri");
            Assert.Equal(2, linkUri.Length);
            Assert.Equal("", linkUri[0]); // empty, because foo.md is not a valid URI and help system will be unhappy.
            Assert.Equal("bar://bar.md", linkUri[1]); // bar://bar.md is a valid URI
        }

        [Fact]
        public void CanProcessAddHistoryAddSnapinMarkdownWithoutErrors()
        {
            string maml = MarkdownStringToMamlString(@"
# Add-History

## SYNOPSIS
Appends entries to the session history.

## DESCRIPTION
The Add-History cmdlet adds entries to the end of the session history, that is, the list of commands entered during the current session.
You can use the Get-History cmdlet to get the commands and pass them to Add-History, or you can export the commands to a CSV or XML file, then import the commands, and pass the imported file to Add-History. You can use this cmdlet to add specific commands to the history or to create a single history file that includes commands from more than one session.

## PARAMETERS

### InputObject [PSObject[]]

```powershell
[Parameter(
  Position = 1,
  ValueFromPipeline = $true,
  ParameterSetName = 'Set 1')]
```

Adds the specified HistoryInfo object to the session history. You can use this parameter to submit a HistoryInfo object, such as the ones that are returned by the Get-History, Import-Clixml, or Import-Csv cmdlets, to Add-History.

### Passthru [switch]

```powershell
[Parameter(ParameterSetName = 'Set 1')]
```

Returns a history object for each history entry. By default, this cmdlet does not generate any output.


## INPUTS
### Microsoft.PowerShell.Commands.HistoryInfo
You can pipe a HistoryInfo object to Add-History.

## OUTPUTS
### None or Microsoft.PowerShell.Commands.HistoryInfo
When you use the PassThru parameter, Add-History returns a HistoryInfo object. Otherwise, this cmdlet does not generate any output.

## NOTES
The session history is a list of the commands entered during the session along with the ID. The session history represents the order of execution, the status, and the start and end times of the command. As you enter each command, Windows PowerShell adds it to the history so that you can reuse it.  For more information about the session history, see about_History.
To specify the commands to add to the history, use the InputObject parameter. The Add-History command accepts only HistoryInfo objects, such as those returned for each command by the Get-History cmdlet. You cannot pass it a path and file name or a list of commands.
You can use the InputObject parameter to pass a file of HistoryInfo objects to Add-History. To do so, export the results of a Get-History command to a file by using the Export-Csv or Export-Clixml cmdlet and then import the file by using the Import-Csv or Import-Clixml cmdlets. You can then pass the file of imported HistoryInfo objects to Add-History through a pipeline or in a variable. For more information, see the examples.
The file of HistoryInfo objects that you pass to the Add-History cmdlet must include the type information, column headings, and all of the properties of the HistoryInfo objects. If you intend to pass the objects back to Add-History, do not use the NoTypeInformation parameter of the Export-Csv cmdlet and do not delete the type information, column headings, or any fields in the file.
To edit the session history, export the session to a CSV or XML file, edit the file, import the file, and use Add-History to append it to the current session history.


## EXAMPLES
### -------------------------- EXAMPLE 1 --------------------------

```powershell
PS C:\>get-history | export-csv c:\testing\history.csv
PS C:\>import-csv history.csv | add-history

```
These commands add the commands typed in one Windows PowerShell session to the history of a different Windows PowerShell session. The first command gets objects representing the commands in the history and exports them to the History.csv file. The second command is typed at the command line of a different session. It uses the Import-Csv cmdlet to import the objects in the History.csv file. The pipeline operator passes the objects to the Add-History cmdlet, which adds the objects representing the commands in the History.csv file to the current session history.






### -------------------------- EXAMPLE 2 --------------------------

```powershell
PS C:\>import-clixml c:\temp\history.xml | add-history -passthru | foreach-object -process {invoke-history}

```
This command imports commands from the History.xml file, adds them to the current session history, and then executes the commands in the combined history.
The first command uses the Import-Clixml cmdlet to import a command history that was exported to the History.xml file. The pipeline operator (|) passes the commands to the Add-History parameter, which adds the commands to the current session history. The PassThru parameter passes the objects representing the added commands down the pipeline.
The command then uses the ForEach-Object cmdlet to apply the Invoke-History command to each of the commands in the combined history. The Invoke-History command is formatted as a script block (enclosed in braces) as required by the Process parameter of the ForEach-Object cmdlet.






### -------------------------- EXAMPLE 3 --------------------------

```powershell
PS C:\>get-history -id 5 -count 5 | add-history

```
This command adds the first five commands in the history to the end of the history list. It uses the Get-History cmdlet to get the five commands ending in command 5. The pipeline operator (|) passes them to the Add-History cmdlet, which appends them to the current history. The Add-History command does not include any parameters, but Windows PowerShell associates the objects passed through the pipeline with the InputObject parameter of  Add-History.






### -------------------------- EXAMPLE 4 --------------------------

```powershell
PS C:\>$a = import-csv c:\testing\history.csv
PS C:\>add-history -inputobject $a -passthru

```
These commands add the commands in the History.csv file to the current session history. The first command uses the Import-Csv cmdlet to import the commands in the History.csv file and store its contents in the variable $a. The second command uses the Add-History cmdlet to add the commands from History.csv to the current session history. It uses the InputObject parameter to specify the $a variable and the PassThru parameter to generate an object to display at the command line. Without the PassThru parameter, the Add-History cmdlet does not generate any output.






### -------------------------- EXAMPLE 5 --------------------------

```powershell
PS C:\>add-history -inputobject (import-clixml c:\temp\history01.xml)

```
This command adds the commands in the History01.xml file to the current session history. It uses the InputObject parameter to pass the results of the command in parentheses to the Add-History cmdlet. The command in parentheses, which is executed first, imports the History01.xml file into Windows PowerShell. The Add-History cmdlet then adds the commands in the file to the session history.







## RELATED LINKS
[Online Version:](http://go.microsoft.com/fwlink/p/?linkid=289569)
[Clear-History]()
[Get-History]()
[Invoke-History]()
[about_History]()

# Add-PSSnapin

## SYNOPSIS
Adds one or more Windows PowerShell snap-ins to the current session.

## DESCRIPTION
The Add-PSSnapin cmdlet adds registered Windows PowerShell snap-ins to the current session. After the snap-ins are added, you can use the cmdlets and providers that the snap-ins support in the current session.
To add the snap-in to all future Windows PowerShell sessions, add an Add-PSSnapin command to your Windows PowerShell profile. For more information, see about_Profiles.
Beginning in Windows PowerShell 3.0, the core commands that are included in Windows PowerShell are packaged in modules. The exception is Microsoft.PowerShell.Core, which is a snap-in (PSSnapin). By default, only the Microsoft.PowerShell.Core snap-in is added to the session. Modules are imported automatically on first use and you can use the Import-Module cmdlet to import them.

## PARAMETERS

### Name [String[]]

```powershell
[Parameter(
  Mandatory = $true,
  Position = 1,
  ValueFromPipelineByPropertyName = $true,
  ParameterSetName = 'Set 1')]
```

Specifies the name of the snap-in. (This is the Name, not the AssemblyName or ModuleName.) Wildcards are permitted.
To find the names of the registered snap-ins on your system, type: ""get-pssnapin -registered"".

### PassThru [switch]

```powershell
[Parameter(ParameterSetName = 'Set 1')]
```

Returns an object representing each added snap-in. By default, this cmdlet does not generate any output.


## INPUTS
### None
You cannot pipe objects to Add-PSSnapin.

## OUTPUTS
### None or System.Management.Automation.PSSnapInInfo
When you use the PassThru parameter, Add-PSSnapin returns a PSSnapInInfo object that represents the snap-in. Otherwise, this cmdlet does not generate any output.

## NOTES
Beginning in Windows PowerShell 3.0, the core commands that are installed with Windows PowerShell are packaged in modules. In Windows PowerShell 2.0, and in host programs that create older-style sessions in later versions of Windows PowerShell, the core commands are packaged in snap-ins (""PSSnapins""). The exception is Microsoft.PowerShell.Core, which is always a snap-in. Also, remote sessions, such as those started by the New-PSSession cmdlet, are older-style sessions that include core snap-ins.
For information about the CreateDefault2 method that creates newer-style sessions with core modules, see ""CreateDefault2 Method"" in MSDN at [http://msdn.microsoft.com/en-us/library/windows/desktop/system.management.automation.runspaces.initialsessionstate.createdefault2(v=VS.85).aspx]().
For detailed information about snap-ins in Windows PowerShell, see about_Pssnapins. For information about how to create a Windows PowerShell snap-in, see ""How to Create a Windows PowerShell Snap-in"" in the MSDN (Microsoft Developer Network) library at http://go.microsoft.com/fwlink/?LinkId=144762http://go.microsoft.com/fwlink/?LinkId=144762.
Add-PSSnapin adds the snap-in only to the current session. To add the snap-in to all Windows PowerShell sessions, add it to your Windows PowerShell profile. For more information, see about_Profiles.
You can add any Windows PowerShell snap-in that has been registered by using the Microsoft .NET Framework install utility. For more information, see ""How to Register Cmdlets, Providers, and Host Applications"" in the MSDN library at http://go.microsoft.com/fwlink/?LinkID=143619http://go.microsoft.com/fwlink/?LinkID=143619.
To get a list of snap-ins that are registered on your computer, type get-pssnapin -registered.
Before adding a snap-in, Add-PSSnapin checks the version of the snap-in to verify that it is compatible with the current version of Windows PowerShell. If the snap-in fails the version check, Windows PowerShell reports an error.

## EXAMPLES
### -------------------------- EXAMPLE 1 --------------------------

```powershell
PS C:\>add-PSSnapIn Microsoft.Exchange, Microsoft.Windows.AD

```
This command adds the Microsoft Exchange and Active Directory snap-ins to the current session.






### -------------------------- EXAMPLE 2 --------------------------

```powershell
PS C:\>get-pssnapin -registered | add-pssnapin -passthru

```
This command adds all of the registered Windows PowerShell snap-ins to the session. It uses the Get-PSSnapin cmdlet with the Registered parameter to get objects representing each of the registered snap-ins. The pipeline operator (|) passes the result to Add-PSSnapin, which adds them to the session. The PassThru parameter returns objects that represent each of the added snap-ins.






### -------------------------- EXAMPLE 3 --------------------------

```powershell
The first command gets snap-ins that have been added to the current session, including the snap-ins that are installed with Windows PowerShell. In this example, ManagementFeatures is not returned. This indicates that it has not been added to the session.
PS C:\>get-pssnapin

The second command gets snap-ins that have been registered on your system (including those that have already been added to the session). It does not include the snap-ins that are installed with Windows PowerShell.In this case, the command does not return any snap-ins. This indicates that the ManagementFeatures snapin has not been registered on the system.
PS C:\>get-pssnapin -registered

The third command creates an alias, ""installutil"", for the path to the InstallUtil tool in .NET Framework.
PS C:\>set-alias installutil $env:windir\Microsoft.NET\Framework\v2.0.50727\installutil.exe

The fourth command uses the InstallUtil tool to register the snap-in. The command specifies the path to ManagementCmdlets.dll, the file name or ""module name"" of the snap-in.
PS C:\>installutil C:\Dev\Management\ManagementCmdlets.dll

The fifth command is the same as the second command. This time, you use it to verify that the ManagementCmdlets snap-in is registered.
PS C:\>get-pssnapin -registered

The sixth command uses the Add-PSSnapin cmdlet to add the ManagementFeatures snap-in to the session. It specifies the name of the snap-in, ManagementFeatures, not the file name.
PS C:\>add-pssnapin ManagementFeatures

To verify that the snap-in is added to the session, the seventh command uses the Module parameter of the Get-Command cmdlet. It displays the items that were added to the session by a snap-in or module.
PS C:\>get-command -module ManagementFeatures

You can also use the PSSnapin property of the object that the Get-Command cmdlet returns to find the snap-in or module in which a cmdlet originated. The eighth command uses dot notation to find the value of the PSSnapin property of the Set-Alias cmdlet.
PS C:\>(get-command set-alias).pssnapin

```
This example demonstrates the process of registering a snap-in on your system and then adding it to your session. It uses ManagementFeatures, a fictitious snap-in implemented in a file called ManagementCmdlets.dll.



## RELATED LINKS
[Online Version:](http://go.microsoft.com/fwlink/p/?linkid=289570)
[Get-PSSnapin]()
[Remove-PSSnapin]()
[about_Profiles]()
[about_PSSnapins]()

");
            string[] examples = GetXmlContent(maml, "msh:helpItems/command:command/command:examples/command:example/dev:code");
            Assert.Equal(5 + 3, examples.Length);
        }

        public static string[] GetXmlContent(string xml, string xpath)
        {
            List<string> result = new List<string>(); 
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xml);
            var nav = xmlDoc.CreateNavigator();

            XmlNamespaceManager xmlns = new XmlNamespaceManager(nav.NameTable);
            xmlns.AddNamespace("command", "http://schemas.microsoft.com/maml/dev/command/2004/10");
            xmlns.AddNamespace("maml", "http://schemas.microsoft.com/maml/2004/10");
            xmlns.AddNamespace("dev", "http://schemas.microsoft.com/maml/dev/2004/10");
            xmlns.AddNamespace("MSHelp", "http://msdn.microsoft.com/mshelp");
            xmlns.AddNamespace("msh", "http://msh");

            XPathNodeIterator iterator = nav.Select(xpath, xmlns);
            foreach (var i in iterator)
            {
                result.Add(i.ToString().Trim());
            }

            return result.ToArray();
        }

        /// <summary>
        /// This is a helper method to do all 3 steps.
        /// </summary>
        /// <param name="markdown"></param>
        /// <returns></returns>
        public static string MarkdownStringToMamlString(string markdown)
        {
            var parser = new MarkdownParser();
            var transformer = new ModelTransformerVersion2();
            var renderer = new MamlRenderer();

            var markdownModel = parser.ParseString(new string[] { markdown });
            var mamlModel = transformer.NodeModelToMamlModel(markdownModel);
            string maml = renderer.MamlModelToString(mamlModel);

            return maml;
        }

        private string MarkdownStringToMarkdownString(string markdown, ParserMode parserMode)
        {
            // Parse
            var parser = new MarkdownParser();
            var markdownModel = parser.ParseString(new string[] { markdown }, ParserMode.FormattingPreserve, null);

            // Convert model to Maml
            var transformer = new ModelTransformerVersion2();
            var mamlModel = transformer.NodeModelToMamlModel(markdownModel).FirstOrDefault();

            // Render as markdown
            var renderer = new MarkdownV2Renderer(parserMode);
            return renderer.MamlModelToString(mamlModel, true);
        }
    }
}
