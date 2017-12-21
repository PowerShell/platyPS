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
            Common.AssertMultilineEqual(expectedOut, content);
        }


        [Fact]
        public void RendererSuccessfullParsesMultiLineAboutContent()
        {
            var renderer = new TextRenderer(80);
            MarkdownParser parser = new MarkdownParser();
            string markdown = @"# About Throw
## about_Throw

# SHORT DESCRIPTION

Describes the Throw keyword, which generates a terminating error.

# LONG DESCRIPTION
The Throw keyword causes a terminating error. You can use the Throw keyword
to stop the processing of a command, function, or script.

For example, you can use the Throw keyword in the script block of an If
statement to respond to a condition or in the Catch block of a
Try-Catch-Finally statement. You can also use the Throw keyword in a
parameter declaration to make a function parameter mandatory.

The Throw keyword can throw any object, such as a user message string or
the object that caused the error.


## SYNTAX

The syntax of the Throw keyword is as follows:

```
throw [<expression>]
```


The expression in the Throw syntax is optional. When the Throw statement
does not appear in a Catch block, and it does not include an expression,
it generates a ScriptHalted error.

```
C:\PS> throw

ScriptHalted
At line:1 char:6
+ throw <<<<
    + CategoryInfo          : OperationStopped: (:) [], RuntimeException
    + FullyQualifiedErrorId : ScriptHalted
```

If the Throw keyword is used in a Catch block without an expression, it
throws the current RuntimeException again. For more information, see
about_Try_Catch_Finally.


## THROWING A STRING

The optional expression in a Throw statement can be a string, as shown in
the following example:

```
C:\PS> throw 'This is an error.'

This is an error.
At line:1 char:6
+ throw <<<< 'This is an error.'
    + CategoryInfo          : OperationStopped: (This is an error.:String) [], RuntimeException
    + FullyQualifiedErrorId : This is an error.
```

## THROWING OTHER OBJECTS

The expression can also be an object that throws the object that represents
the PowerShell process, as shown in the following example:

```
C:\PS> throw (get-process PowerShell)

System.Diagnostics.Process (PowerShell)
At line:1 char:6
+ throw <<<<  (get-process PowerShell)
    + CategoryInfo          : OperationStopped: (System.Diagnostics.Process (PowerShell):Process) [
RuntimeException
    + FullyQualifiedErrorId : System.Diagnostics.Process(PowerShell)
```

You can use the TargetObject property of  the ErrorRecord object in the
$error automatic variable to examine the error.

```
C:\PS> $error[0].targetobject

Handles  NPM(K)    PM(K)      WS(K) VM(M)   CPU(s)     Id ProcessName

-------  ------    -----      ----- -----   ------     -- -----------

    319      26    61016      70864   568     3.28   5548 PowerShell
```

You can also throw an ErrorRecord object or a Microsoft.NET Framework
exception. The following example uses the Throw keyword to throw a
System.FormatException object.

```
C:\PS> $formatError = new- object system.formatexception

C:\PS> throw $formatError

One of the identified items was in an invalid format.
At line:1 char:6
+ throw <<<<  $formatError
    + CategoryInfo          : OperationStopped: (:) [], FormatException
    + FullyQualifiedErrorId : One of the identified items was in an invalid format.
```

## RESULTING ERROR

The Throw keyword can generate an ErrorRecord object. The Exception
property of the ErrorRecord object contains a RuntimeException object.
The remainder of the ErrorRecord object and the RuntimeException object
vary with the object that the Throw keyword throws.

The RunTimeException object is wrapped in an ErrorRecord object, and the
ErrorRecord object is automatically saved in the $Error automatic variable.


## USING THROW TO CREATE A MANDATORY PARAMETER

You can use the Throw keyword to make a function parameter mandatory.

This is an alternative to using the Mandatory parameter of the Parameter
keyword.When you use the Mandatory parameter, the system prompts the user
for the required parameter value. When you use the Throw keyword, the
command stops and displays the error record.

For example, the Throw keyword in the parameter subexpression makes the
Path parameter a required parameter in the function.

In this case, the Throw keyword throws a message string, but it is the
presence of the Throw keyword that generates the terminating error if
the Path parameter is not specified. The expression that follows Throw
is optional.

```
function Get-XMLFiles
{
        param($path = $(throw 'The Path parameter is required.'))
    dir - path $path\*.xml - recurse | sort lastwritetime | ft lastwritetime, attributes, name - auto
}
```

# SEE ALSO

- about_Break
- about_Continue
- about_Scope
- about_Trap
- about_Try_Catch_Finally
";
            DocumentNode document = parser.ParseString(new string[] { markdown });

            string content = renderer.AboutMarkdownToString(document);

            string expectedOut = @"TOPIC
    about_throw

SHORT DESCRIPTION
    Describes the Throw keyword, which generates a terminating error.

LONG DESCRIPTION
    The Throw keyword causes a terminating error. You can use the Throw keyword
    to stop the processing of a command, function, or script.
    For example, you can use the Throw keyword in the script block of an If
    statement to respond to a condition or in the Catch block of a
    Try-Catch-Finally statement. You can also use the Throw keyword in a
    parameter declaration to make a function parameter mandatory.
    The Throw keyword can throw any object, such as a user message string or the
    object that caused the error.

SYNTAX
    The syntax of the Throw keyword is as follows:

    throw [<expression>]

    The expression in the Throw syntax is optional. When the Throw statement
    does not appear in a Catch block, and it does not include an expression, it
    generates a ScriptHalted error.

    C:\PS> throw
    
    ScriptHalted
    At line:1 char:6
    + throw <<<<
        + CategoryInfo          : OperationStopped: (:) [], RuntimeException
        + FullyQualifiedErrorId : ScriptHalted

    If the Throw keyword is used in a Catch block without an expression, it
    throws the current RuntimeException again. For more information, see
    about_Try_Catch_Finally.

THROWING A STRING
    The optional expression in a Throw statement can be a string, as shown in
    the following example:

    C:\PS> throw 'This is an error.'
    
    This is an error.
    At line:1 char:6
    + throw <<<< 'This is an error.'
        + CategoryInfo          : OperationStopped: (This is an error.:String) [], RuntimeException
        + FullyQualifiedErrorId : This is an error.

THROWING OTHER OBJECTS
    The expression can also be an object that throws the object that represents
    the PowerShell process, as shown in the following example:

    C:\PS> throw (get-process PowerShell)
    
    System.Diagnostics.Process (PowerShell)
    At line:1 char:6
    + throw <<<<  (get-process PowerShell)
        + CategoryInfo          : OperationStopped: (System.Diagnostics.Process (PowerShell):Process) [
    RuntimeException
        + FullyQualifiedErrorId : System.Diagnostics.Process(PowerShell)

    You can use the TargetObject property of  the ErrorRecord object in the
    $error automatic variable to examine the error.

    C:\PS> $error[0].targetobject
    
    Handles  NPM(K)    PM(K)      WS(K) VM(M)   CPU(s)     Id ProcessName
    
    -------  ------    -----      ----- -----   ------     -- -----------
    
        319      26    61016      70864   568     3.28   5548 PowerShell

    You can also throw an ErrorRecord object or a Microsoft.NET Framework
    exception. The following example uses the Throw keyword to throw a
    System.FormatException object.

    C:\PS> $formatError = new- object system.formatexception
    
    C:\PS> throw $formatError
    
    One of the identified items was in an invalid format.
    At line:1 char:6
    + throw <<<<  $formatError
        + CategoryInfo          : OperationStopped: (:) [], FormatException
        + FullyQualifiedErrorId : One of the identified items was in an invalid format.

RESULTING ERROR
    The Throw keyword can generate an ErrorRecord object. The Exception property
    of the ErrorRecord object contains a RuntimeException object. The remainder
    of the ErrorRecord object and the RuntimeException object vary with the
    object that the Throw keyword throws.
    The RunTimeException object is wrapped in an ErrorRecord object, and the
    ErrorRecord object is automatically saved in the $Error automatic variable.

USING THROW TO CREATE A MANDATORY PARAMETER
    You can use the Throw keyword to make a function parameter mandatory.
    This is an alternative to using the Mandatory parameter of the Parameter
    keyword.When you use the Mandatory parameter, the system prompts the user
    for the required parameter value. When you use the Throw keyword, the
    command stops and displays the error record.
    For example, the Throw keyword in the parameter subexpression makes the Path
    parameter a required parameter in the function.
    In this case, the Throw keyword throws a message string, but it is the
    presence of the Throw keyword that generates the terminating error if the
    Path parameter is not specified. The expression that follows Throw is
    optional.

    function Get-XMLFiles
    {
            param($path = $(throw 'The Path parameter is required.'))
        dir - path $path\*.xml - recurse | sort lastwritetime | ft lastwritetime, attributes, name - auto
    }

SEE ALSO
    - about_Break
    - about_Continue
    - about_Scope
    - about_Trap
    - about_Try_Catch_Finally

";
            Common.AssertMultilineEqual(expectedOut, content);
        }
    }
}
