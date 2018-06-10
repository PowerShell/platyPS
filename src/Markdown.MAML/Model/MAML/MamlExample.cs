using Markdown.MAML.Model.Markdown;

namespace Markdown.MAML.Model.MAML
{
    public class MamlExample
    {
        public string Title { get; set; }
        public MamlCodeBlock[] Code { get; set; }
        public string Remarks { get; set; }
        public string Introduction { get; set; }

        /// <summary>
        /// Additional options that determine how the section will be formated when rendering markdown.
        /// </summary>
        public SectionFormatOption FormatOption { get; set; }

        /// <summary>
        /// Serves as a hash function for a <see cref="T:Markdown.MAML.Model.MAML.MamlExample"/> object.
        /// Ignores the FormatOption field.
        /// </summary>
        /// <returns>A hash code for this instance that is suitable for use in hashing algorithms and data structures such as a
        /// hash table.</returns>
        public override int GetHashCode()
        {
            int hash = 1;
            if (Code != null)
            {
                foreach (var codeBlock in Code) {
                    hash ^= codeBlock.GetHashCode();
                }
            }
            return
                hash ^
                (Title == null ? 123 : Title.GetHashCode()) ^
                (Remarks == null ? 12345 : Remarks.GetHashCode()) ^
                (Introduction == null ? 123457 : Introduction.GetHashCode());
        }
    }
}
