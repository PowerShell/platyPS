using Markdown.MAML.Model.MAML;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Markdown.MAML.Model.YAML;
using Markdown.MAML.Renderer;
using Xunit;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
using Markdown.MAML.Model.Markdown;

namespace Markdown.MAML.Test.Renderer
{
    public class YamlRendererTests
    {
        [Fact]
        public void RendererProducesNameAndTextFields()
        {
            var model = CreateModel();

            var output = YamlRenderer.MamlModelToString(model);

            var deserializer = CreateDeserializer();

            var writtenModel = deserializer.Deserialize<YamlCommand>(output);

            Assert.NotNull(writtenModel);
            Assert.Equal(model.Name, writtenModel.Name);
            Assert.Equal(model.Description.Text, writtenModel.Remarks);
            Assert.Equal(model.Synopsis.Text, writtenModel.Summary);
            Assert.Equal(model.Notes.Text, writtenModel.Notes);
        }

        [Fact]
        public void RenderProducesExamples()
        {
            var model = CreateModel();

            var output = YamlRenderer.MamlModelToString(model);

            var deserializer = CreateDeserializer();

            var writtenModel = deserializer.Deserialize<YamlCommand>(output);

            Assert.Single(writtenModel.Examples);

            var example = writtenModel.Examples.Single();

            Assert.Equal(model.Examples.Single().Introduction, example.PreCode);
            Assert.Equal(model.Examples.Single().Code.Single().Text, example.Code);
            Assert.Equal(model.Examples.Single().Remarks, example.PostCode);
            Assert.Equal(model.Examples.Single().Title, example.Name);
        }

        [Fact]
        public void RenderProducesInputs()
        {
            var model = CreateModel();

            var output = YamlRenderer.MamlModelToString(model);

            var deserializer = CreateDeserializer();

            var writtenModel = deserializer.Deserialize<YamlCommand>(output);

            Assert.Single(writtenModel.Inputs);

            var input = writtenModel.Inputs.Single();

            Assert.Equal(model.Inputs.Single().Description, input.Description);
            Assert.Equal(model.Inputs.Single().TypeName, input.Type);
        }

        [Fact]
        public void RenderProducesLinks()
        {
            var model = CreateModel();

            var output = YamlRenderer.MamlModelToString(model);

            var deserializer = CreateDeserializer();

            var writtenModel = deserializer.Deserialize<YamlCommand>(output);

            Assert.Single(writtenModel.Links);

            var link = writtenModel.Links.Single();

            Assert.Equal(model.Links.Single().LinkName, link.Text);
            Assert.Equal(model.Links.Single().LinkUri, link.Href);
        }

        [Fact]
        public void RenderProducesModule()
        {
            var model = CreateModel();

            var output = YamlRenderer.MamlModelToString(model);

            var deserializer = CreateDeserializer();

            var writtenModel = deserializer.Deserialize<YamlCommand>(output);

            Assert.NotNull(writtenModel.Module);

            Assert.Equal(model.ModuleName, writtenModel.Module.Name);
        }

        [Fact]
        public void RenderProducesOptionalParameters()
        {
            var model = CreateModel();

            var output = YamlRenderer.MamlModelToString(model);

            var deserializer = CreateDeserializer();

            var writtenModel = deserializer.Deserialize<YamlCommand>(output);

            Assert.Single(writtenModel.OptionalParameters);

            var optionalParameter = writtenModel.OptionalParameters.Single();
            var expectedParameter = model.Parameters.Single(p => !p.Required);

            Assert.Equal(expectedParameter.Globbing, optionalParameter.AcceptWildcardCharacters);
            Assert.Equal(expectedParameter.Aliases, optionalParameter.Aliases);
            Assert.Equal(expectedParameter.DefaultValue, optionalParameter.DefaultValue);
            Assert.Equal(expectedParameter.Description, optionalParameter.Description);
            Assert.Equal(expectedParameter.Name, optionalParameter.Name);
            Assert.Equal(expectedParameter.ParameterValueGroup, optionalParameter.ParameterValueGroup);
            Assert.Equal(expectedParameter.PipelineInput, optionalParameter.PipelineInput);
            Assert.Equal(expectedParameter.Position, optionalParameter.Position);
            Assert.Equal(expectedParameter.Type, optionalParameter.Type);
        }

