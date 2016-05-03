using Markdown.MAML.Model.MAML;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Markdown.MAML.Transformer
{
    public class ModelTransformerVersion2 : ModelTransformerBase
    {
        public ModelTransformerVersion2() : this(null) { }

        /// <summary>
        /// </summary>
        /// <param name="infoCallback">Report string information to some channel</param>
        public ModelTransformerVersion2(Action<string> infoCallback) : base(infoCallback) { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="command"></param>
        /// <returns>true if Section was found</returns>
        override protected bool SectionDispatch(MamlCommand command)
        {
            var node = GetNextNode();
            var headingNode = GetHeadingWithExpectedLevel(node, COMMAND_ENTRIES_HEADING_LEVEL);
            if (headingNode == null)
            {
                return false;
            }

            // TODO: When we are going to implement Localization story, we would need to replace
            // this strings by MarkdownStrings values.
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
                case "SYNTAX":
                    {
                        SyntaxRule(command);
                        break;
                    }
                case "EXAMPLES":
                    {
                        ExamplesRule(command);
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

        protected void SyntaxRule(MamlCommand commmand)
        {
            MamlSyntax syntax;
            while ((syntax = SyntaxEntryRule()) != null)
            {
                // We ignore Syntax section from markdown in the transformation
                // We get all nessesary info from parameters section
            }
        }

        protected MamlSyntax SyntaxEntryRule()
        {
            // grammar:
            // ### ParameterSetName 
            // ```
            // code
            // ```
            var node = GetNextNode();
            var headingNode = GetHeadingWithExpectedLevel(node, PARAMETERSET_NAME_HEADING_LEVEL);
            if (headingNode == null)
            {
                return null;
            }

            MamlSyntax syntax = new MamlSyntax()
            {
                ParameterSetName = headingNode.Text
            };

            var codeBlock = CodeBlockRule();
            // we don't use the output of it
            // TODO: we should capture syntax and verify that it's complient.
            return syntax;
        }

        protected void ParametersRule(MamlCommand commmand)
        {
            while (ParameterRule(commmand))
            {
            }
        }

        protected bool ParameterRule(MamlCommand commmand)
        {
            // TODO: implement
            return false;
        }
    }
}
