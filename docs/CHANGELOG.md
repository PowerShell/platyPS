# CHANGELOG

## Not released

## 1.0.0-preview1

- Total rewrite of the module in C#

## 0.14.2

- Add HelpInfoUri for platyPS module
- CI changes to exclude build on v2 branch
- Fix issues with module and external help file metadata
  - Escape # in output ype
  - Allow hyphen in module name
  - Make sure for script module .psm1 is added to the external help file metadata for nested module
- New-Yaml help generates YAML for common parameters

## 0.14.1

- Fix appveyor build script
  - Update-Help for selected modules only
  - Pin Pester to version 4.10.1
- Add makecab directive to set size limit to `CDROM`

## 0.14.0

* Fix passing `ExcludeDontShow` flag to modules (#459)
* Make `CommonParameters` text use proper link format (#449)
* Modified `GetSchemaVersion` function to assume `2.0.0` if no schema version found (#452)
* Fix errors for `New-MarkdownHelp` `ps1` input (#450)

## 0.13.0

* Fix `Update-MarkdownHelp` to not rewrite files if there are not updates (#412) (Thanks @iricigor)
* Add `-ModulePagePath` parameter to `New-MarkdownHelp` for better placement of generated file (#416) (Thanks @PrzemyslawKlys)
* Remove doubled blank lines and trailing spaces in docs (#425) (Thanks @it-praktyk)
* Add Localized Data support for strings in `platyPS.psm1` (#426) (Thanks @tnieto88)
* Add tags to module manifest to improve search on gallery (#428) (Thanks @it-praktyk)
* Fix `Update-MarkdownHelp` to respect order of parameters (#423) (Thanks @it-praktyk)
* Fix `Update-MarkdownHelpModule` to respect `-Force` parameter (#438) (Thanks @it-praktyk)
* Fix for Common and Workflow parameters autowrap (#431) (Thanks @PrzemyslawKlys)
* Do not generate help content for parameters with `DontShow` attribute if `-ExlcudeDontShow` switch is used (#445) (Thanks @MiYanni)

## 0.12.0

* Include proper escaping handing for `\*` and `\_` (#398)
* Update YamlDotNet dependency to 5.0.0 (CVE-2018-1000210)
* Add New-ExternalHelpCab File Type Check (#403)
* Add `Update-MarkdownHelpModule -ModulePagePath` parameter (#397)

## 0.11.1

* Fix the incorrect metadata retrieval for the single parameter sets in remote sessions (#399).

## 0.11.0

* Add default descriptions for paging parameters (#392).
* Fix `yaml` new-line bug (#381).
* Add `-UpdateInputOutput` parameter (#383).
* Add `-UseFullName` parameter to Update-MarkdownHelpModule (#380)

## 0.10.2

* Fix remote module support to work correctly with parameter sets with no parameters.

## 0.10.1

* Fix remote module support for PowerShell version 3.0 and 4.0.
* Avoid doubling `about_` when user provide it in the `AboutName` parameter.

## 0.10.0

* Add basic deduplication for Examples in `Merge-MarkdownHelp` [#331](https://github.com/PowerShell/platyPS/issues/331)
* Support for remote modules. Pass `-Session` parameter in supported cmdlets to retrieve accurate metadata from the remote session.
* The ShowProgress parameter was added to the New-ExternalHelp function. By default progress bars are not display to increase speed of files processing.

## 0.9.0

* platyPS is now cross platform and works on powershell 6.0 as well as previous versions of powershell.

## 0.8.4

* Clean up trailing whitespace during markdown generation [#225](https://github.com/PowerShell/platyPS/issues/225)
* Preserve line breaks after headers when `Update-MarkdownHelp` is used [#319](https://github.com/PowerShell/platyPS/issues/319)
* MAML generation now pads example titles with dash (-) to improve readability [#119](https://github.com/PowerShell/platyPS/issues/119)
* Fixed issue with `New-MarkdownHelp` preventing CommonParameter being added for advanced functions and workflows [#223](https://github.com/PowerShell/platyPS/issues/223) [#253](https://github.com/PowerShell/platyPS/issues/253)
* Fixed issue with `Update-MarkdownHelp` removing named info-string from code fence [#318](https://github.com/PowerShell/platyPS/issues/318)

## 0.8.3

* New-ExternalHelp support 'about' help topics. (Thanks @jazzdelightsme)
* Improve first build experience. (Thanks @jazzdelightsme)
* Fixed spellings and casing in developer notes. (Thanks @DarqueWarrior)
* Added ErrorLogFile parameter to New-ExternalHelp cmdlet. (Thanks @DarqueWarrior)
* Added -MaxAboutWidth parameter to New-ExternalHelp cmdlet. (Thanks @jazzdelightsme)
* Use ToString to get full type name for parameters. (Thanks @yishengjin1413)

## 0.8.2

* Help content for Merge-MarkdownHelp is updated.
* Added New-YamlHelp cmdlet to convert Markdown help into YAML format.

## 0.8.1

* New-MarkdownHelp and Update-MarkdownHelp now support -UseFullTypeName parameter.
* MamlRenderer now uses XLinq to serialize MAML rather than a Stack and a StringBuilder.
* Bugfixes for Merge-MarkdownHelp:
  - Sort tags alphabetically
  - Fix -ExplicitApplicableIfAll to work as expected

## 0.8.0

* Removing the auto increment of help version [#269](https://github.com/PowerShell/platyPS/issues/269)
* Schema version v1 is not longer supported. Update-MarkdownHelpSchema cmdlet removed.
* Enhanced error reporting for New-ExternalHelp [#270](https://github.com/PowerShell/platyPS/issues/270)
* New schema feature: applicable tag [#273](https://github.com/PowerShell/platyPS/issues/273)
  - You can now combine different version / flavours of the same cmdlet into a single markdown file.
  - Applicable entry in yaml metadata controls what is applicable for each module on two levels: cmdlet and parameter.
  - New cmdlet [Merge-MarkdownHelp](https://github.com/PowerShell/platyPS/blob/master/docs/Merge-MarkdownHelp.md)
* Fix WildcardSupport for -Path parameter for multiply comments in our help.

## 0.7.6

* Updated artifact name to 0.7.6

## 0.7.5

* Updated New-ExternalHelpCab.md for removing duplicate word in synopsis.
* Fixed XML_PREAMBULA to have msh namespace, so provider help works.
* Updated the names of the generated cab and zip files so that they have correct case sensitivity.

## 0.7.4

* Add path to md file to the error message [Issue #237](https://github.com/PowerShell/platyPS/issues/237)
* Allow multiple code snippets in the examples
* Allow schema to have non-yaml codesnippets in the Parameter as part of Description [Issue #239](https://github.com/PowerShell/platyPS/issues/239)

## 0.7.3

* Fix about topic naming pattern [Issue #235](https://github.com/PowerShell/platyPS/issues/235)

## 0.7.2

* Enhanced Logging and Error trapping for Update Scenarios [Issue #214](https://github.com/PowerShell/platyPS/issues/214)

## 0.7.1

* Improve blank-line padding in `New-MarkdownHelp` and `Update-MarkdownHelp`. [Issue #210](https://github.com/PowerShell/platyPS/issues/210)
* Auto-generate default descriptions for `-Confirm` and `-WhatIf` parameters in `New-MarkdownHelp`. [Issue #211](https://github.com/PowerShell/platyPS/issues/211)

## 0.7.0

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