        [Fact]
        public void RenderProducesOutputs()
        {
            var model = CreateModel();

            var output = YamlRenderer.MamlModelToString(model);

            var deserializer = CreateDeserializer();

            var writtenModel = deserializer.Deserialize<YamlCommand>(output);

            Assert.Single(writtenModel.Outputs);

            var outputItem = writtenModel.Outputs.Single();

            Assert.Equal(model.Outputs.Single().TypeName, outputItem.Type);
            Assert.Equal(model.Outputs.Single().Description, outputItem.Description);
        }

        [Fact]
        public void RenderProducesRequiredParameters()
        {
            var model = CreateModel();

            var output = YamlRenderer.MamlModelToString(model);

            var deserializer = CreateDeserializer();

            var writtenModel = deserializer.Deserialize<YamlCommand>(output);

            Assert.Single(writtenModel.RequiredParameters);

            var requiredParameter = writtenModel.RequiredParameters.Single();
            var expectedParameter = model.Parameters.Single(p => p.Required);

            Assert.Equal(expectedParameter.Globbing, requiredParameter.AcceptWildcardCharacters);
            Assert.Equal(expectedParameter.Aliases, requiredParameter.Aliases);
            Assert.Equal(expectedParameter.DefaultValue, requiredParameter.DefaultValue);
            Assert.Equal(expectedParameter.Description, requiredParameter.Description);
            Assert.Equal(expectedParameter.Name, requiredParameter.Name);
            Assert.Equal(expectedParameter.ParameterValueGroup, requiredParameter.ParameterValueGroup);
            Assert.Equal(expectedParameter.PipelineInput, requiredParameter.PipelineInput);
            Assert.Equal(expectedParameter.Position, requiredParameter.Position);
            Assert.Equal(expectedParameter.Type, requiredParameter.Type);
        }

        [Fact]
        public void RenderProducesSyntaxes()
        {
            var model = CreateModel();

            var output = YamlRenderer.MamlModelToString(model);

            var deserializer = CreateDeserializer();

            var writtenModel = deserializer.Deserialize<YamlCommand>(output);

            Assert.Single(writtenModel.Syntaxes);

            var syntax = writtenModel.Syntaxes.Single();

            Assert.Equal(model.Syntax.Single().ParameterSetName, syntax.ParameterValueGroup);
            
            Assert.Single(syntax.Parameters);
            Assert.Equal(model.Syntax.Single().Parameters.Single().Name, syntax.Parameters.Single());
        }

        private static MamlCommand CreateModel()
        {
            var command = new MamlCommand
            {
                Name = "Test-Unit",
                Description = new SectionBody("A test cmdlet"),
                Synopsis = new SectionBody("A cmdlet to test"),
                Notes = new SectionBody("This is just a test"),
                ModuleName = "TestModule"
            };

            command.Examples.Add(new MamlExample
            {
                Remarks = "Example 1 remarks",
                Code = new [] { new MamlCodeBlock("Example 1 code") },
                Introduction = "Example 1 intro",
                Title = "Example 1 title"
            });

            command.Inputs.Add(new MamlInputOutput
            {
                Description = "Input 1",
                TypeName = "System.String"
            });

            command.Links.Add(new MamlLink
            {
                LinkUri = "https://docs.microsoft.com",
                LinkName = "Docs"
            });

            var mamlOptionalParameter = new MamlParameter
            {
                Required = false,
                Name = "-TestParam",
                Globbing = true,
                Aliases = new [] {"-Alias1"},
                DefaultValue = "Test",
                Description = "Test Parameter",
                PipelineInput = "false",
                Position = "1",
                Type = "System.String"
            };
            mamlOptionalParameter.ParameterValueGroup.Add("TestGroup");

            var mamlRequiredParameter = new MamlParameter
            {
                Required = true,
                Name = "-TestParam2",
                Globbing = false,
                Aliases = new[] { "-Alias2" },
                DefaultValue = "Test2",
                Description = "Test Parameter 2",
                PipelineInput = "true",
                Position = "2",
                Type = "System.Boolean"
            };
            mamlRequiredParameter.ParameterValueGroup.Add("TestGroup2");

            command.Parameters.AddRange(new [] {mamlRequiredParameter, mamlOptionalParameter});

            command.Outputs.Add(new MamlInputOutput
            {
                Description = "Output 1",
                TypeName = "System.String"
            });

            var mamlSyntax = new MamlSyntax
            {
                ParameterSetName = "TestGroup",
                IsDefault = true
            };
            mamlSyntax.Parameters.Add(mamlOptionalParameter);
            command.Syntax.Add(mamlSyntax);

            return command;
        }

        private static Deserializer CreateDeserializer()
        {
            return new DeserializerBuilder()
                            .WithNamingConvention(new CamelCaseNamingConvention())
                            .Build();
        }
    }
}
