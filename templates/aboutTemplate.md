# TOPIC NAME
## FileNameForHelpSystem

The first header of the about topic should be the topic name.
The second header contains the lookup name used by the help system.

IE:
```
# Some Help Topic Name
## SomeHelpTopicFileName
```
This will be transformed into the text file as `about_SomeHelpTopicFileName`.

Do not include file extensions.
The second header should have no spaces.              

# SHORT DESCRIPTION
About topics can be no longer than 80 characters wide when rendered to text.
Any topics greater than 80 characters will be automatically wrapped.
The generated about topic will be encoded UTF-8.
                                   
# LONG DESCRIPTION
This section can be used to provide a detailed description. 

All informaiton on a feature or technology that was not in other console help.
    
## Optional Subtopics
Additional information can be broken into sub categories.

# EXAMPLES
Code or descriptive examples of how to leverage the functions described.

## How to get about help: 
```
Get-Help about_SomeHelpTopicFileName
```

# NOTE
Additional information that a user needs to know.

# TROUBLESHOOTING NOTE
Bugs that you are likely to fix. 

Warns users that the current behavior is likely to change


# SEE ALSO
- [Useful related content](https://msdn.microsoft.com/en-us/powershell/) 
`This should be a link to related content, see raw markdown.`

##List related PowerShell help topics.
You can also list related articles, abouts, blogs, and videos with their URLs.


# KEYWORDS

- Sample Keyword 1
- Sample Keyword 2
- Sample Keyword 3
- Sample Keyword 4 

##Get-Help finds the keywords during its full-text search.    
List alternate names or titles for this topic that readers might use. 


    