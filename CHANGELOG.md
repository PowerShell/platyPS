CHANGELOG
-------------

## Not released

(Capture your changes here)

## 0.4.0

*   Schema 2.0.0
*   Layout of generated files - one file per cmdlet.
*   Cmdlets renamed: `platyPS` prefix removed from the names.
*   Removed APIs:
    -   `Get-ModuleFromMaml`
    -   `New-MarkdownHelp` always produces one md file per cmdlet in `-OutputFolder`.
*   New APIs:    
    -   `New-ExternalHelpCab` - create updatable help Cab files.
    -   `Get-MarkdownMetadata` - get a hashtable with yaml metadata (new in schema 2.0.0) from the markdown file
    -   `Update-MarkdownHelp`, `Update-MarkdownHelpModule` - update your markdown with actual parameters metadata using reflection. 
    -   `Update-MarkdowhHelpSchema` - migrate from schema version 1.0.0 to schema version 2.0.0
    -   `Get-HelpPreview` - get preview of help from Maml file. Returns help objects.

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
