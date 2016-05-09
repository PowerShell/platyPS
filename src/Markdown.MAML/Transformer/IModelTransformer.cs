using System.Collections.Generic;
using Markdown.MAML.Model.MAML;
using Markdown.MAML.Model.Markdown;

namespace Markdown.MAML.Transformer
{
    public interface IModelTransformer
    {
        IEnumerable<MamlCommand> NodeModelToMamlModel(DocumentNode node);
    }
}