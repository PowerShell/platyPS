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
        private Action<string> _infoCallback;

        private const int COMMAND_NAME_HEADING_LEVEL = 1;
        private const int COMMAND_ENTRIES_HEADING_LEVEL = 2;
        private const int PARAMETER_NAME_HEADING_LEVEL = 3;
        private const int INPUT_OUTPUT_TYPENAME_HEADING_LEVEL = 3;
        private const int EXAMPLE_HEADING_LEVEL = 3;
        
        public ModelTransformer() : this(null) {}

        /// <summary>
        /// </summary>
        /// <param name="infoCallback">Report string information to some channel</param>
        public ModelTransformer(Action<string> infoCallback)
        {
            _infoCallback = infoCallback;
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

                                if (_infoCallback != null)
                                {
                                    Console.WriteLine("Start processing command " + command.Name);
                                }

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
            if (paragraphNode == null)
            {
                return;
            }

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
            bool first = true;
            bool previousIsHyperLink = false;
            foreach (var paragraphSpan in spans)
            {
                // TODO: make it handle hyperlinks, codesnippets, etc more wisely
                
                if (!first && paragraphSpan is HyperlinkSpan)
                {
                    sb.Append(" ");
                }
                else if (previousIsHyperLink)
                {
                    sb.Append(" ");
                }
                
                sb.Append(paragraphSpan.Text);

                first = false;
                previousIsHyperLink = paragraphSpan is HyperlinkSpan;
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

        /// <summary>
        /// We do this shift to avoid collisions with default parameter names, like informationVariable.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private string ShiftName(string name)
        {
            return name + "__";
        }

        /// <summary>
        /// We do this shift to avoid collisions with default parameter names, like informationVariable.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private string UndoShiftName(string name)
        {
            return name.Substring(0, name.Length-2);
        }

        /// <summary>
        /// PowerShell doesn't provide an easy way to retrive parameterSetName from syntaxItem, so we do this quirky heuristic.
        /// </summary>
        /// <param name="syntaxItem"></param>
        /// <returns></returns>
        private string GetParameterSetNameFromSyntaxItem(PSObject syntaxItem)
        {
            var syntaxParams = (object[])syntaxItem.Properties["parameter"].Value;
            Dictionary<string, int> counts = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            int paramCount = 0;
            foreach (PSObject syntaxParamPsObject in syntaxParams.OfType<PSObject>())
            {
                var parameterSetString = (string) syntaxParamPsObject.Properties["parameterSetName"].Value;
                string[] sets = parameterSetString.Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries);
                if (sets.Length > 0 && !string.Equals(parameterSetString, "(All)", StringComparison.OrdinalIgnoreCase))
                {
                    // Count only parameters with explicit parameter sets.
                    paramCount++;

                    foreach (string setName in sets)
                    {
                        int oldCount = counts.ContainsKey(setName) ? counts[setName] : 0;
                        counts[setName] = oldCount + 1;
                    }
                }
            }

            foreach (var countPair in counts)
            {
                if (countPair.Value == paramCount)
                {
                    return countPair.Key;
                }
            }

            return "";
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

# $h = Get-Help Get-AttributeDocFunction
{1}

$h.parameters.parameter
";

            // this is a workaround for https://github.com/PowerShell/platyPS/issues/27
            const string getHelpString = @"
$isAdmin = (New-Object Security.Principal.WindowsPrincipal ([Security.Principal.WindowsIdentity]::GetCurrent())).IsInRole([Security.Principal.WindowsBuiltinRole]::Administrator)  
$prev = Get-ItemProperty -Name DisablePromptToUpdateHelp -path 'HKLM:\SOFTWARE\Microsoft\PowerShell' -ErrorAction SilentlyContinue
try
{
    if ($isAdmin)
    {
        Set-ItemProperty -Name DisablePromptToUpdateHelp -Value 1 -path 'HKLM:\SOFTWARE\Microsoft\PowerShell' 
    }
    # this is an importent line that populates object
    $h = Get-Help Get-AttributeDocFunction
    }
    finally
    {
    if ($isAdmin)
    {
        if ($prev)
        {
            if ($prev.DisablePromptToUpdateHelp -ne 1) { Set-ItemProperty -Name DisablePromptToUpdateHelp -Value ($prev.DisablePromptToUpdateHelp) -path 'HKLM:\SOFTWARE\Microsoft\PowerShell' }
        }
        else
        {
            Remove-ItemProperty -Name DisablePromptToUpdateHelp -path 'HKLM:\SOFTWARE\Microsoft\PowerShell' 
        }
    }
}
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
                        .Select(p => string.Format(parameterFormatString, p.AttributesText, ShiftName(p.Name)));

                var functionScript =
                    string.Format(
                        docFunctionFormatString,
                        string.Join(",\r\n", parameterBlocks),
                        getHelpString);

                // TODO: There could be some security concerns with executing arbitrary
                // text here, need to investigate safer ways to do it.  JEA?
                powerShell.Runspace = this.runspace;
                powerShell.AddScript(functionScript);
                var parameterDetailses = powerShell.Invoke<PSObject>();
                if (powerShell.Streams.Error.Any())
                {
                    throw new HelpSchemaException(command.Extent, "Errors when processing command " + command.Name + ":\n" + string.Join(";\n", powerShell.Streams.Error));    
                }

                foreach (PSObject parameterDetailsPsObject in parameterDetailses)
                {
                    var parameter = 
                        command.Parameters.FirstOrDefault(
                            p => string.Equals(p.Name, UndoShiftName((string)parameterDetailsPsObject.Properties["name"].Value)));

                    FillUpParameterFromPSObject(parameter, parameterDetailsPsObject);
                }

                powerShell.Commands.Clear();
                powerShell.Commands.AddScript("$h.Syntax.syntaxItem");
                var syntaxDetailses = powerShell.Invoke<PSObject>();
                if (powerShell.Streams.Error.Any())
                {
                    throw new HelpSchemaException(command.Extent, "Errors when processing command " + command.Name + ":\n" + string.Join(";\n", powerShell.Streams.Error));
                }

                var sortedSyntaxItems = syntaxDetailses.ToList();
                sortedSyntaxItems.Sort((si1, si2) => String.CompareOrdinal(GetParameterSetNameFromSyntaxItem(si1), GetParameterSetNameFromSyntaxItem(si2)));

                foreach (var syntaxDetails in sortedSyntaxItems)
                {
                    MamlSyntax syntax = new MamlSyntax();

                    var syntaxParams = (object[])syntaxDetails.Properties["parameter"].Value;
                    foreach (PSObject syntaxParamPsObject in syntaxParams.OfType<PSObject>())
                    {
                        string paramName = UndoShiftName((string) syntaxParamPsObject.Properties["name"].Value);
                        MamlParameter parametersParameter = command.Parameters.FirstOrDefault(p => string.Equals(p.Name, paramName));
                        if (parametersParameter == null)
                        {
                            throw new HelpSchemaException(command.Extent, "Cannot find corresponding parameter for syntax item " + paramName);
                        }

                        MamlParameter syntaxParameter = new MamlParameter()
                        {
                            Name = parametersParameter.Name,
                            Type = parametersParameter.Type
                        };

                        FillUpParameterFromPSObject(syntaxParameter, syntaxParamPsObject);
                        syntax.Parameters.Add(syntaxParameter);
                    }

                    command.Syntax.Add(syntax);
                }
            }
        }

        private static void FillUpParameterFromPSObject(MamlParameter parameter, PSObject parameterDetails)
        {
            // TODO: What about if null?

            // parameter.Type = (string)((PSObject)parameterDetails.Properties["type"].Value).Properties["name"].Value;

            parameter.Position = (string) parameterDetails.Properties["position"].Value;
            parameter.Required = ((string) parameterDetails.Properties["required"].Value).Equals("true");
            string pipelineInput = (string) parameterDetails.Properties["pipelineInput"].Value;
            if (pipelineInput.StartsWith("t"))
            {
                // for some reason convention is:
                // false
                // True (ByValue)
                // True (ByPropertyName)
                pipelineInput = 'T' + pipelineInput.Substring(1);
            }

            parameter.PipelineInput = pipelineInput;

            // TODO: Still need to determine how to get these
            //parameter.VariableLength = ((string)parameterDetails.Properties["variableLength"].Value).Equals("true");
            //parameter.ValueVariableLength = false;

            // it turns to work very well on all real-world examples, but in theory it can be orbitrary
            parameter.ValueRequired = parameter.Type == "switch" ? false : true;

            var parameterValueGroup = parameterDetails.Properties["parameterValueGroup"];
            if (parameterValueGroup != null)
            {
                var validateSet = (parameterValueGroup.Value as PSObject).Properties["parameterValue"].Value as object[];
                parameter.ParameterValueGroup.AddRange(validateSet.Select(x => x.ToString()));
            }
            
            // $h.Syntax.syntaxItem[0].parameter[0].parameterValueGroup.parameterValue

            // The 'aliases' property will contain either 'None' or a
            // comma-separated list of aliases.
            string aliasesString = ((string) parameterDetails.Properties["aliases"].Value);
            if (!string.Equals(aliasesString, "None"))
            {
                parameter.Aliases =
                    aliasesString.Split(
                        new string[] {", "},
                        StringSplitOptions.RemoveEmptyEntries);
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
            // #### Name [TypeName]     -   mandatory
            // ```powershell            -   optional
            // [Parameter(...)]
            // ```
            // Description              -   optional
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

            int equalIndex = headingNode.Text.IndexOf('=');
            string headingNodeText;
            if (equalIndex >= 0)
            {
                parameter.DefaultValue = headingNode.Text.Substring(equalIndex + 1).Trim();
                // trim it for this case from PSReadLine:
                // #### WordDelimiters [Int32] = ;:,.[]{}()/\|^&*-=+
                // We need to make sure that closing ] corresponds to [Int32], so it's the last ] before first = sign.
                headingNodeText = headingNode.Text.Substring(0, equalIndex);
            }
            else
            {
                headingNodeText = headingNode.Text;
            }

            int typeBeginIndex = headingNodeText.IndexOf('[');
            int typeEndIndex = headingNodeText.LastIndexOf(']');
            if (typeBeginIndex >= 0 && typeEndIndex > 0)
            {
                parameter.Type = headingNodeText.Substring(typeBeginIndex + 1, typeEndIndex - typeBeginIndex - 1);
            }

            node = GetNextNode();

            ParagraphNode descriptionNode = null;
            CodeBlockNode attributesNode = null;

            // it can be the end
            if (node != null)
            {
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
                        // next parameter started
                        UngetNode(node);
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
            }

            parameter.Description = GetTextFromParagraphNode(descriptionNode);
            parameter.AttributesText = 
                attributesNode != null ?
                    attributesNode.Text : string.Empty;

            if (parameter.AttributesText.Contains(@"[SupportsWildCards()]"))
            {
                parameter.Globbing = true;
            }

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
    }
}
