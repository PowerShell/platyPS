# Schema

PlatyPS requires you to keep the content in a specific structure and Markdown notation. Any authoring must not break this formatting or the MAML will not be generated correctly.
It closely resembles output of `Get-Help`.

## Legend

*   `{string}` - single-line string value
*   `{{text}}` - multi-line text
*   `//` - line comment in schema
*   tabs show the scopes of `// for` statements; they should not be included in the Markdown output.

### Version 2.0.0 (not implemented yet. [Discussion](https://github.com/PowerShell/platyPS/issues/20))

    // top-level metadata. You can put your own "key: value" statements there
    // unknown values would be ignored by platyPS
    // You can query this data from markdown file with `Get-PlatyPSYamlHeader`
    ---
    schema: 2.0.0
    ---

    // for every command:
        # {Command name}
    
        // following level-2 headers sections can go in any order
        // here is the recommended order
        
        ## SYNOPSIS
        {{Synopsis text}}

        ## SYNTAX
        // for each parameter set
            ### Parameter Set Name
            ```
            {{Output of Get-Command -Syntax}}
            ```

        ## DESCRIPTION
        {{Description text}}

        ## EXAMPLES
        // for every example
            ### {Example Name}

            {{Example introduction text}}
            
            ```powershell
            {{Example body}}
            ```
            
            {{Example remarks}} // not a mandatory, i.e. TechNet articles don't use remarks

        ## PARAMETERS

        // for every parameter
            // default value is non-mandatory
            ### {Parameter name}

            {{Parameter description text}}

            // parameter metadata
            // for every unique parameter metadata set 
            // Note: two Parameter Sets can have the same parameter as mandatory and non-mandatory
            // then we put them in two yaml snippets.
            // If they have the same metadata, we put them in one yaml snippet.
                ```yaml // this gives us key/value highlighting
                Type: {Parameter type}  // can be ommitted, then default assumed
                Parameter sets: {comma-separated list of names, i.e. "SetName1, SetName2"} // if ommitted => default
                Aliases: {comma-separated list of aliases, i.e. EA, ERR} // if ommitted => default
                Accepted values: {ValidateSet, comma-separated list of valid values, i.e. Foo, Bar} // if ommitted => everything is accepted
                // break line to improve readability and separate metadata block
                                        
                Required: {true | false}
                Position: {1..n} | named
                Default value: {None | False (for switch parameters) | the actual default value}
                Accept pipeline input: {false | true (ByValue, ByPropertyName)}
                Accept wildcard characters: {true | false}
                ```

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

### Version 1.0.0

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
