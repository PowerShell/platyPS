using Markdown.MAML.Model.MAML;
using System;
using System.Collections.Generic;
using System.Linq;
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

            Report("----Cmdlet {0} is updating.----", metadataModel.Name);

            MamlCommand result = new MamlCommand()
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
            
            Report("----Cmdlet {0} updated.----\r\n\r\n", result.Name);

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
            
            //report name changes to syntax objects
            Report("::Syntax Block and Parameter Set Names fully replaced by Cmdlet Reflection");

            //reports changes to parameter set names
            foreach (var reflectedSyntax in metadataModel.Syntax)
            {
                var reflectedSyntaxParameters = reflectedSyntax.Parameters.Select(s => s.Name).ToList();
                reflectedSyntaxParameters.Sort();

                bool foundSyntaxMatch = false;

                foreach (var stringSyntax in stringModel.Syntax)
                {
                    var stringSyntaxParameters = stringSyntax.Parameters.Select(s => s.Name).ToList();
                    stringSyntaxParameters.Sort();

                    if (reflectedSyntaxParameters.SequenceEqual(stringSyntaxParameters))
                    {
                        Report("::ParameterSet Name: <{0}> replaced [{1}]",reflectedSyntax.ParameterSetName,stringSyntax.ParameterSetName);
                        foundSyntaxMatch = true;
                    }
                }

                if (!foundSyntaxMatch)
                {
                    Report("::ParameterSet Name no match found: <{0}> added", reflectedSyntax.ParameterSetName);
                }
                
                if (reflectedSyntax.IsDefault)
                {
                    Report("::{0} has been set as the default Parameter Set for {1}.\r\n",reflectedSyntax.ParameterSetName,metadataModel.Name);
                }
            }

            foreach (var param in metadataModel.Parameters)
            {
                var aliases = param.Aliases.Aggregate(string.Empty, (current, alias) => current + " " + alias);

                if (aliases != string.Empty)
                {
                    Report("::Aliases updated for {0}:{1}", param.Name, aliases);
                }

                var strParam = FindParameterByName(param.Name, stringModel.Parameters);
                if (strParam == null)
                {
                    Report("::{0}: parameter {1} cannot be found in the markdown file.", metadataModel.Name, param.Name);
                }
                else
                {
                    param.Description = metadataStringCompare(param.Description,
                        strParam.Description,
                        metadataModel.Name, 
                        param.Name, 
                        "description");
                    
                    param.DefaultValue = strParam.DefaultValue;
                    // don't update type
                    // param.Type = strParam.Type;
                    param.Extent = strParam.Extent;
                    
                }
                
                result.Parameters.Add(param);
            }

            foreach(var param in stringModel.Parameters)
            {
                if (FindParameterByName(param.Name, metadataModel.Parameters) == null)
                {
                    Report("::{0}: parameter {1} is not longer present.", metadataModel.Name, param.Name);
                }
            }
        }

        /// <summary>
        /// Compares parameters
        /// </summary>
        private string metadataStringCompare(string metadataContent, string stringContent, string moduleName, string paramName, string contentItemName)
        {
            if(!StringComparer.Ordinal.Equals((stringContent == null ? "" : Pretify(stringContent)),
                (metadataContent == null ? "" : Pretify(metadataContent))))
            {
                Report("::{0}: parameter {1} - {2} has been updated:\r\n<Old from MAML\r\n    {4}\r\n>\r\n\r\n[New from Markdown\r\n    {3}\r\n]", 
                    moduleName, 
                    paramName, 
                    contentItemName,
                    stringContent == null ? "" : Pretify(stringContent).TrimEnd(' '),
                    metadataContent == null ? "" : Pretify(metadataContent).TrimEnd(' '));
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
        /// Compares Cmdlet values
        /// </summary>
        private string metadataStringCompare(string metadataContent, string stringContent, string moduleName, string contentItemName)
        {
            var metadataContentPretified = (metadataContent == null ? "" : Pretify(metadataContent).TrimEnd(' '));
            var stringContentPretified = (stringContent == null ? "" : Pretify(stringContent).TrimEnd(' '));

            if (!StringComparer.Ordinal.Equals(metadataContentPretified, stringContentPretified))
            {
                Report("::{0}: {1} has been updated:\r\n<Old from MAML\r\n    {3}\r\n>\r\n\r\n[New from Markdown\r\n    {2}\r\n]\r\n",
                    moduleName,
                    contentItemName,
                    stringContentPretified,
                    metadataContentPretified);
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
