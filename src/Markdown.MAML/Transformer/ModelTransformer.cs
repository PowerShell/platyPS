using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Markdown.MAML.Model;
using Markdown.MAML.Model.Markdown;
using Markdown.MAML.Model.MAML;
using System.Management.Automation;
using System.Management.Automation.Runspaces;

namespace Markdown.MAML.Transformer
{
    public class ModelTransformer
    {
        private Runspace runspace;
        private DocumentNode _root;
        private IEnumerator<MarkdownNode> _rootEnumerator;

        private const int COMMAND_NAME_HEADING_LEVEL = 2;
        private const int COMMAND_ENTRIES_HEADING_LEVEL = 3;
        private const int PARAMETER_NAME_HEADING_LEVEL = 4;
        private const int INPUT_OUTPUT_TYPENAME_HEADING_LEVEL = 4;
        private const int EXAMPLE_HEADING_LEVEL = 4;

        private MarkdownNode _ungotNode { get; set; }

        private MarkdownNode GetCurrentNode()
        {
            if (_ungotNode != null)
            {
                var node = _ungotNode;
                return node;
            }

            return _rootEnumerator.Current;
        }

        private MarkdownNode GetNextNode()
        {
            if (_ungotNode != null)
            {
                _ungotNode = null;
                return _rootEnumerator.Current;
            }

            if (_rootEnumerator.MoveNext())
            {
                return _rootEnumerator.Current;
            }

            return null;
        }

        private void UngetNode(MarkdownNode node)
        {
            if (_ungotNode != null)
            {
                throw new ArgumentException("Cannot ungot token, already ungot one");
            }

            _ungotNode = node;
        }

        private string SimpleTextSectionRule()
        {
            // grammar:
            // Simple paragraph Text
            return GetTextFromParagraphNode(ParagraphNodeRule());
        }

        private void ParametersRule(MamlCommand commmand)
        {
            while (ParameterRule(commmand))
            {
            }

            this.GatherParameterDetails(commmand);
        }

        private void InputsRule(MamlCommand commmand)
        {
            MamlInputOutput input;
            while ((input = InputOutputRule()) != null)
            {
                commmand.Inputs.Add(input);
            }
        }

        private void OutputsRule(MamlCommand commmand)
        {
            MamlInputOutput output;
            while ((output = InputOutputRule()) != null)
            {
                commmand.Outputs.Add(output);
            }
        }

        private void ExamplesRule(MamlCommand commmand)
        {
            MamlExample example;
            while ((example = ExampleRule()) != null)
            {
                commmand.Examples.Add(example);
            }
        }

        private MamlExample ExampleRule()
        {
            // grammar:
            // #### ExampleTitle
            // ```
            // code
            // ```
            // Explanation
            var node = GetNextNode();
            var headingNode = GetHeadingWithExpectedLevel(node, EXAMPLE_HEADING_LEVEL);
            if (headingNode == null)
            {
                return null;
            }

            MamlExample example = new MamlExample()
            {
                Title = headingNode.Text
            };

            var codeBlock = CodeBlockRule();

            example.Code = codeBlock.Text;
            example.Remarks = GetTextFromParagraphNode(ParagraphNodeRule());
            
            return example;
        }

        private void RelatedLinksRule(MamlCommand commmand)
        {
            var paragraphNode = ParagraphNodeRule();
            foreach (var paragraphSpan in paragraphNode.Spans)
            {
                var linkSpan = paragraphSpan as HyperlinkSpan;
                if (linkSpan != null)
                {
                    commmand.Links.Add(new MamlLink()
                    {
                        LinkName = linkSpan.Text,
                        LinkUri = linkSpan.Uri
                    });
                }
                else
                {
                    throw new HelpSchemaException(paragraphSpan.SourceExtent, "Expect hyperlink, but got " + paragraphSpan.Text);
                }
            }
        }

        private MamlInputOutput InputOutputRule()
        {
            // grammar:
            // #### TypeName
            // Description
            var node = GetNextNode();
            var headingNode = GetHeadingWithExpectedLevel(node, INPUT_OUTPUT_TYPENAME_HEADING_LEVEL);
            if (headingNode == null)
            {
                return null;
            }

            MamlInputOutput typeEntity = new MamlInputOutput()
            {
                TypeName = headingNode.Text
            };

            typeEntity.Description = SimpleTextSectionRule();

            return typeEntity;
        }

