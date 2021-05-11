using Markdown.MAML.Model.MAML;
using Markdown.MAML.Model.Markdown;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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

        public MamlMultiModelMerger() : this(null, true, "!!! ") { }

        /// <summary>
        /// </summary>
        /// <param name="infoCallback">Report string information to some channel</param>
        /// <param name="ignoreTagsIfAllApplicable">if True merger will skip adding applicable tags if it's applicable to all</param>
        public MamlMultiModelMerger(Action<string> infoCallback, bool ignoreTagsIfAllApplicable, string mergeMarker)
        {
            _infoCallback = infoCallback;
            _ignoreTagsIfAllApplicable = ignoreTagsIfAllApplicable;
            _mergeMarker = mergeMarker;
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
                Synopsis = new SectionBody(MergeText(tagsModel.ToDictionary(pair => pair.Key, pair => pair.Value.Synopsis?.Text))),
                Description = new SectionBody(MergeText(tagsModel.ToDictionary(pair => pair.Key, pair => pair.Value.Description?.Text))),
                Notes = new SectionBody(MergeText(tagsModel.ToDictionary(pair => pair.Key, pair => pair.Value.Notes?.Text))),
                Extent = referenceModel.Extent
            };

            result.Links.AddRange(MergeSimplifiedLinks(tagsModel.Select(pair => pair.Value.Links)));

            MergeExamples(result, applicableTag2Model);

            result.Inputs.AddRange(MergeEntityList(tagsModel.Select(pair => pair.Value.Inputs)));
            result.Outputs.AddRange(MergeEntityList(tagsModel.Select(pair => pair.Value.Outputs)));

            MergeParameters(result, applicableTag2Model);
            return result;
        }

        private IEnumerable<MamlLink> MergeSimplifiedLinks(IEnumerable<List<MamlLink>> linksList)
        {
            // In theory we could simply use MergeEntityList, but we have this SimplifiedLinks hack:
            // we just put whole paragraphs of text into LinkName.

            // To acoount for it we should
            // split any simplified link into separate ones.
            // Then we can combine them and return in the form of simplified links.

            List<List<MamlLink>> candidates = new List<List<MamlLink>>();
            foreach (var links in linksList)
            {
                foreach (var link in links)
                {
                    if (!link.IsSimplifiedTextLink)
                    {
                        throw new ArgumentException("All links are expected in simplified form");
                    }
                    string[] segments = link.LinkName.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
                    candidates.Add(segments.Select(s => new MamlLink(isSimplifiedTextLink: true) { LinkName = s.Trim() + "\r\n\r\n" }).ToList());
                }
            }

            return MergeEntityList(candidates);
        }

        /// <summary>
        /// Merge entities, exclude duplicates and preserve the order.
        /// </summary>
        /// <param name="links"></param>
        /// <returns></returns>
        private List<TEntity> MergeEntityList<TEntity>(IEnumerable<List<TEntity>> entities) 
            where TEntity : IEquatable<TEntity>
        {
            List<TEntity> result = new List<TEntity>();
            foreach(var entity in entities)
            {
                foreach (var candidate in entity)
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
        /// Merge Parameter lists, exclude duplicates by name and preserve the order.
        /// </summary>
        /// <param name="links"></param>
        /// <returns></returns>
        private List<MamlParameter> MergeParameterList(IEnumerable<List<MamlParameter>> parameterLists)
        {
            List<MamlParameter> result = new List<MamlParameter>();
            foreach (var list in parameterLists)
            {
                foreach (var candidate in list)
                {
                    // this cycle can be optimized but that's fine
                    bool found = false;
                    foreach (var added in result)
                    {
                        if (StringComparer.OrdinalIgnoreCase.Equals(added.Name, candidate.Name))
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
        /// Combine examples together.
        /// If examples are identical, we deduplicate them.
        /// If example is not found in all the models, add the applicable tags in parentheses.
        /// </summary>
        /// <param name="result"></param>
        /// <param name="applicableTag2Model"></param>
        private void MergeExamples(MamlCommand result, Dictionary<string, MamlCommand> applicableTag2Model)
        {
            // We want to more or less keep the order of the original examples.
            // To do it, we use the following algorithm:
            // We are adding examples sequentially:
            // 1st from every model, 2nd from every model, etc.
            // At the same time we are doing deduplication by hashcode.

            // this dictonary gives the mapping between example hashes and applicable tags
            var hash2tag = new Dictionary<int, List<String>>();
            foreach (var pair in applicableTag2Model)
            {
                foreach (var example in pair.Value.Examples) {
                    List<String> tagList;
                    int hash = example.GetHashCode();
                    if (!hash2tag.TryGetValue(hash, out tagList))
                    {
                        tagList = new List<string>();
                        hash2tag[hash] = tagList;
                    }
                    tagList.Add(pair.Key);
                }
            }

            // this hashset contains the example hashes for already used examples
            var usedHashes = new HashSet<int>();

            int max = applicableTag2Model.Select(pair => pair.Value.Examples.Count).Max();
            for (int i = 0; i < max; i++)
            {
                foreach (var pair in applicableTag2Model)
                {
                    if (pair.Value.Examples.Count > i)
                    {
                        var example = pair.Value.Examples.ElementAt(i);
                        int hash = example.GetHashCode();
                        if (usedHashes.Contains(hash)) {
                            continue;
                        }
                        // if all tags are covered, don't add anything
                        if (hash2tag[hash].Count < applicableTag2Model.Count)
                        {
                            // we will sort the same list few times, but that's fine
                            hash2tag[hash].Sort();
                            string listString = string.Join(", ", hash2tag[hash]);
                            example.Title += string.Format(" ({0})", listString);
                        }
                        usedHashes.Add(hash);
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
            // To avoid dealing with complicated applicableTag + ParameterSetName pairs.
            // They already serve very similar purpose.
            // 
            // We simply merge syntaxes of the same name. That way all possible parameter names from all applicable tags
            // apearing in the syntax. This is not ideal, but alternative would be introducing "applicable" for syntax.
            // That would make it even more complicated.
            var tagsModel = applicableTag2Model.ToList();
            var syntaxes = tagsModel.SelectMany(pair => pair.Value.Syntax);
            var syntaxNames = syntaxes.Select(syntax => syntax.ParameterSetName).Distinct(StringComparer.OrdinalIgnoreCase);
            var defaultSyntaxNames = syntaxes.Where(syntax => syntax.IsDefault).Select(syntax => syntax.ParameterSetName).Distinct(StringComparer.OrdinalIgnoreCase);
            if (defaultSyntaxNames.Count() > 1)
            {
                // reporting warning and continue
                Report("Found conflicting default ParameterSets ({0}) in applicableTags ({1})", 
                    string.Join(", ", defaultSyntaxNames), string.Join(", ", applicableTag2Model.Keys));
            }
            string defaultSyntaxName = defaultSyntaxNames.FirstOrDefault();

            foreach (string syntaxName in syntaxNames)
            {
                var newSyntax = new MamlSyntax()
                {
                    ParameterSetName = syntaxName,
                    IsDefault = StringComparer.OrdinalIgnoreCase.Equals(defaultSyntaxName, syntaxName),
                };

                var paramSetsToMerge = syntaxes.Where(syntax => StringComparer.OrdinalIgnoreCase.Equals(syntax.ParameterSetName, syntaxName));

                newSyntax.Parameters.AddRange(MergeParameterList(paramSetsToMerge.Select(syntax => syntax.Parameters)));
                result.Syntax.Add(newSyntax);
            }

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
                string pretty = pair.Value != null ? pair.Value : "";
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
                result.AppendFormat("{0}{1}{2}{2}{3}{2}{2}", _mergeMarker, tagsString, "\r\n", pair.Key);
            }

            return result.ToString();
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
