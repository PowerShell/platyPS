namespace Markdown.MAML.Model.YAML
{
    public class YamlInputOutput
    {
        string _type;
        public string Type
        {
            get
            {
                if (_type.Contains("#"))
                {
                    return _type.Replace("#", "\\#");
                }

                return _type;
            }
            set
            {
                _type = value;
            }
        }
        public string Description { get; set; }
    }
}