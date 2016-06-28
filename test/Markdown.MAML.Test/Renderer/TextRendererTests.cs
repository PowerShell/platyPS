using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Markdown.MAML.Model.Markdown;
using Markdown.MAML.Parser;
using Markdown.MAML.Renderer;

namespace Markdown.MAML.Test.Renderer
{
    public class TextRendererTests
    {
        [Fact]
        public void RendererCreatesAboutTopicString()
        {
            var renderer = new TextRenderer(80);
            MarkdownParser parser = new MarkdownParser();
            string markdown = @"# TopicName
## about_TopicName

```
ABOUT TOPIC NOTE:
The first header of the about topic should be the topic name.
The second header contains the lookup name used by the help system.

IE:
# Some Help Topic Name
## SomeHelpTopicFileName

This will be transformed into the text file as `about_SomeHelpTopicFileName`.

Do not include file extensions.
The second header should have no spaces.
```

# SHORT DESCRIPTION
{{Short Description Placeholder}}

```
ABOUT TOPIC NOTE:
About topics can be no longer than 80 characters wide when rendered to text.
Any topics greater than 80 characters will be automatically wrapped.
The generated about topic will be encoded UTF-8.
```

# LONG DESCRIPTION
{{Long Description Placeholder}}

## Optional Subtopics
{{Optional Subtopic Placeholder}}

# EXAMPLES
{{Code or descriptive examples of how to leverage functions described.}}

# NOTE
{{Note Placeholder - Additional information that a user needs to know.}}

# TROUBLESHOOTING NOTE
{{Troubleshooting Placeholder - Warns users of bug.}}

{{Explains behavior that is likely to change with fixes}}

# SEE ALSO
{{See also placeholder, list related articles, blogs, and video URLs.}}

- https://somewebsite.com

# KEYWORDS
{{List alternate names or titles for this topic that readers might use.}}

- {{Keyword Placeholder}}
- {{Keyword Placeholder}}
- {{Keyword Placeholder}}
- {{Keyword Placeholder}}
";
            DocumentNode document = parser.ParseString(new string[] { markdown });

            string content = renderer.AboutMarkdownToString(document);

            string expectedOut = @"TOPIC
    about_topicname

    ABOUT TOPIC NOTE:
    The first header of the about topic should be the topic name.
    The second header contains the lookup name used by the help system.
    
    IE:
    # Some Help Topic Name
    ## SomeHelpTopicFileName
    
    This will be transformed into the text file as `about_SomeHelpTopicFileName`.
    
    Do not include file extensions.
    The second header should have no spaces.

SHORT DESCRIPTION
    {{Short Description Placeholder}}

    ABOUT TOPIC NOTE:
    About topics can be no longer than 80 characters wide when rendered to text.
    Any topics greater than 80 characters will be automatically wrapped.
    The generated about topic will be encoded UTF-8.

LONG DESCRIPTION
    {{Long Description Placeholder}}

Optional Subtopics
    {{Optional Subtopic Placeholder}}

EXAMPLES
    {{Code or descriptive examples of how to leverage functions described.}}

NOTE
    {{Note Placeholder - Additional information that a user needs to know.}}

TROUBLESHOOTING NOTE
    {{Troubleshooting Placeholder - Warns users of bug.}}
    {{Explains behavior that is likely to change with fixes}}

SEE ALSO
    {{See also placeholder, list related articles, blogs, and video URLs.}}
    - https://somewebsite.com

KEYWORDS
    {{List alternate names or titles for this topic that readers might use.}}
    - {{Keyword Placeholder}}
    - {{Keyword Placeholder}}
    - {{Keyword Placeholder}}
    - {{Keyword Placeholder}}

";
            Assert.Equal(expectedOut, content);
        }
    }
}
