# Schema

PlatyPS requires you to keep the content in a specific structure and Markdown notation. Any authoring must not break this formatting or the MAML will not be generated correctly.
It closely resembles output of `Get-Help`.

## Legend

*   `{string}` - single-line string value
*   `{{text}}` - multi-line text
*   `//` - line comment in schema
*   tabs show the scopes of `// for` statements; they should not be included in the Markdown output.

### Version 1.0.0

    // for every command:
        # {Command name}

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

