CHANGELOG
-------------

## Not released

(Capture your changes here)

#### New
* Update markdown parser to handle underscore version of italics and bold [Issue #176](https://github.com/PowerShell/platyPS/issues/176)
* Cab creation the help version metadata is incremented prior to cabbing. IE: 1.0.0.0 -> 1.0.0.1
* Update markdown will now check the external help file name and update to correct value.
* Update-MarkdownHelpModule new switch -RefreshModulePage, update the module page with cmdlet synopsis and preserves the module description field.
* Add -AlphabeticParamsOrder parameter to `New-MarkdownHelp`, `Update-MarkdownHelp` and `New-MarkdownHelpModule`.
  When specified orders parameters alphabetically by name in PARAMETERS section.

#### Bug Fixes
* Resolved issue where about topic second header was case sensitive. Now insensitive. [Issue #174](https://github.com/PowerShell/platyPS/issues/174)
* About topics will be named with about_<Topic Name> when converted to txt using New-ExternalHelp. Enhancement suggested in [Issue #174](https://github.com/PowerShell/platyPS/issues/174)
* If PowerShell session is in a non-file system provider, PlatyPS will switch to a file system provider. Preference C: drive. [Issue #161](https://github.com/PowerShell/platyPS/issues/161)
* Updates to Pester-Tests to fix issues occuring when updatable-help changes
* Default parameter values should be 'none' not 'false' [Issue #167](https://github.com/PowerShell/platyPS/issues/167)
* Updated New-MarkdownAboutHelp to name the about file about_<AboutTopicName>.md instead of <AboutTopicName>.md. [Issue #177](https://github.com/PowerShell/PlatyPS/issues/177)

## 0.6.1

#### New
* Forcing metadata in md files to be sorted alphabetically by key.

## 0.6.0

#### New
* About Topics Support
  - New Cmdlet New-MarkdownAboutHelp generates a MD of a blank about topic MD file from a template file.
  - The About MD file template can be customized. The template is stored in the templates folder in PlatyPS
  - New-ExternalHelp will now transform About MD files into About Txt files.

#### Bug Fixes
* Update-MarkdownHelp Log updated to improve readability.
* You can use relative file paths for related links now. [#164](https://github.com/PowerShell/platyPS/issues/164)

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
