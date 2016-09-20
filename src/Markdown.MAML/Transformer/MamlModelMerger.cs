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
            MamlCommand result = null;

            Report($"---- UPDATING Cmdlet : {metadataModel.Name} ----");
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
            
            //report name changes to syntax objects
            Report("Syntax Block and Parameter Set Names reflected and document syntax fully updated.\r\n\r\n");

            var metadataSyntaxSet = new SortedSet<MamlSyntax>(metadataModel.Syntax, new MamlSyntaxComparer());
            var stringSyntaxSet = new SortedSet<MamlSyntax>(stringModel.Syntax, new MamlSyntaxComparer());
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

                bool foundSyntaxMatch = false;

                foreach (var stringSyntax in stringModel.Syntax)
                {
                    var stringSyntaxParameters = stringSyntax.Parameters.Select(s => s.Name).ToList();
                    stringSyntaxParameters.Sort();

                    if (reflectedSyntaxParameters.SequenceEqual(stringSyntaxParameters))
                    {
                        Report($"Parameter Set Name Updated:\r\nOld Set:{stringSyntax.ParameterSetName}\r\nNew Set:{reflectedSyntax.ParameterSetName}\r\n\r\n");
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
                        "parameter description");
                    
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