        private SourceExtent GetExtent(MarkdownNode node)
        {
            TextNode textNode = node as TextNode;
            if (textNode != null)
            {
                return textNode.SourceExtent;
            }
            ParagraphNode paragraphNode = node as ParagraphNode;
            if (paragraphNode != null && paragraphNode.Spans.Any())
            {
                return paragraphNode.Spans.First().SourceExtent;
            }

            return new SourceExtent("", 0, 0, 0, 0);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="node"></param>
        /// <param name="level"></param>
        /// <returns>
        /// return headingNode if expected heading level encounterd.
        /// null, if higher level encountered.
        /// throw exception, if unexpected node encountered.
        /// </returns>
        private HeadingNode GetHeadingWithExpectedLevel(MarkdownNode node, int level)
        {
            if (node == null)
            {
                return null;
            }

            // check for appropriate header
            if (node.NodeType != MarkdownNodeType.Heading)
            {
                throw new HelpSchemaException(GetExtent(node), "Expect Heading");
            }

            var headingNode = node as HeadingNode;
            if (headingNode.HeadingLevel < level)
            {
                UngetNode(node);
                return null;
            }

            if (headingNode.HeadingLevel != level)
            {
                throw new HelpSchemaException(headingNode.SourceExtent, "Expect Heading level " + level);
            }
            return headingNode;
        }

        /// <summary>
        /// </summary>
        /// <returns>
        /// return paragraphNode if encounterd.
        /// null, if any header level encountered.
        /// throw exception, if other unexpected node encountered.
        /// </returns>
        private ParagraphNode ParagraphNodeRule()
        {
            var node = GetNextNode();
            if (node == null)
            {
                return null;
            }

            switch (node.NodeType)
            {
                case MarkdownNodeType.Paragraph:
                    break;
                case MarkdownNodeType.Heading:
                    UngetNode(node);
                    return null;
                default:
                    throw new HelpSchemaException(GetExtent(node), "Expect Paragraph");
            }

            return node as ParagraphNode;
        }

        /// <summary>
        /// </summary>
        /// <returns>
        /// return paragraphNode if encounterd.
        /// null, if any header level encountered.
        /// throw exception, if other unexpected node encountered.
        /// </returns>
        private CodeBlockNode CodeBlockRule()
        {
            var node = GetNextNode();
            if (node == null)
            {
                return null;
            }

            switch (node.NodeType)
            {
                case MarkdownNodeType.CodeBlock:
                    break;
                case MarkdownNodeType.Heading:
                    UngetNode(node);
                    return null;
                default:
                    throw new HelpSchemaException(GetExtent(node), "Expect CodeBlock");
            }

            return node as CodeBlockNode;
        }

        private string GetTextFromParagraphSpans(IEnumerable<ParagraphSpan> spans)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var paragraphSpan in spans)
            {
                // TODO: make it handle hyperlinks, codesnippets, etc 
                sb.Append(paragraphSpan.Text);
            }
            return sb.ToString();
        }

        private string GetTextFromParagraphNode(ParagraphNode node)
        {
            if (node == null)
            {
                return "";
            }
            return GetTextFromParagraphSpans(node.Spans);
        }

