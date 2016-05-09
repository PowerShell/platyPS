CHANGELOG
-------------

## Not released

*   Schema 2.0.0
*   Removed APIs:
    -   `New-PlatyPSModuleFromMaml` (private now)
*   New APIs:    
    -   `New-PlatyPSCab` - create updatable help Cab files
    -   `Get-PlatyPSYamlMetadata` - get a hashtable with yaml metadata (new in schema 2.0.0) from the markdown file
*   Generate one file per cmdlet is not the default

## 0.3.1

*   Fix issue that cause error in CI scenario https://github.com/PowerShell/platyPS/issues/27

## 0.3.0

*   Add Get-PlatyPSTextHelpFromMaml command to simplify resulted help review

## 0.2.0

*   Better support for working with big modules:
    *   `Get-PlatyPSMarkdown -OneFilePerCommand -OutputFolder`
    *   `Get-PlatyPSExternalHelp -MarkdownFolder`

## 0.1.0

*   Initial release
*   Commands: `Get-PlatyPSExternalHelp`, `Get-PlatyPSMarkdown`, `New-PlatyPSModuleFromMaml`
*   Schema: Version 1.0.0
