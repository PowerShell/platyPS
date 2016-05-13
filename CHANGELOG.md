CHANGELOG
-------------

## Not released

*   Schema 2.0.0
*   Cmdlets renamed: `platyPS` prefix removed from the names.
*   Removed APIs:
    -   `Get-ModuleFromMaml` (private now)
    -   `New-Markdown` always produces one md file per cmdlet in `-OutputFolder`.
*   New APIs:    
    -   `New-ExternalHelpCab` - create updatable help Cab files
    -   `Get-MarkdownMetadata` - get a hashtable with yaml metadata (new in schema 2.0.0) from the markdown file
    -   `Update`
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
*   Commands: `Get-PlatyPSExternalHelp`, `Get-PlatyPSMarkdown`, `Get-ModuleFromMaml`
*   Schema: Version 1.0.0
