using Markdown.MAML.Renderer;
using Markdown.MAML.Model.MAML;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using System.Collections;
using Markdown.MAML.Parser;
using Markdown.MAML.Model.Markdown;

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
            Assert.Equal(@"\\(", MarkdownV2Renderer.GetEscapedMarkdownText(@"\("));
            Assert.Equal(@"(", MarkdownV2Renderer.GetEscapedMarkdownText(@"("));
            Assert.Equal(@")", MarkdownV2Renderer.GetEscapedMarkdownText(@")"));
            Assert.Equal(@"\[", MarkdownV2Renderer.GetEscapedMarkdownText(@"["));
            Assert.Equal(@"\]", MarkdownV2Renderer.GetEscapedMarkdownText(@"]"));
            Assert.Equal(@"\`", MarkdownV2Renderer.GetEscapedMarkdownText(@"`"));
        }

        [Fact]
        public void ReturnsSyntaxString()
        {
            var command = new MamlCommand()
            {
                Name = "Get-Foo",
                SupportCommonParameters = true
            };

            var param1 = new MamlParameter()
            {
                Name = "Bar",
                Type = "BarObject"
            };

            var syntax = new MamlSyntax()
            {
                IsDefault = true
            };

            syntax.Parameters.Add(param1);

            string syntaxString = MarkdownV2Renderer.GetSyntaxString(command, syntax);
            Assert.Equal("Get-Foo [-Bar <BarObject>] [<CommonParameters>]", syntaxString);
        }

        [Fact]
        public void RendererIgnoresLineBreakWhenBodyIsEmpty()
        {
            var renderer = new MarkdownV2Renderer(ParserMode.Full);
            MamlCommand command = new MamlCommand()
            {
                Name = "Test-LineBreak",
                Notes = new SectionBody("", SectionFormatOption.LineBreakAfterHeader)
            };

            string markdown = renderer.MamlModelToString(command, null);

            Assert.DoesNotContain("\r\n\r\n\r\n", markdown);
        }

        [Fact]
        public void RendererLineBreakAfterParameter()
        {
            var renderer = new MarkdownV2Renderer(ParserMode.Full);

            MamlCommand command = new MamlCommand()
            {
                Name = "Test-LineBreak",
                Synopsis = new SectionBody("This is the synopsis"),
                Description = new SectionBody("This is a long description"),
                Notes = new SectionBody("This is a note")
            };

            var parameter1 = new MamlParameter()
            {
                Type = "String",
                Name = "Name",
                Required = true,
                Description = "Name description.",
                Globbing = true
            };

            var parameter2 = new MamlParameter()
            {
                Type = "String",
                Name = "Path",
                FormatOption = SectionFormatOption.LineBreakAfterHeader,
                Required = true,
                Description = "Path description.",
                Globbing = true
            };

            command.Parameters.Add(parameter1);
            command.Parameters.Add(parameter2);

            var syntax1 = new MamlSyntax()
            {
                ParameterSetName = "ByName"
            };

            syntax1.Parameters.Add(parameter1);
            syntax1.Parameters.Add(parameter2);
            command.Syntax.Add(syntax1);

            string markdown = renderer.MamlModelToString(command, null);

            // Does not use line break and should not be added
            Assert.Contains("### -Name\r\nName description.", markdown);

            // Uses line break and should be preserved
            Assert.Contains("### -Path\r\n\r\nPath description.", markdown);
        }

        [Fact]
        public void RendererLineBreakAfterParameterForUpdate()
        {
            var renderer = new MarkdownV2Renderer(ParserMode.FormattingPreserve);

            MamlCommand command = new MamlCommand()
            {
                Name = "Test-LineBreak",
                Synopsis = new SectionBody("This is the synopsis"),
                Description = new SectionBody("This is a long description"),
                Notes = new SectionBody("This is a note")
            };

            var parameter1 = new MamlParameter()
            {
                Type = "String",
                Name = "Name",
                Required = true,
                Description = "Name description.",
                Globbing = true
            };

            var parameter2 = new MamlParameter()
            {
                Type = "String",
                Name = "Path",
                FormatOption = SectionFormatOption.LineBreakAfterHeader,
                Required = true,
                Description = "Path description.",
                Globbing = true
            };

            command.Parameters.Add(parameter1);
            command.Parameters.Add(parameter2);

            var syntax1 = new MamlSyntax()
            {
                ParameterSetName = "ByName"
            };

            syntax1.Parameters.Add(parameter1);
            syntax1.Parameters.Add(parameter2);
            command.Syntax.Add(syntax1);

            string markdown = renderer.MamlModelToString(command, null);

            // Does not use line break and should not be added
            Assert.Contains("### -Name\r\nName description.\r\n\r\n```yaml", markdown);

            // Uses line break and should be preserved
            Assert.Contains("### -Path\r\n\r\nPath description.\r\n\r\n```yaml", markdown);
        }

        [Fact]
        public void RendersExamplesFromMaml()
        {
            var renderer = new MarkdownV2Renderer(ParserMode.Full);

            MamlCommand command = new MamlCommand()
            {
                Name = "Test-LineBreak",
            };

            var example1 = new MamlExample()
            {
                Title = "Example 1",
                Code = new[] { new MamlCodeBlock("PS C:\\> Get-Help") },
                Remarks = "This is an example to get help."
            };

            var example2 = new MamlExample()
            {
                Title = "Example 2",
                Code = new[] { new MamlCodeBlock("PS C:\\> Get-Help -Full") },
                Introduction = "Intro"
            };

            var example3 = new MamlExample()
            {
                Title = "Example 3",
                FormatOption = SectionFormatOption.LineBreakAfterHeader,
                Code = new[] { new MamlCodeBlock("PS C:\\> Get-Help", "powershell") },
                Remarks = "This is an example to get help."
            };

            var example4 = new MamlExample()
            {
                Title = "Example 4",
                FormatOption = SectionFormatOption.LineBreakAfterHeader,
                Code = new[] { new MamlCodeBlock("PS C:\\> Get-Help -Full") },
                Introduction = "Intro"
            };

            var example5 = new MamlExample()
            {
                Title = "---Example 5---",
                Code = new[] { new MamlCodeBlock("PS C:\\> Get-Help -Full") },
                Introduction = "With some dashes and no spaces"
            };

            var example6 = new MamlExample()
            {
                Title = "------------------ Example 6: With extra info ------------------",
                Code = new[] { new MamlCodeBlock("PS C:\\> Get-Help -Full") },
                Introduction = "Padded to 64 characters and spaces"
            };

            var example7 = new MamlExample()
            {
                Title = "Example 7: ".PadRight(66, 'A'),
                Code = new[] { new MamlCodeBlock("PS C:\\> Get-Help -Full") },
                Introduction = "Greater then 64 characters"
            };

            command.Examples.Add(example1);
            command.Examples.Add(example2);
            command.Examples.Add(example3);
            command.Examples.Add(example4);
            command.Examples.Add(example5);
            command.Examples.Add(example6);
            command.Examples.Add(example7);

            string markdown = renderer.MamlModelToString(command, null);

            // Does not use line break and should not be added
            Assert.Contains("### Example 1\r\n```", markdown);
            Assert.Contains("### Example 2\r\nIntro\r\n\r\n```", markdown);

            // Uses line break and should be preserved
            Assert.Contains("### Example 3\r\n\r\n```powershell", markdown);
            Assert.Contains("### Example 4\r\n\r\nIntro\r\n\r\n```", markdown);

            // Includes title padding that should be removed
            Assert.Contains("### Example 5\r\n", markdown);
            Assert.Contains("### Example 6: With extra info\r\n", markdown);
            Assert.Contains($"### {example7.Title}\r\n", markdown);
        }

        [Fact]
        public void RendererCreatesWorkflowParametersEntry()
        {
            var renderer = new MarkdownV2Renderer(ParserMode.Full);
            MamlCommand command = new MamlCommand()
            {
                Name = "Workflow",
                IsWorkflow = true
            };

            command.Syntax.Add(new MamlSyntax());

            string markdown = renderer.MamlModelToString(command, null);
            Common.AssertMultilineEqual(@"---
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
This cmdlet supports the following workflow common parameters: -PSParameterCollection, -PSComputerName, -PSCredential, -PSConnectionRetryCount, -PSConnectionRetryIntervalSec, -PSRunningTimeoutSec, -PSElapsedTimeoutSec, -PSPersist, -PSAuthentication, -PSAuthenticationLevel, -PSApplicationName, -PSPort, -PSUseSSL, -PSConfigurationName, -PSConnectionURI, -PSAllowRedirection, -PSSessionOption, -PSCertificateThumbprint, -PSPrivateMetadata, -AsJob, -JobName, and -InputObject. For more information, see [about_WorkflowCommonParameters](http://go.microsoft.com/fwlink/p/?LinkID=533952).

### CommonParameters
This cmdlet supports the common parameters: -Debug, -ErrorAction, -ErrorVariable, -InformationAction, -InformationVariable, -OutVariable, -OutBuffer, -PipelineVariable, -Verbose, -WarningAction, and -WarningVariable. For more information, see [about_CommonParameters](http://go.microsoft.com/fwlink/?LinkID=113216).

## INPUTS

## OUTPUTS

## NOTES

## RELATED LINKS
", markdown);
        }

        [Fact]
        public void RendererNormalizeQuotesAndDashes()
        {
            var renderer = new MarkdownV2Renderer(ParserMode.Full);
            MamlCommand command = new MamlCommand()
            {
                Name = "Test-Quotes",
                Description = new SectionBody(@"”“‘’––-")
            };

            string markdown = renderer.MamlModelToString(command, null);
            Common.AssertMultilineEqual(@"---
schema: 2.0.0
---

# Test-Quotes

## SYNOPSIS

## SYNTAX

## DESCRIPTION
""""''---

## EXAMPLES

## PARAMETERS

### CommonParameters
This cmdlet supports the common parameters: -Debug, -ErrorAction, -ErrorVariable, -InformationAction, -InformationVariable, -OutVariable, -OutBuffer, -PipelineVariable, -Verbose, -WarningAction, and -WarningVariable. For more information, see [about_CommonParameters](http://go.microsoft.com/fwlink/?LinkID=113216).

## INPUTS

## OUTPUTS

## NOTES

## RELATED LINKS
", markdown);
        }

        [Fact]
        public void RendererProduceMarkdownV2Output()
        {
            var renderer = new MarkdownV2Renderer(ParserMode.Full);
            MamlCommand command = new MamlCommand()
            {
                Name = "Get-Foo",
                Synopsis = new SectionBody("This is the synopsis"),
                Description = new SectionBody("This is a long description.\r\nWith two paragraphs. And the second one contains of few line! They should be auto-wrapped. Because, why not? We can do that kind of the things, no problem.\r\n\r\n-- Foo. Bar.\r\n-- Don't break. The list.\r\n-- Into. Pieces"),
                Notes = new SectionBody("This is a multiline note.\r\nSecond line.")
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
                Applicable = new string[] { "Module1", "Module2" }
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
            });
            command.Outputs.Add(new MamlInputOutput()
            {
                TypeName = "String",
                Description = "Output Description goes here!"
            });
            command.Examples.Add(new MamlExample()
            {
                Title = "Example 1",
                Code = new[] { new MamlCodeBlock("PS:> Get-Help -YouNeedIt") },
                Remarks = "This does stuff!"
            });
            command.Examples.Add(new MamlExample()
            {
                Title = "Example 2",
                Code = new[] { new MamlCodeBlock("PS:> Get-Help -YouNeedTwo") },
            });
            command.Examples.Add(new MamlExample()
            {
                Title = "Example 3",
                Code = new[] { new MamlCodeBlock("PS:> Get-Help -YouNeedTwo") },
            });
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

            // Note that the metadata should end up getting alphabetized.
            var metadata = new Hashtable();
            metadata["foo"] = "bar";
            metadata["null"] = null;
            string markdown = renderer.MamlModelToString(command, metadata);
            Common.AssertMultilineEqual(@"---
foo: bar
null:
schema: 2.0.0
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

-- Foo. Bar.
-- Don't break. The list.
-- Into. Pieces

## EXAMPLES

### Example 1
```
PS:> Get-Help -YouNeedIt
```

This does stuff!

### Example 2
```
PS:> Get-Help -YouNeedTwo
```

### Example 3
```
PS:> Get-Help -YouNeedTwo
```

## PARAMETERS

### -Name
Parameter Description.

```yaml
Type: String
Parameter Sets: (All)
Aliases: GF, Foos, Do
Accepted values: Value1, Value2
Applicable: Module1, Module2

Required: True
Position: 1
Default value: trololo
Accept pipeline input: True (ByValue)
Accept wildcard characters: True
```

### CommonParameters
This cmdlet supports the common parameters: -Debug, -ErrorAction, -ErrorVariable, -InformationAction, -InformationVariable, -OutVariable, -OutBuffer, -PipelineVariable, -Verbose, -WarningAction, and -WarningVariable. For more information, see [about_CommonParameters](http://go.microsoft.com/fwlink/?LinkID=113216).

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

        [Fact]
        public void RenderesAllParameterSetMoniker()
        {
            var renderer = new MarkdownV2Renderer(ParserMode.Full);
            MamlCommand command = new MamlCommand()
            {
                Name = "Get-Foo",
            };

            var commonParam = new MamlParameter()
            {
                Type = "String",
                Name = "Common",
                Required = true
            };

            var parameter1 = new MamlParameter()
            {
                Type = "String",
                Name = "First",
                Required = true
            };

            var parameter2 = new MamlParameter()
            {
                Type = "String",
                Name = "Second",
                Required = true
            };

            command.Parameters.Add(commonParam);
            command.Parameters.Add(parameter1);
            command.Parameters.Add(parameter2);

            var syntax1 = new MamlSyntax()
            {
                ParameterSetName = "FirstSyntax"
            };

            syntax1.Parameters.Add(commonParam);
            syntax1.Parameters.Add(parameter1);

            var syntax2 = new MamlSyntax()
            {
                ParameterSetName = "SecondSyntax",
                IsDefault = true
            };

            syntax2.Parameters.Add(commonParam);
            syntax2.Parameters.Add(parameter2);

            command.Syntax.Add(syntax1);
            command.Syntax.Add(syntax2);

            string markdown = renderer.MamlModelToString(command, null);
            Common.AssertMultilineEqual(@"---
schema: 2.0.0
---

# Get-Foo

## SYNOPSIS

## SYNTAX

### FirstSyntax
```
Get-Foo -Common <String> -First <String> [<CommonParameters>]
```

### SecondSyntax (Default)
```
Get-Foo -Common <String> -Second <String> [<CommonParameters>]
```

## DESCRIPTION

## EXAMPLES

## PARAMETERS

### -Common
```yaml
Type: String
Parameter Sets: (All)
Aliases:

Required: True
Position: Named
Default value: None
Accept pipeline input: false
Accept wildcard characters: False
```

### -First
```yaml
Type: String
Parameter Sets: FirstSyntax
Aliases:

Required: True
Position: Named
Default value: None
Accept pipeline input: false
Accept wildcard characters: False
```

### -Second
```yaml
Type: String
Parameter Sets: SecondSyntax
Aliases:

Required: True
Position: Named
Default value: None
Accept pipeline input: false
Accept wildcard characters: False
```

### CommonParameters
This cmdlet supports the common parameters: -Debug, -ErrorAction, -ErrorVariable, -InformationAction, -InformationVariable, -OutVariable, -OutBuffer, -PipelineVariable, -Verbose, -WarningAction, and -WarningVariable. For more information, see [about_CommonParameters](http://go.microsoft.com/fwlink/?LinkID=113216).

## INPUTS

## OUTPUTS

## NOTES

## RELATED LINKS
", markdown);
        }

        [Fact]
        public void RenderesWithPreservedFormatting()
        {
            var renderer = new MarkdownV2Renderer(ParserMode.FormattingPreserve);
            MamlCommand command = new MamlCommand()
            {
                Name = "Get-Foo",
                SupportCommonParameters = false,
                Description = new SectionBody(@"Hello
This \<description \> should be preserved by renderer
With all [hyper](https://links.com) and yada
  -- yada

* Also
* This list. May look. A little
weired
* [ ] But

* [ ] It should be left"
            )};

            command.Links.Add(
                new MamlLink(isSimplifiedTextLink: true)
                {
                    LinkName = "Any text [can](go here)\r\n[any](text)"
                }
            );

            string markdown = renderer.MamlModelToString(command, null);
            Common.AssertMultilineEqual(@"---
schema: 2.0.0
---

# Get-Foo

## SYNOPSIS

## SYNTAX

## DESCRIPTION
" + command.Description + @"

## EXAMPLES

## PARAMETERS

## INPUTS

## OUTPUTS

## NOTES

## RELATED LINKS

Any text [can](go here)
[any](text)", markdown);
        }
    }
}
