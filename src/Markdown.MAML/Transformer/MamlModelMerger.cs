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

        private bool _cmdletUpdated;

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
            _cmdletUpdated = false;

            Report($"---- UPDATING Cmdlet : {metadataModel.Name} ----\r\n");
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
                Report($"    Exception message: \r\n{ex.Message}\r\n");
                _cmdletUpdated = true;
            }

            if (!_cmdletUpdated)
            {
                Report("\tNo updates done\r\n");
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
            
            //reports changes to parameter set names
            var metadataSyntaxSet = new SortedSet<MamlSyntax>(metadataModel.Syntax, new MamlSyntaxNameComparer());
            var stringSyntaxSet = new SortedSet<MamlSyntax>(stringModel.Syntax, new MamlSyntaxNameComparer());
            var removedSyntaxes = stringSyntaxSet.Except(metadataSyntaxSet,new MamlParameterSetEqualityComparer()).ToList();
            var addedSyntaxes = metadataSyntaxSet.Except(stringSyntaxSet, new MamlParameterSetEqualityComparer()).ToList();

            foreach (var addedSyntax in addedSyntaxes)
            {
                Report($"\tParameter Set Added: {addedSyntax.ParameterSetName}\r\n");
                _cmdletUpdated = true;
            }
            foreach (var droppedSyntax in removedSyntaxes)
            {
                Report($"\tParameter Set Deleted: {droppedSyntax.ParameterSetName}\r\n");
                _cmdletUpdated = true;
            }

            foreach (var metadataSyntax in metadataModel.Syntax)
            {
                var metadataParameters = new SortedSet<MamlParameter>(metadataSyntax.Parameters,new MamlParameterNameComparer());

                foreach (var stringSyntax in stringModel.Syntax)
                {
                    var stringParameters = new SortedSet<MamlParameter>(stringSyntax.Parameters, new MamlParameterNameComparer());
                    if (metadataParameters.SetEquals(stringParameters) &&
                        stringSyntax.ParameterSetName != metadataSyntax.ParameterSetName)
                    {
                        Report($"\tParameter Set Name Updated: {metadataSyntax.ParameterSetName}\r\n\t\tOld Set: {stringSyntax.ParameterSetName}\r\n\t\tNew Set: {metadataSyntax.ParameterSetName}\r\n");
                    }
                }
            }
        
            //Processing Parameters for cmdlet
            var stringParameterSet = new SortedSet<MamlParameter>(stringModel.Parameters, new MamlParameterNameComparer());
            var metadataParameterSet = new SortedSet<MamlParameter>(metadataModel.Parameters, new MamlParameterNameComparer());
            var addedParameters = metadataParameterSet.Except(stringParameterSet, new MamlParameterEqualityComparer()).ToList();
            var removedParameters = stringParameterSet.Except(metadataParameterSet, new MamlParameterEqualityComparer()).ToList();

            foreach (var addedParam in addedParameters)
            {
                Report($"\tParameter Added: {addedParam.Name}\r\n");
                _cmdletUpdated = true;
            }
            foreach (var removedParam in removedParameters)
            {
                Report($"\tParameter Deleted: {removedParam.Name}\r\n");
                _cmdletUpdated = true;
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

            var matchedParametersSet = stringParameterSet.Intersect(metadataParameterSet,new MamlParameterEqualityComparer()).ToList();
            var matchedComparer = new MamlParameterAttributeComparer();

            foreach (var matchedParam in matchedParametersSet)
            {
                var metadataMatch = metadataModel.Parameters.Single(p => p.Name == matchedParam.Name);
                if (matchedComparer.Compare(matchedParam, metadataMatch) == 0)
                {
                    continue;
                }
                if (matchedParam.Type != metadataMatch.Type)
                {
                    _cmdletUpdated = true;
                    Report($"\tParameter Updated: {matchedParam.Name}\r\n\t\tType updated from {matchedParam.Type} to {metadataMatch.Type}\r\n");
                }
                if (string.Join(",", matchedParam.Aliases) != string.Join(",", metadataMatch.Aliases))
                {
                    _cmdletUpdated = true;
                    Report($"\tParameter Updated: {matchedParam.Name}\r\n\t\tAliases updated from {string.Join(",",matchedParam.Aliases)} to {string.Join(",",metadataMatch.Aliases)}\r\n");
                }
                if (matchedParam.Required != metadataMatch.Required)
                {
                    _cmdletUpdated = true;
                    Report($"\tParameter Updated: {matchedParam.Name}\r\n\t\tRequired updated from {matchedParam.Required} to {metadataMatch.Required}\r\n");
                }
                if (matchedParam.Position != metadataMatch.Position)
                {
                    _cmdletUpdated = true;
                    Report($"\tParameter Updated: {matchedParam.Name}\r\n\t\tPosition updated from {matchedParam.Position} to {metadataMatch.Position}\r\n");
                }
                if (matchedParam.DefaultValue != metadataMatch.DefaultValue)
                {
                    _cmdletUpdated = true;
                    Report($"\tParameter Updated: {matchedParam.Name}\r\n\t\tDefault Value updated from {matchedParam.DefaultValue} to {metadataMatch.DefaultValue}\r\n");
                }
                if (matchedParam.PipelineInput != metadataMatch.PipelineInput)
                {
                    _cmdletUpdated = true;
                    Report($"\tParameter Updated: {matchedParam.Name}\r\n\t\tAccepts Pipeline Input updated from {matchedParam.PipelineInput} to {metadataMatch.PipelineInput}\r\n");
                }
                if (matchedParam.Globbing != metadataMatch.Globbing)
                {
                    _cmdletUpdated = true;
                    Report($"\tParameter Updated: {matchedParam.Name}\r\n\t\tAccepts wildcard characters updated from {matchedParam.Globbing} to {metadataMatch.Globbing}\r\n");
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
