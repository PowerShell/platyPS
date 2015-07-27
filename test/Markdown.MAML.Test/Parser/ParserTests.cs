using Markdown.MAML.Model;
using Markdown.MAML.Parser;
using System;
using System.Linq;
using Xunit;

namespace Markdown.MAML.Test
{
    public class ParserTests
    {
        const string headingText = "Heading Text";
        const string codeBlockText = "Code block text\non multiple lines";
        const string paragraphText = "Some text\non multiple\nlines";

        [Fact]
        public void ParsesHeadingsWithHashPrefix()
        {

            for (int i = 1; i <= 6; i++)
            {
                HeadingNode headingNode =
                    this.ParseAndGetExpectedChild<HeadingNode>(
                        new String('#', i) + headingText,
                        MarkdownNodeType.Heading);

                Assert.Equal(i, headingNode.HeadingLevel);
                Assert.Equal(headingText, headingNode.Text);
            }
        }

        [Fact]
        public void ParsesHeadingsWithUnderlines()
        {
            string[] headingUnderlines =
            {
                new String('=', headingText.Length),
                new String('-', headingText.Length)
            };

            for (int i = 1; i <= 2; i++)
            {
                HeadingNode headingNode =
                    this.ParseAndGetExpectedChild<HeadingNode>(
                        headingText + "\n" + headingUnderlines[i - 1],
                        MarkdownNodeType.Heading);

                Assert.Equal(i, headingNode.HeadingLevel);
                Assert.Equal(headingText, headingNode.Text);
            }
        }

        [Fact]
        public void ParsesCodeBlock()
        {
            CodeBlockNode codeBlockNode =
                this.ParseAndGetExpectedChild<CodeBlockNode>(
                    string.Format("```\n{0}\n```", codeBlockText),
                    MarkdownNodeType.CodeBlock);

            Assert.Equal(codeBlockText, codeBlockNode.Text);
        }

        [Fact]
        public void ParsesParagraph()
        {
            ParagraphNode paragraphNode =
                this.ParseAndGetExpectedChild<ParagraphNode>(
                    paragraphText,
                    MarkdownNodeType.Paragraph);

            // Paragraph text will end with two newlines due to normalization
            Assert.Equal(paragraphText, paragraphNode.Text);
        }

        [Fact]
        public void ParsesDocumentWithMultipleNodes()
        {
            string documentText =
                string.Format(
@"
# {0}

```
{1}
```

{2}
", headingText, codeBlockText, paragraphText);

            MarkdownParser markdownParser = new MarkdownParser();
            DocumentNode documentNode =
                markdownParser.ParseString(
                    documentText);

            HeadingNode headingNode =
                this.AssertNodeType<HeadingNode>(
                    documentNode.Children.ElementAtOrDefault(0),
                    MarkdownNodeType.Heading);

            Assert.Equal(headingText, headingNode.Text);

            CodeBlockNode codeBlockNode =
                this.AssertNodeType<CodeBlockNode>(
                    documentNode.Children.ElementAtOrDefault(1),
                    MarkdownNodeType.CodeBlock);

            Assert.Equal(codeBlockText, codeBlockNode.Text);

            ParagraphNode paragraphNode =
                this.AssertNodeType<ParagraphNode>(
                    documentNode.Children.ElementAtOrDefault(2),
                    MarkdownNodeType.Paragraph);

            Assert.Equal(paragraphText, paragraphNode.Text);
        }

        private TNode ParseAndGetExpectedChild<TNode>(
            string markdownString, 
            MarkdownNodeType expectedNodeType)
        {
            MarkdownParser markdownParser = new MarkdownParser();
            DocumentNode documentNode = markdownParser.ParseString(markdownString);
            return 
                this.AssertNodeType<TNode>(
                    documentNode.Children.FirstOrDefault(),
                    expectedNodeType);
        }

        private TNode AssertNodeType<TNode>(
            MarkdownNode markdownNode,
            MarkdownNodeType expectedNodeType)
        {
            Assert.NotNull(markdownNode);
            Assert.Equal(expectedNodeType, markdownNode.NodeType);
            return Assert.IsType<TNode>(markdownNode);
        }
    }
}
