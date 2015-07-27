using Markdown.MAML.Model;
using Markdown.MAML.Parser;
using System;
using System.Linq;
using Xunit;

namespace Markdown.MAML.Test
{
    public class ParserTests
    {
        [Fact]
        public void ParsesHeadingsWithHashPrefix()
        {
            const string headingText = "Heading Text";

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
            const string headingText = "Heading Text";

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

        private TNode ParseAndGetExpectedChild<TNode>(
            string markdownString, 
            MarkdownNodeType expectedNodeType)
        {
            MarkdownParser markdownParser = new MarkdownParser();
            DocumentNode documentNode = markdownParser.ParseString(markdownString);
            MarkdownNode firstChildNode = documentNode.Children.FirstOrDefault();

            Assert.Equal(MarkdownNodeType.Heading, firstChildNode.NodeType);
            return Assert.IsType<TNode>(firstChildNode);
        }
    }
}
