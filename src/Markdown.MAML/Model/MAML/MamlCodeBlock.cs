namespace Markdown.MAML.Model.MAML
{
    /// <summary>
    /// A section of code such as PowerShell commands or command output.
    /// </summary>
    public sealed class MamlCodeBlock
    {
        public MamlCodeBlock(string text, string languageMoniker = null)
        {
            Text = text;
            LanguageMoniker = languageMoniker ?? string.Empty;
        }

        /// <summary>
        /// An optional language or info-string. If no language string is suppled plain text is assumed.
        /// For more information see: http://spec.commonmark.org/0.28/#info-string
        /// </summary>
        public string LanguageMoniker { get; private set; }

        /// <summary>
        /// The text of the code block.
        /// </summary>
        public string Text { get; private set; }

        /// <summary>
        /// Serves as a hash function for a <see cref="T:Markdown.MAML.Model.MAML.MamlCodeBlock"/> object.
        /// </summary>
        /// <returns>A hash code for this instance that is suitable for use in hashing algorithms and data structures such as a
        /// hash table.</returns>
        public override int GetHashCode()
        {
            return
                (Text == null ? 679 : Text.GetHashCode()) ^
                (LanguageMoniker == null ? 765679 : LanguageMoniker.GetHashCode());
        }
    }
}
