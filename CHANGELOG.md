CHANGELOG
-------------

## Not released

(Capture your changes here)

## 0.5.0

This release focuses on stability and usability.
Special thanks to [@TimShererAtAquent](https://github.com/TimShererAtAquent) for the thoughful feedback.

#### New
* normalize all the things:
  - casing in the parameters metadata section [#122](https://github.com/PowerShell/platyPS/issues/122)
    (i.e. `Accepts pipeline input: true` -> `Accepts pipeline input: True`)
  - Em-dashes, smart quotes are replaced by regular ones [#127](https://github.com/PowerShell/platyPS/issues/127)
  - Unicode whitespaces [#113](https://github.com/PowerShell/platyPS/issues/113)
* Enhance your help to leverage markdown:
  - `New-MarkdownHelp -ConvertNotesToList` creates bullet list for NOTES section. [#125](https://github.com/PowerShell/platyPS/issues/125)
  - `New-MarkdownHelp -ConvertDoubleDashLists` turns you double-dash lists into markdown-compatible single-dash lists. [#117](https://github.com/PowerShell/platyPS/issues/117)
* Default parameter set marker in SYNTAX [#107](https://github.com/PowerShell/platyPS/issues/107)
* Online version (for `Get-Help -Online`) got it's own place in markdown metadata header [#123](https://github.com/PowerShell/platyPS/issues/123)
* `(` and `)` are not escaped in the generated markdown anymore.
* If SYNOPSIS not present, generate placeholder for it, instead of a duplicate for SYNTAX [#110](https://github.com/PowerShell/platyPS/issues/110)

#### Bugfixes

* `Update-MarkdownHelp` doesn't add extra-lines anymore.
* Bold or italic doesn't strip spaces anymore [#130](https://github.com/PowerShell/platyPS/issues/130)
* `platyPS.psm1` doesn't use aliases anymore [#126](https://github.com/PowerShell/platyPS/issues/126)
* Parameter `HelpMessage` attribute can be used to generate Parameter description in markdown [#115](https://github.com/PowerShell/platyPS/issues/115)
* No unwanted markdown files for aliases anymore [#114](https://github.com/PowerShell/platyPS/issues/114)

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
*   Tab-completion support by [@dotps1](https://github.com/dotps1)
    -    Requires PS v5 or [TabExpansionPlusPlus](https://github.com/lzybkr/TabExpansionPlusPlus)
    -    Provides complitions for module names in `New-ExternalHelp -Module <Tab>`

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
