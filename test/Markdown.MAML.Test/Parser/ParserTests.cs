using System;
using System.Linq;
using Markdown.MAML.Model;
using Markdown.MAML.Model.Markdown;
using Markdown.MAML.Parser;
using Xunit;

namespace Markdown.MAML.Test.Parser
{
    public class ParserTests
    {
        const string headingText = "Heading Text";
        const string codeBlockText = "Code block text\r\non multiple lines";
        const string paragraphText = "Some text\r\non multiple\r\nlines";
        const string hyperlinkText = "Microsoft Corporation";
        const string hyperlinkUri = "https://go.microsoft.com/fwlink/?LinkID=135175&query=stuff";

        [Fact]
        public void ParsesHeadingsWithHashPrefix()
        {

            for (int i = 1; i <= 6; i++)
            {
                HeadingNode headingNode =
                    this.ParseAndGetExpectedChild<HeadingNode>(
                        new String('#', i) + headingText + "\r\n",
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
                        headingText + "\r\n" + headingUnderlines[i - 1] + "\r\n",
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
                    string.Format("```\r\n{0}\r\n```\r\n", codeBlockText),
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

            Assert.Equal(paragraphText, paragraphNode.Spans.First().Text);
        }

        [Fact]
        public void ParsesHyperlink()
        {
            ParagraphNode paragraphNode =
                this.ParseAndGetExpectedChild<ParagraphNode>(
                    string.Format(
                        "[{0}]({1})",
                        hyperlinkText,
                        hyperlinkUri),
                    MarkdownNodeType.Paragraph);

            HyperlinkSpan hyperlinkSpan =
                Assert.IsType<HyperlinkSpan>(
                    paragraphNode.Spans.FirstOrDefault());

            Assert.Equal(hyperlinkText, hyperlinkSpan.Text);
            Assert.Equal(hyperlinkUri, hyperlinkSpan.Uri);
        }

        [Fact]
        public void ParsesHyperlinkWithoutLink()
        {
            ParagraphNode paragraphNode =
                this.ParseAndGetExpectedChild<ParagraphNode>(
                    string.Format(
                        "[{0}]()",
                        hyperlinkText),
                    MarkdownNodeType.Paragraph);

            HyperlinkSpan hyperlinkSpan =
                Assert.IsType<HyperlinkSpan>(
                    paragraphNode.Spans.FirstOrDefault());

            Assert.Equal(hyperlinkText, hyperlinkSpan.Text);
            Assert.Equal("", hyperlinkSpan.Uri);
        }

        [Fact]
        public void TextSpansCanContainDoubleQuotes()
        {
            string documentText = @"
# Foo
This is a :""text"" with doublequotes
";
            MarkdownParser markdownParser = new MarkdownParser();
            DocumentNode documentNode =
                markdownParser.ParseString(
                    documentText);
            var children = documentNode.Children.ToArray();
            Assert.Equal(2, children.Count());
            var spans = Assert.IsType<ParagraphNode>(children[1]).Spans.ToArray();
            Assert.Equal(@"This is a :""text"" with doublequotes", spans[0].Text);
        }

        [Fact]
        public void TextSpansCanContainBrackets()
        {
            string documentText = @"
# Foo
about_Hash_Tables (http://go.microsoft.com/fwlink/?LinkID=135175).
";
            MarkdownParser markdownParser = new MarkdownParser();
            DocumentNode documentNode =
                markdownParser.ParseString(
                    documentText);
            var children = documentNode.Children.ToArray();
            Assert.Equal(2, children.Count());
            var spans = Assert.IsType<ParagraphNode>(children[1]).Spans.ToArray();
            Assert.Equal(@"about_Hash_Tables (http://go.microsoft.com/fwlink/?LinkID=135175).", spans[0].Text);
        }


        [Fact]
        public void ParsesParagraphWithSupportedCharacters()
        {
            const string allCharacterString = 
                "This is a \"test\" string; it's very helpful.  Success: yes!?";

            ParagraphNode paragraphNode =
                this.ParseAndGetExpectedChild<ParagraphNode>(
                    allCharacterString,
                    MarkdownNodeType.Paragraph);

            ParagraphSpan[] spans = paragraphNode.Spans.ToArray();

            Assert.Equal(allCharacterString, spans[0].Text);
        }

        [Fact]
        public void ParsesParagraphWithFormattedSpans()
        {
            ParagraphNode paragraphNode =
                this.ParseAndGetExpectedChild<ParagraphNode>(
                    "Normal\r\nText *Italic*  \r\n**Bold**\r\n### New header!\r\nBoooo\r\n----\r\n",
                    MarkdownNodeType.Paragraph);

            ParagraphSpan[] spans = paragraphNode.Spans.ToArray();

            Assert.Equal("Normal\r\nText", spans[0].Text);
            Assert.Equal("Italic", spans[1].Text);
            Assert.IsType<HardBreakSpan>(spans[2]);
            Assert.Equal("Bold", spans[3].Text);
        }

        [Fact]
        public void ParsesDocumentWithMultipleNodes()
        {
            string documentText =
                string.Format(
@"
# {0}

{2}

```
{1}
```

## {0}
{2} [{3}]({4})
", headingText, codeBlockText, paragraphText, hyperlinkText, hyperlinkUri);

            MarkdownParser markdownParser = new MarkdownParser();
            DocumentNode documentNode =
                markdownParser.ParseString(
                    documentText);

            HeadingNode headingNode =
                this.AssertNodeType<HeadingNode>(
                    documentNode.Children.ElementAtOrDefault(0),
                    MarkdownNodeType.Heading);

            Assert.Equal(headingText, headingNode.Text);
            Assert.Equal(1, headingNode.HeadingLevel);

            CodeBlockNode codeBlockNode =
                this.AssertNodeType<CodeBlockNode>(
                    documentNode.Children.ElementAtOrDefault(2),
                    MarkdownNodeType.CodeBlock);

            Assert.Equal(codeBlockText, codeBlockNode.Text);

            headingNode =
                this.AssertNodeType<HeadingNode>(
                    documentNode.Children.ElementAtOrDefault(3),
                    MarkdownNodeType.Heading);

            Assert.Equal(headingText, headingNode.Text);
            Assert.Equal(2, headingNode.HeadingLevel);

            ParagraphNode paragraphNode =
                this.AssertNodeType<ParagraphNode>(
                    documentNode.Children.ElementAtOrDefault(4),
                    MarkdownNodeType.Paragraph);

            Assert.Equal(paragraphText, paragraphNode.Spans.First().Text);

            HyperlinkSpan hyperlinkSpan =
                Assert.IsType<HyperlinkSpan>(
                    paragraphNode.Spans.ElementAt(1));

            Assert.Equal(hyperlinkText, hyperlinkSpan.Text);
            Assert.Equal(hyperlinkUri, hyperlinkSpan.Uri);
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
