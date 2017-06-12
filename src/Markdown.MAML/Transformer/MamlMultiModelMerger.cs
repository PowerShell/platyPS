using Markdown.MAML.Model.MAML;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Text.RegularExpressions;

namespace Markdown.MAML.Transformer
{
    /// <summary>
    /// Model merger that's capable of combining few models at once.
    /// It's useful for scenarios, when you want to assign applicable tags to parameters.
    /// </summary>
    public class MamlMultiModelMerger
    {
        private Action<string> _infoCallback;
        private bool _ignoreTagsIfAllApplicable;
        private string _mergeMarker;

        public MamlMultiModelMerger() : this(null, true) { }

        /// <summary>
        /// </summary>
        /// <param name="infoCallback">Report string information to some channel</param>
        /// <param name="ignoreTagsIfAllApplicable">if True merger will skip adding applicable tags if it's applicable to all</param>
        public MamlMultiModelMerger(Action<string> infoCallback, bool ignoreTagsIfAllApplicable, string mergeMarker = "!!!!!! ")
        {
            _infoCallback = infoCallback;
            _ignoreTagsIfAllApplicable = ignoreTagsIfAllApplicable;
            _mergeMarker = mergeMarker;
        }

        /// <summary>
        /// Facade for Merge method that's easier to call from PowerShell
        /// </summary>
        /// <param name="applicableTag2Model"></param>
        /// <returns></returns>
        public MamlCommand MergePS(Hashtable applicableTag2Model)
        {
            Dictionary<string, MamlCommand> dict = new Dictionary<string, MamlCommand>();
            foreach (DictionaryEntry pair in applicableTag2Model)
            {
                MamlCommand value;
                if (pair.Value is PSObject)
                {
                    value = (pair.Value as PSObject).BaseObject as MamlCommand;
                }
                else
                {
                    value = pair.Value as MamlCommand;
                }

                if (value == null)
                {
                    throw new ArgumentException("Value of hashtable cannot be casted to MamlCommand");
                }

                dict[pair.Key.ToString()] = pair.Value as MamlCommand;
            }

            return this.Merge(dict);
        }

        public MamlCommand Merge(Dictionary<string, MamlCommand> applicableTag2Model)
        {
            if (applicableTag2Model.Count == 0)
            {
                throw new ArgumentException("applicableTag2Model");
            }

            var tagsModel = applicableTag2Model.ToList();

            // just take one model to use name from it and such
            var referenceModel = applicableTag2Model.First().Value;

            MamlCommand result = null;
            result = new MamlCommand()
            {
                Name = referenceModel.Name,
                Synopsis = MergeText(tagsModel.ToDictionary(pair => pair.Key, pair => pair.Value.Synopsis)),
                Description = MergeText(tagsModel.ToDictionary(pair => pair.Key, pair => pair.Value.Description)),
                Notes = MergeText(tagsModel.ToDictionary(pair => pair.Key, pair => pair.Value.Notes)),
                Extent = referenceModel.Extent
            };

            // post all links, exclude dups
            result.Links.AddRange(MergeEntityList(tagsModel.ToDictionary(pair => pair.Key, pair => pair.Value.Links)));

            MergeExamples(result, applicableTag2Model);

            result.Inputs.AddRange(MergeEntityList(tagsModel.ToDictionary(pair => pair.Key, pair => pair.Value.Inputs)));
            result.Outputs.AddRange(MergeEntityList(tagsModel.ToDictionary(pair => pair.Key, pair => pair.Value.Outputs)));

            MergeParameters(result, applicableTag2Model);
            return result;
        }

