using Markdown.MAML.Renderer;
using Markdown.MAML.Model.MAML;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using System.Collections;

namespace Markdown.MAML.Test.Renderer
{
    public class MarkdownV2RendererTests
    {
        [Fact]
        public void RendererUsesCorrectEscaping()
        {
            Assert.Equal(@"\\\<", MarkdownV2Renderer.GetEscapedMarkdownText(@"\<"));
            Assert.Equal(@"\\\`", MarkdownV2Renderer.GetEscapedMarkdownText(@"\`"));
            Assert.Equal(@"\\\\\<", MarkdownV2Renderer.GetEscapedMarkdownText(@"\\<"));
            Assert.Equal(@"\\\\\\\<", MarkdownV2Renderer.GetEscapedMarkdownText(@"\\\<"));
            Assert.Equal(@"\", MarkdownV2Renderer.GetEscapedMarkdownText(@"\"));
            Assert.Equal(@"\\\\", MarkdownV2Renderer.GetEscapedMarkdownText(@"\\"));
            Assert.Equal(@"\\\(", MarkdownV2Renderer.GetEscapedMarkdownText(@"\("));
            Assert.Equal(@"\(", MarkdownV2Renderer.GetEscapedMarkdownText(@"("));
            Assert.Equal(@"\)", MarkdownV2Renderer.GetEscapedMarkdownText(@")"));
            Assert.Equal(@"\[", MarkdownV2Renderer.GetEscapedMarkdownText(@"["));
            Assert.Equal(@"\]", MarkdownV2Renderer.GetEscapedMarkdownText(@"]"));
            Assert.Equal(@"\`", MarkdownV2Renderer.GetEscapedMarkdownText(@"`"));
        }

        [Fact]
        public void RendererCreatesWorkflowParametersEntry()
        {
            var renderer = new MarkdownV2Renderer();
            MamlCommand command = new MamlCommand()
            {
                Name = "Workflow",
                IsWorkflow = true
            };

            command.Syntax.Add(new MamlSyntax());

            string markdown = renderer.MamlModelToString(command, null);
            Assert.Equal(@"---
schema: 2.0.0
---

# Workflow
## SYNOPSIS

## SYNTAX

```
Workflow [<WorkflowCommonParameters>] [<CommonParameters>]
```

## DESCRIPTION

## EXAMPLES

## PARAMETERS

### WorkflowCommonParameters
This cmdlet supports the following workflow common parameters: -PSParameterCollection, -PSComputerName, -PSCredential, -PSConnectionRetryCount, -PSConnectionRetryIntervalSec, -PSRunningTimeoutSec, -PSElapsedTimeoutSec, -PSPersist, -PSAuthentication, -PSAuthenticationLevel, -PSApplicationName, -PSPort, -PSUseSSL, -PSConfigurationName, -PSConnectionURI, -PSAllowRedirection, -PSSessionOption, -PSCertificateThumbprint, -PSPrivateMetadata, -AsJob, -JobName, and –InputObject.
For more information, see about_WorkflowCommonParameters \(http://go.microsoft.com/fwlink/p/?LinkID=533952\).

### CommonParameters
This cmdlet supports the common parameters: -Debug, -ErrorAction, -ErrorVariable, -InformationAction, -InformationVariable, -OutVariable, -OutBuffer, -PipelineVariable, -Verbose, -WarningAction, and -WarningVariable.
For more information, see about_CommonParameters \(http://go.microsoft.com/fwlink/?LinkID=113216\).

## INPUTS

## OUTPUTS

## NOTES

## RELATED LINKS

", markdown);
        }

        [Fact]
        public void RendererProduceMarkdownV2Output()
        {
            var renderer = new MarkdownV2Renderer();
            MamlCommand command = new MamlCommand()
            {
                Name = "Get-Foo",
                Synopsis = "This is the synopsis",
                Description = "This is a long description.\r\nWith two paragraphs. And the second one contains of few line! They should be auto-wrapped. Because, why not? We can do that kind of the things, no problem.",
                Notes = "This is a multiline note.\r\nSecond line."
            };

            var parameter = new MamlParameter()
            {
                Type = "String",
                Name = "Name",
                Required = true,
                Description = "Parameter Description.",
                VariableLength = true,
                Globbing = true,
                PipelineInput = "True (ByValue)",
                Position = "1",
                DefaultValue = "trololo",
                Aliases = new string[] { "GF", "Foos", "Do" },
            };
            parameter.ParameterValueGroup.AddRange(new string[] { "Value1", "Value2" });

            command.Parameters.Add(parameter);

            var syntax1 = new MamlSyntax()
            {
                ParameterSetName = "ByName"
            };
            syntax1.Parameters.Add(parameter);
            command.Syntax.Add(syntax1);

            command.Inputs.Add(new MamlInputOutput()
            {
                TypeName = "String",
                Description = "Input <Description> goes here!"
            }
            );
            command.Outputs.Add(new MamlInputOutput()
            {
                TypeName = "String",
                Description = "Output Description goes here!"
            }
            );
            command.Examples.Add(new MamlExample()
            {
                Title = "Example 1",
                Code = "PS:> Get-Help -YouNeedIt",
                Remarks = "This does stuff!"
            }
            );
            command.Links.Add(new MamlLink()
            {
                LinkName = "PowerShell made by Microsoft Hackathon",
                LinkUri = "www.microsoft.com"

            });

            command.Links.Add(new MamlLink()
            {
                LinkName = "", // if name is empty, it would be populated with uri
                LinkUri = "http://foo.com"

            });

            var metadata = new Hashtable();
            metadata["foo"] = "bar";
            metadata["null"] = null;
            string markdown = renderer.MamlModelToString(command, metadata);
            Assert.Equal(@"---
schema: 2.0.0
foo: bar
null: 
---

# Get-Foo
## SYNOPSIS
This is the synopsis

## SYNTAX

```
Get-Foo [-Name] <String> [<CommonParameters>]
```

## DESCRIPTION
This is a long description.

With two paragraphs.
And the second one contains of few line!
They should be auto-wrapped.
Because, why not?
We can do that kind of the things, no problem.

## EXAMPLES

### Example 1
```
PS:> Get-Help -YouNeedIt
```

This does stuff!

## PARAMETERS

### -Name
Parameter Description.

```yaml
Type: String
Aliases: GF, Foos, Do
Accepted values: Value1, Value2

Required: True
Position: 1
Default value: trololo
Accept pipeline input: True (ByValue)
Accept wildcard characters: True
```

### CommonParameters
This cmdlet supports the common parameters: -Debug, -ErrorAction, -ErrorVariable, -InformationAction, -InformationVariable, -OutVariable, -OutBuffer, -PipelineVariable, -Verbose, -WarningAction, and -WarningVariable.
For more information, see about_CommonParameters \(http://go.microsoft.com/fwlink/?LinkID=113216\).

## INPUTS

### String
Input \<Description\> goes here!

## OUTPUTS

### String
Output Description goes here!

## NOTES
This is a multiline note.

Second line.

## RELATED LINKS

[PowerShell made by Microsoft Hackathon](www.microsoft.com)

[http://foo.com](http://foo.com)

", markdown);
        }
    }
}
