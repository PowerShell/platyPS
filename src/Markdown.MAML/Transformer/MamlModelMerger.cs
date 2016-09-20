using Markdown.MAML.Model.MAML;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation.Language;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Markdown.MAML.Transformer
{
    public class MamlModelMerger
    {
        private Action<string> _infoCallback;

        public MamlModelMerger() : this(null) { }

        /// <summary>
        /// </summary>
        /// <param name="infoCallback">Report string information to some channel</param>
        public MamlModelMerger(Action<string> infoCallback)
        {
            _infoCallback = infoCallback;
        }

        public MamlCommand Merge(MamlCommand metadataModel, MamlCommand stringModel)
        {
            MamlCommand result = null;

            Report($"---- UPDATING Cmdlet : {metadataModel.Name} ----\r\n\r\n");
            try
            {
                result = new MamlCommand()
                {
                    Name = metadataModel.Name,
                    Synopsis = metadataStringCompare(metadataModel.Synopsis,
                            stringModel.Synopsis,
                            metadataModel.Name,
                            "synopsis"),
                    Description = metadataStringCompare(metadataModel.Description,
                            stringModel.Description,
                            metadataModel.Name,
                            "description"),
                    Notes = metadataStringCompare(metadataModel.Notes,
                            stringModel.Notes,
                            metadataModel.Name,
                            "notes"),
                    Extent = stringModel.Extent
                };

                // TODO: convert into MergeMetadataProperty
                result.Links.AddRange(stringModel.Links);

                // All examples come only from strtringModel
                result.Examples.AddRange(stringModel.Examples);

                // TODO: figure out what's the right thing for MamlInputOutput
                result.Inputs.AddRange(stringModel.Inputs);
                result.Outputs.AddRange(stringModel.Outputs);

                //Result takes in the merged parameter results.
                MergeParameters(result, metadataModel, stringModel);
            }
            catch (Exception ex)
            {
                Report($"---- ERROR UPDATING Cmdlet : {metadataModel.Name}----\r\n");
                Report($"Exception message: \r\n{ex.Message}\r\n");
            }
            Report($"---- COMPLETED UPDATING Cmdlet : {metadataModel.Name} ----\r\n\r\n");

            return result;
        }

        private MamlParameter FindParameterByName(string name, IEnumerable<MamlParameter> list)
        {
            return list.FirstOrDefault(x => StringComparer.OrdinalIgnoreCase.Equals(name, x.Name));
        }

        private void MergeParameters(MamlCommand result, MamlCommand metadataModel, MamlCommand stringModel)
        {
            // we care only about metadata for parameters in syntax
            result.Syntax.AddRange(metadataModel.Syntax);
            
            //report name changes to syntax objects (Parameter Sets)
            Report("Syntax Block and Parameter Set Names reflected and document syntax fully updated.\r\n\r\n");

            var metadataSyntaxSet = new SortedSet<MamlSyntax>(metadataModel.Syntax, new MamlSyntaxNameComparer());
            var stringSyntaxSet = new SortedSet<MamlSyntax>(stringModel.Syntax, new MamlSyntaxNameComparer());
            var removedSyntaxes = stringSyntaxSet.Except(metadataSyntaxSet).ToList();
            var addedSyntaxes = metadataSyntaxSet.Except(stringSyntaxSet).ToList();

            foreach (var addedSyntax in addedSyntaxes)
            {
                Report($"Parameter Set Added: {addedSyntax.ParameterSetName}\r\n\r\n");
            }
            foreach (var droppedSyntax in removedSyntaxes)
            {
                Report($"Parameter Set Deleted: {droppedSyntax.ParameterSetName}\r\n\r\n");
            }

            //reports changes to parameter set names
            foreach (var reflectedSyntax in metadataModel.Syntax)
            {
                var reflectedSyntaxParameters = reflectedSyntax.Parameters.Select(s => s.Name).ToList();
                reflectedSyntaxParameters.Sort();

                var foundSyntaxMatch = false;

                foreach (var stringSyntax in stringModel.Syntax)
                {
                    var stringSyntaxParameters = stringSyntax.Parameters.Select(s => s.Name).ToList();
                    stringSyntaxParameters.Sort();

                    if (reflectedSyntaxParameters.SequenceEqual(stringSyntaxParameters))
                    {
                        Report($"Parameter Set Name Updated:\r\nOld Set: {stringSyntax.ParameterSetName}\r\nNew Set: {reflectedSyntax.ParameterSetName}\r\n\r\n");
                        foundSyntaxMatch = true;
                    }
                }

                if (!foundSyntaxMatch)
                {
                    Report($"Parameter Set Added: {reflectedSyntax.ParameterSetName}\r\n\r\n");
                }
                
                if (reflectedSyntax.IsDefault)
                {
                    Report($"Default Parameter Set: {reflectedSyntax.ParameterSetName}\r\n\r\n");
                }
            }

            //Processing Parameters for cmdlet
            var stringParameterSet = new SortedSet<MamlParameter>(stringModel.Parameters, new MamlParameterNameComparer());
            var metadataParameterSet = new SortedSet<MamlParameter>(metadataModel.Parameters, new MamlParameterNameComparer());
            var addedParameters = metadataParameterSet.Except(stringParameterSet).ToList();
            var removedParameters = stringParameterSet.Except(metadataParameterSet).ToList();

            foreach (var addedParam in addedParameters)
            {
                Report($"Parameter Added: {addedParam.Name}\r\n\r\n");
            }
            foreach (var removedParam in removedParameters)
            {
                Report($"Parameter Deleted: {removedParam.Name}\r\n\r\n");
            }

            var matchedParametersSet = stringParameterSet.Intersect(metadataParameterSet,new MamlParameterEqualityComparer()).ToList();
            var matchedComparer = new MamlParameterAttributeComparer();

            foreach (var matchedParam in matchedParametersSet)
            {
                var metadataMatch = metadataModel.Parameters.Single(p => p.Name == matchedParam.Name);
                if (matchedComparer.Compare(matchedParam, metadataMatch) == 0)
                {
                    continue;
                }
                var metaDataAttributeString = assembleAttributeString(metadataMatch);
                var stringAttributeString = assembleAttributeString(matchedParam);
                Report($"Parameter Updated: {matchedParam.Name}\r\nUpdated from:\r\n{metaDataAttributeString}\r\nTo:\r\n{stringAttributeString}\r\n\r\n");
            }

            foreach (var param in metadataModel.Parameters)
            {
                var strParam = FindParameterByName(param.Name, stringModel.Parameters);
                if (strParam != null)
                {
                    param.Description = metadataStringCompare(param.Description,
                        strParam.Description,
                        metadataModel.Name, 
                        param.Name, 
                        "parameter description");
                    
                    param.DefaultValue = strParam.DefaultValue;
                    // don't update type
                    // param.Type = strParam.Type;
                    param.Extent = strParam.Extent;
                    
                }
                
                result.Parameters.Add(param);
            }
            
        }

        private string assembleAttributeString(MamlParameter parameter)
        {
            var attributeBlock = new StringBuilder();
            attributeBlock.AppendLine($"Type: {parameter.Type}");
            var aliasas = parameter.Aliases.Aggregate(string.Empty, (current, alias) => current + " " + alias);
            attributeBlock.AppendLine($"Aliases: {aliasas}");
            attributeBlock.AppendLine($"Required: {parameter.Required}");
            attributeBlock.AppendLine($"Position: {parameter.Position}");
            attributeBlock.AppendLine($"Default Value: {parameter.DefaultValue}");
            attributeBlock.AppendLine($"Accept pipeline input: {parameter.PipelineInput}");
            attributeBlock.AppendLine($"Accept wildcard characters: {parameter.Globbing}");

            return attributeBlock.ToString();
        }

        /// <summary>
        /// Compares parameters
        /// </summary>
        private string metadataStringCompare(string metadataContent, string stringContent, string moduleName, string paramName, string contentItemName)
        {

            var pretifiedStringContent = stringContent == null ? "" : Pretify(stringContent).TrimEnd(' ');
            var pretifiedMetadataContent = metadataContent == null ? "" : Pretify(metadataContent).TrimEnd(' ');

            if (!StringComparer.Ordinal.Equals((pretifiedStringContent),(pretifiedMetadataContent)))
            {
                Report($"Reflection found a new {contentItemName}, for parameter {paramName}. The original content has been preserved.\r\n\r\nPreserved:\r\n{pretifiedStringContent}\r\n\r\nOverridden:\r\n{pretifiedMetadataContent}\r\n\r\n");   
            }

            return stringContent;
        }

        /// <summary>
        /// Cleans the extra \r\n and inserts a tab at the beginning of new lines, mid paragraphs
        /// </summary>
        private static string Pretify(string multiLineText)
        {
            if(string.IsNullOrEmpty(multiLineText))
            {
                multiLineText = "";
            }
            return Regex.Replace(multiLineText, "(\r\n)+", "\r\n    ");
        }

        /// <summary>
        /// Compares Cmdlet values: Synopsis, Description, and Notes. Preserves the content from the string model (old) or the metadata model (new).
        /// </summary>
        private string metadataStringCompare(string metadataContent, string stringContent, string moduleName, string contentItemName)
        {
            var metadataContentPretified = (metadataContent == null ? "" : Pretify(metadataContent).TrimEnd(' '));
            var stringContentPretified = (stringContent == null ? "" : Pretify(stringContent).TrimEnd(' '));

            if (!StringComparer.Ordinal.Equals(metadataContentPretified, stringContentPretified))
            {
                Report($"Reflection found a new {contentItemName}. The original content has been preserved.\r\n\r\nPreserved:\r\n{stringContentPretified}\r\n\r\nOverridden:\r\n{metadataContentPretified}\r\n\r\n");
            }

            return stringContent;
        }

        private void Report(string format, params object[] objects)
        {
            if (_infoCallback != null)
            {
                _infoCallback.Invoke(string.Format(format, objects));
            }
        }
    }
}
