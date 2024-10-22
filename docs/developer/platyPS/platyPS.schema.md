# Schema

PlatyPS requires you to keep the content in a specific structure and Markdown notation. Any authoring must not break this formatting or the MAML will not be generated correctly.
It closely resembles output of `Get-Help`.

## Legend

*   `{string}` - single-line string value
*   `{{text}}` - multi-line text
*   `//` - line comment in schema
*   tabs show the scopes of `// for` statements; they should not be included in the Markdown output.

### Version 2.0.0
    
    // Every cmdlet help placed in it's own `Command-Name.md` file in one folder.
    // We sometimes reference to this folder as "HelpModule".

    // Top-level metadata. You can put your own "key: value" statements there
    // unknown values would be ignored by platyPS
    // You can query this data from markdown file with `Get-MarkdownMetadata`
    //
    // Keys that have meaning for platyPS have separate entries
    ---
    schema: 2.0.0
    external help file: {file name for `New-ExternalHelp`}.xml
    online version: {url for `Get-Help -Online`}
    applicable: {comma-separated list of tags where this cmdlet exists} // if omitted then applicable for any tag
    {{ User-specific key-value pairs }}
    ---

    # {Command name}

    // following level-2 headers sections can go in any order
    // here is the recommended order
    
    ## SYNOPSIS
    {{Synopsis text}}

    ## SYNTAX
    // for each parameter set
        ### {Parameter Set Name, if default parameter set, followed by "(Default)"}
        // i.e.: FromPath (Default)
        // This syntax would be ignored during maml generation.
        // syntax would be generated from parameters metadata
        ```
        {{Output of Get-Command -Syntax}}
        ```

    ## DESCRIPTION
    {{Description text}}

    ## EXAMPLES
    // for every example
        ### {Example Name}

        {{Example introduction text}}
        
        // one or more times, codesnippet
        // it's useful to put the ```powershell code
        // before the plain text command exectution output
            ```{Syntax language, i.e. PowerShell or nothing for plain text}
            {{Example body}}
            ```
        
        {{Example remarks}} // not a mandatory, i.e. TechNet articles don't use remarks

    ## PARAMETERS

    // for every parameter
        // default value is non-mandatory
        ### -{Parameter name}
        {{Parameter description text. It can also include codesnippets, but they could not be ```yaml}}

        // parameter metadata
        // for every unique parameter metadata set 
        // Note: two Parameter Sets can have the same parameter as mandatory and non-mandatory
        // then we put them in two yaml snippets.
        // If they have the same metadata, we put them in one yaml snippet.
            ```yaml // this gives us key/value highlighting
            Type: {Parameter type}  // can be ommitted, then default assumed
            Parameter sets: {comma-separated list of names, i.e. "SetName1, SetName2" or "(All)" for all parameter sets}
            Aliases: {comma-separated list of aliases, i.e. EA, ERR} // if ommitted => default
            Accepted values: {ValidateSet, comma-separated list of valid values, i.e. Foo, Bar} // if ommitted => everything is accepted
            Applicable: {comma-separated list of tags where this cmdlet exists} // if omitted then applicable for any tag
            // break line to improve readability and separate metadata block
                                    
            Required: {true | false}
            Position: {1..n} | named
            Default value: {None | False (for switch parameters) | the actual default value}
            Accept pipeline input: {false | true (ByValue, ByPropertyName)}
            Accept wildcard characters: {true | false}
            ```
        // if supports workflow parameters
        ### <WorkflowCommonParameters>
        {{ Workflow common parameters text, would be ingored during maml generation }}

        // if supports common parameters
        ### <CommonParameters>
        {{ Common parameters text, would be ingored during maml generation }}

    ## INPUTS
    // for every input type
        ### {Input type}
        {{Description text}}

    ## OUTPUTS
    // for every output type
        ### {Output type}
        {{Description text}}

    ## RELATED LINKS

    // for every link
        [{link name}]({link url})

### Version 1.0.0 (Deprecated)
v0.7.6 is the last platyPS version that supports it.

    // for every command:
        # {Command name}
    
        // following level-2 headers sections can go in any order
        // here is the recommended order
    
        ## SYNOPSIS
        {{Synopsis text}}

        ## DESCRIPTION
        {{Description text}}

        ## PARAMETERS

        // for every parameter
            // type and default value are non-mandatory
            ### {Parameter name} [{Parameter type}] = {Parameter default value}

            // parameter metadata
            ```powershell
            {{Parameter attributes as specified in param() block in PowerShell functions
            i.e. [Parameter(ParameterSetName = 'ByName')]
            }}
            ```

            {{Parameter description text}}

        ## INPUTS
        // for every input type
            ### {Input type}
            {{Description text}}

        ## OUTPUTS
        // for every output type
            ### {Output type}
            {{Description text}}

        ## EXAMPLES
        // for every example
            ### {Example Name}

            ```powershell
            {{Example body}}
            ```
            {{Example text explanation}}

        ## RELATED LINKS

        // for every link
            [{link name}]({link url})
