using Markdown.MAML.Model.MAML;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
            MamlCommand result = new MamlCommand()
            {
                Name = MergeMetadataProperty(metadataModel.Name, stringModel.Name),
                Synopsis = MergeStringProperty(metadataModel.Synopsis, stringModel.Synopsis),
                Description = MergeStringProperty(metadataModel.Description, stringModel.Description),
                Notes = MergeStringProperty(metadataModel.Notes, stringModel.Notes),
                Extent = stringModel.Extent
            };

            // TODO: convert into MergeMetadataProperty
            result.Links.AddRange(metadataModel.Links);

            // All examples come only from strtringModel
            result.Examples.AddRange(stringModel.Examples);

            // TODO: figure out what's the right thing for MamlInputOutput
            result.Inputs.AddRange(stringModel.Inputs);
            result.Outputs.AddRange(stringModel.Outputs);

            MergeParameters(result, metadataModel, stringModel);

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

            foreach (var param in metadataModel.Parameters)
            {
                var strParam = FindParameterByName(param.Name, stringModel.Parameters);
                if (strParam == null)
                {
                    Report("Parameter {0} cannot be found in markdown", param.Name);
                }
                else
                {
                    param.Description = strParam.Description;
                    param.DefaultValue = strParam.DefaultValue;
                    // don't update type
                    // param.Type = strParam.Type;
                    param.Extent = strParam.Extent;
                }

                result.Parameters.Add(param);
            }
        }

        private string MergeMetadataProperty(string metadataStr, string stringStr)
        {
            if (!StringComparer.Ordinal.Equals(metadataStr, stringStr))
            {
                // TODO: report
                // Report("Update {0} from {1} to {2}");   
            }

            return metadataStr;
        }

        private string MergeStringProperty(string metadataStr, string stringStr)
        {
            if (!StringComparer.Ordinal.Equals(metadataStr, stringStr))
            {
                // TODO: report
                // Report("Update {0} from {1} to {2}");   
            }

            return stringStr;
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
