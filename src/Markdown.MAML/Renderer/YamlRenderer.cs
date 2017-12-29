using Markdown.MAML.Model.MAML;
using Markdown.MAML.Model.YAML;
using System.IO;
using System.Linq;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Markdown.MAML.Renderer
{
    public class YamlRenderer
    {
        public static string MamlModelToString(MamlCommand mamlCommand)
        {
            var serializer = new SerializerBuilder()
                                .WithNamingConvention(new CamelCaseNamingConvention())
                                .Build();

            var model = new YamlCommand
            {
                Name = mamlCommand.Name,
                Notes = mamlCommand.Notes.Text,
                Remarks = mamlCommand.Description.Text,
                Summary = mamlCommand.Synopsis.Text,
                Examples = mamlCommand.Examples.Select(CreateExample).ToList(),
                Inputs = mamlCommand.Inputs.Select(CreateInputOutput).ToList(),
                Links = mamlCommand.Links.Select(CreateLink).ToList(),
                Module = new YamlModule { Name = mamlCommand.ModuleName },
                OptionalParameters = mamlCommand.Parameters.Where(p => !p.Required).Select(CreateParameter).ToList(),
                Outputs = mamlCommand.Outputs.Select(CreateInputOutput).ToList(),
                RequiredParameters = mamlCommand.Parameters.Where(p => p.Required).Select(CreateParameter).ToList(),
                Syntaxes = mamlCommand.Syntax.Select(CreateSyntax).ToList()
            };

            using (var writer = new StringWriter())
            {
                serializer.Serialize(writer, model);
                writer.Flush();
                return writer.ToString();
            }
        }

        private static YamlExample CreateExample(MamlExample mamlExample)
        {
            return new YamlExample
            {
                Name = mamlExample.Title,
                PreCode = mamlExample.Introduction,
                Code = string.Join("\r\n\r\n", mamlExample.Code.Select(block => block.Text)),
                PostCode = mamlExample.Remarks
            };
        }

        private static YamlInputOutput CreateInputOutput(MamlInputOutput mamlInputOutput)
        {
            return new YamlInputOutput
            {
                Description = mamlInputOutput.Description,
                Type = mamlInputOutput.TypeName
            };
        }

        private static YamlLink CreateLink(MamlLink mamlLink)
        {
            return new YamlLink
            {
                Href = mamlLink.LinkUri,
                Text = mamlLink.LinkName
            };
        }

        private static YamlParameter CreateParameter(MamlParameter mamlParameter)
        {
            return new YamlParameter
            {
                Name = mamlParameter.Name,
                AcceptWildcardCharacters = mamlParameter.Globbing,
                Description = mamlParameter.Description,
                Aliases = mamlParameter.Aliases.ToList(),
                Type = mamlParameter.Type,
                ParameterValueGroup = mamlParameter.ParameterValueGroup,
                Position = mamlParameter.Position,
                PipelineInput = mamlParameter.PipelineInput,
                DefaultValue = mamlParameter.DefaultValue
            };
        }

        private static YamlSyntax CreateSyntax(MamlSyntax syntax)
        {
            return new YamlSyntax
            {
                IsDefault = syntax.IsDefault,
                ParameterValueGroup = syntax.ParameterSetName,
                Parameters = syntax.Parameters.Select(p => p.Name).ToList()
            };
        }
    }
}