        /// <summary>
        /// Merge entities, exclude duplicates and preserve the order.
        /// </summary>
        /// <param name="links"></param>
        /// <returns></returns>
        private List<TEntity> MergeEntityList<TEntity>(Dictionary<string, List<TEntity>> applicableTag2Model) 
            where TEntity : IEquatable<TEntity>
        {
            List<TEntity> result = new List<TEntity>();
            foreach(var pair in applicableTag2Model)
            {
                foreach (var candidate in pair.Value)
                {
                    // this cycle can be optimized but that's fine
                    bool found = false; 
                    foreach (var added in result)
                    {
                        if (added.Equals(candidate))
                        {
                            found = true;
                            break;
                        }
                    }

                    if (!found)
                    {
                        result.Add(candidate);
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Combine examples together. Don't perform any reduction, but group the examples after each other and add tag name to the title.
        /// TODO can we combine examples better perhaps?
        /// </summary>
        /// <param name="result"></param>
        /// <param name="applicableTag2Model"></param>
        private void MergeExamples(MamlCommand result, Dictionary<string, MamlCommand> applicableTag2Model)
        {
            int max = applicableTag2Model.Select(pair => pair.Value.Examples.Count).Max();
            for (int i = 0; i < max; i++)
            {
                foreach (var pair in applicableTag2Model)
                {
                    if (pair.Value.Examples.Count > i)
                    {
                        var example = pair.Value.Examples.ElementAt(i);
                        example.Title += string.Format(" ({0})", pair.Key);
                        result.Examples.Add(example);
                    }
                }
            }
        }

        private MamlParameter FindParameterByName(string name, IEnumerable<MamlParameter> list)
        {
            return list.FirstOrDefault(x => StringComparer.OrdinalIgnoreCase.Equals(name, x.Name));
        }

        private void MergeParameters(MamlCommand result, Dictionary<string, MamlCommand> applicableTag2Model)
        {
            // First handle syntax.
            // TODO: we can do it better probably
            var tagsModel = applicableTag2Model.ToList();
            result.Syntax.AddRange(MergeEntityList(tagsModel.ToDictionary(pair => pair.Key, pair => pair.Value.Syntax)));

            // Then merge individual parameters

            var allNames = tagsModel.SelectMany(pair => pair.Value.Parameters).Select(p => p.Name).Distinct().ToList();
            foreach (string name in allNames)
            {
                Dictionary<string, MamlParameter> paramsToMerge = new Dictionary<string, MamlParameter>();
                foreach (var pair in tagsModel)
                {
                    MamlParameter candidate = FindParameterByName(name, pair.Value.Parameters);
                    if (candidate != null)
                    {
                        paramsToMerge[pair.Key] = candidate;
                    }
                }

                string newDescription = MergeText(paramsToMerge.ToDictionary(pair => pair.Key, pair => pair.Value.Description));
                var newParameter = paramsToMerge.First().Value.Clone();
                newParameter.Description = newDescription;
                if (paramsToMerge.Count != applicableTag2Model.Count || !this._ignoreTagsIfAllApplicable)
                {
                    // we should not update applicable tags, if it's applicable to everything and not explicitly specified
                    newParameter.Applicable = paramsToMerge.Select(p => p.Key).ToArray();
                }

                result.Parameters.Add(newParameter);
            }
        }

        /// <summary>
        /// Merges text attributed with tags into a single representation
        /// </summary>
        private string MergeText(Dictionary<string, string> applicableTag2Text)
        {
            var reverseMap = new Dictionary<string, List<string>>();
            foreach (var pair in applicableTag2Text)
            {
                string pretty = pair.Value != null ? pair.Value.Trim() : "";
                List<string> tags;
                if (!reverseMap.TryGetValue(pretty, out tags))
                {
                    tags = new List<string>();
                    reverseMap[pretty] = tags;
                }
                tags.Add(pair.Key);
            }

            if (reverseMap.Count == 1)
            {
                return reverseMap.Keys.First();
            }

            var result = new StringBuilder();
            foreach (var pair in reverseMap)
            {
                var tagsString = string.Join(", ", pair.Value);
                result.AppendFormat("{0}{1}{2}{2}{3}{2}{2}", _mergeMarker, tagsString, Environment.NewLine, pair.Key);
            }

            return result.ToString().Trim();
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