        private void GatherParameterDetails(MamlCommand command)
        {
            const string parameterFormatString = @"
        {0}
        ${1}";

            const string docFunctionFormatString = @"
function Get-AttributeDocFunction
{{
    param(
        {0}
    )

}}

$h = Get-Help Get-AttributeDocFunction
$h.parameters.parameter
";

            // Create the Runspace on demand
            if (this.runspace == null)
            {
                this.runspace = RunspaceFactory.CreateRunspace();
                this.runspace.Open();
            }

            using (PowerShell powerShell = PowerShell.Create())
            {
                var parameterBlocks =
                    command
                        .Parameters
                        .Select(p => string.Format(parameterFormatString, p.AttributesText, p.Name));

                var functionScript =
                    string.Format(
                        docFunctionFormatString,
                        string.Join(",\r\n", parameterBlocks));

                // TODO: There could be some security concerns with executing arbitrary
                // text here, need to investigate safer ways to do it.  JEA?
                powerShell.Runspace = this.runspace;
                powerShell.AddScript(functionScript);
                var executionResults = powerShell.Invoke<PSObject>();
                if (powerShell.Streams.Error.Any())
                {
                    throw new HelpSchemaException(command.Extent, "Errors when processing command " + command.Name + ":\n" + string.Join(";\n", powerShell.Streams.Error));    
                }

                foreach (var parameterDetails in executionResults)
                {
                    var parameter = 
                        command.Parameters.FirstOrDefault(
                            p => string.Equals(p.Name, (string)parameterDetails.Properties["name"].Value));

                    // TODO: What about if null?

                    // parameter.Type = (string)((PSObject)parameterDetails.Properties["type"].Value).Properties["name"].Value;
                    
                    parameter.Position = (string)parameterDetails.Properties["position"].Value;
                    parameter.Required = ((string)parameterDetails.Properties["required"].Value).Equals("true");
                    parameter.PipelineInput = ((string)parameterDetails.Properties["pipelineInput"].Value).StartsWith("true");

                    // TODO: Still need to determine how to get these
                    //parameter.Globbing = false;
                    //parameter.ValueRequired = false;
                    //parameter.ValueVariableLength = false;
                    //parameter.VariableLength = false;

                    // The 'aliases' property will contain either 'None' or a
                    // comma-separated list of aliases.
                    string aliasesString = ((string)parameterDetails.Properties["aliases"].Value);
                    if (!string.Equals(aliasesString, "None"))
                    {
                        parameter.Aliases = 
                            aliasesString.Split(
                                new string[] { ", " }, 
                                StringSplitOptions.RemoveEmptyEntries);
                    }
                }

                powerShell.Commands.Clear();
                powerShell.Commands.AddScript("$h.Syntax.syntaxItem");
                foreach (var syntaxDetails in powerShell.Invoke<PSObject>())
                {
                    MamlSyntax syntax = new MamlSyntax();

                    var syntaxParams = (object[])syntaxDetails.Properties["parameter"].Value;
                    foreach (var syntaxParam in syntaxParams.OfType<PSObject>())
                    {
                        syntax.Parameters.Add(
                            command.Parameters.FirstOrDefault(
                                p => string.Equals(
                                    p.Name,
                                    (string)syntaxParam.Properties["name"].Value)));
                    }

                    command.Syntax.Add(syntax);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="command"></param>
        /// <returns>true if Parameter was found</returns>
        private bool ParameterRule(MamlCommand command)
        {
            // grammar:
            // #### Name [TypeName]
            // ```powershell
            // [Parameter(...)]
            // ```
            // Description
            var node = GetNextNode();
            var headingNode = GetHeadingWithExpectedLevel(node, PARAMETER_NAME_HEADING_LEVEL);
            if (headingNode == null)
            {
                return false;
            }

            var name = headingNode.Text.Split()[0];
            
            MamlParameter parameter = new MamlParameter()
            {
                Name = name,
                Extent = headingNode.SourceExtent
            };

            int typeBeginIndex = headingNode.Text.IndexOf('[');
            int typeEndIndex = headingNode.Text.IndexOf(']');
            if (typeBeginIndex > 0 && typeEndIndex > 0)
            {
                parameter.Type = headingNode.Text.Substring(typeBeginIndex + 1, typeEndIndex - typeBeginIndex - 1);
            }

            node = GetNextNode();

            ParagraphNode descriptionNode = null;
            CodeBlockNode attributesNode = null;

            switch (node.NodeType)
            {
                case MarkdownNodeType.Unknown:
                    break;
                case MarkdownNodeType.Document:
                    break;
                case MarkdownNodeType.Paragraph:
                    descriptionNode = node as ParagraphNode;
                    break;
                case MarkdownNodeType.Heading:
                    break;
                case MarkdownNodeType.CodeBlock:
                    attributesNode = node as CodeBlockNode;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            if (descriptionNode == null)
            {
                descriptionNode = ParagraphNodeRule();
            }

            parameter.Description = GetTextFromParagraphNode(descriptionNode);
            parameter.AttributesText = 
                attributesNode != null ?
                    attributesNode.Text : string.Empty;

            command.Parameters.Add(parameter);
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="command"></param>
        /// <returns>true if Section was found</returns>
        private bool SectionDispatch(MamlCommand command)
        {
            var node = GetNextNode();
            var headingNode = GetHeadingWithExpectedLevel(node, COMMAND_ENTRIES_HEADING_LEVEL);
            if (headingNode == null)
            {
                return false;
            }

            switch (headingNode.Text.ToUpper())
            {
                case "DESCRIPTION":
                    {
                        command.Description = SimpleTextSectionRule();
                        break;
                    }
                case "SYNOPSIS":
                    {
                        command.Synopsis = SimpleTextSectionRule();
                        break;
                    }
                case "PARAMETERS":
                    {
                        ParametersRule(command);
                        break;
                    }
                case "INPUTS":
                    {
                        InputsRule(command);
                        break;
                    }
                case "OUTPUTS":
                    {
                        OutputsRule(command);
                        break;
                    }
                case "NOTES":
                    {
                        command.Notes = SimpleTextSectionRule();
                        break;
                    }
                case "EXAMPLES":
                    {
                        ExamplesRule(command);
                        break;
                    }
                case "RELATED LINKS":
                    {
                        RelatedLinksRule(command);
                        break;
                    }
                default:
                    {
                        throw new HelpSchemaException(headingNode.SourceExtent, "Unexpected header name " + headingNode.Text);
                    }
            }
            return true;
        }

        public IEnumerable<MamlCommand> NodeModelToMamlModel(DocumentNode node)
        {
            _root = node;
            if (_root.Children == null)
            {
                // HACK:
                _rootEnumerator = (new LinkedList<MarkdownNode>()).GetEnumerator();
            }
            else
            {
                _rootEnumerator = _root.Children.GetEnumerator();
            }

            List<MamlCommand> commands = new List<MamlCommand>();
            MarkdownNode markdownNode;
            while ((markdownNode = GetNextNode()) != null)
            {
                if (markdownNode is HeadingNode)
                {
                    var headingNode = markdownNode as HeadingNode;
                    switch (headingNode.HeadingLevel)
                    {
                        case COMMAND_NAME_HEADING_LEVEL:
                            {
                                MamlCommand command = new MamlCommand()
                                {
                                    Name = headingNode.Text,
                                    Extent = headingNode.SourceExtent
                                };
                                // fill up command 
                                while (SectionDispatch(command)) { }

                                commands.Add(command);
                                break;
                            }
                        default: throw new HelpSchemaException(headingNode.SourceExtent, "Booo, I don't know what is the heading level " + headingNode.HeadingLevel);
                    }
                }
            }
            return commands;
        }
    }
}
