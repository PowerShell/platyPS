# Schema

PlatyPS requires you to keep the content in a specific structure and Markdown notation. Any authoring must not break this formatting or the MAML will not be generated correctly.
It closely resembles output of `Get-Help`.

## Legend

*   `{string}` - single-line string value
*   `{{text}}` - multi-line text
*   `//` - line comment in schema
*   tabs show the scopes of `// for` statements; they should not be included in the Markdown output.

### Version 1.1.0

    // for every command:
        # {Command name}

        ## SYNOPSIS
        {{Synopsis text}}

        ## SYNTAX
        // for each parameter set
            {{Output of Get-Command -Syntax}}

        ## DESCRIPTION
        {{Description text}}

        ## PARAMETERS

        // for every parameter
            // default value is non-mandatory
            ### {Parameter name} [{Parameter type}] = {Parameter default value}

            {{Parameter description text}}

            // parameter metadata
            ```yaml // this gives us key/value highlighting
            Required: {true | false}
            Position: {1..n}
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

