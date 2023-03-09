---
title: PlatyPS 2.0 specification
ms.date: 03/09/2023
---

# PlatyPS 2.0 Update

PlatyPS provides module owners with the ability to write PowerShell External Help in MarkDown.
PlatyPS generates downloadable and updatable help.

## For this update

PlatyPS expects Markdown to be authored in a particular way. We have defined a schema to determine
how parameters are described, where scripts examples are shown, and so on. The schema closely
resembles the existing output format of the `Get-Help` cmdlet in PowerShell. One of the primary
goals of this update is to improve the schema. The goal of 2.0.0 release is to maintain
compatibility while fixing the longstanding issues.

This update includes the following goals:

Priorities

- Fix schema so that it contains all the Help Data we need while maintaining as much
  compatibility as possible with PS 5.1 and Exchange
- Switch markdown engine to markdig
- Fix OPS rendering problems (requires specific changes in PlatyPS and OPS)

After the initial release, we should look at new features and consider whether we fork the tool to a
new no-legacy feature set or try to maintain compatibility going forward (ala PowerShellGet 3.0).

## Motivation

PowerShell external help files have been authored by hand or using complex tool chains and rendered
as MAML XML for use as console help. MAML is cumbersome to edit by hand, and common tools and
editors don't support it for complex scenarios like they do with Markdown. PlatyPS is provided as a
solution for allow documenting PowerShell help in any editor or tool that supports Markdown.

An additional challenge PlatyPS tackles, is to handle PowerShell documentation for complex scenarios
(e.g. very large, closed source, and/or C#/binary modules) where it may be desirable to have
documentation abstracted away from the codebase. PlatyPS does not need source access to generate
documentation.

Markdown is designed to be human-readable, without rendering. This makes writing and editing easy
and efficient. Many editors support it (Visual Studio Code, Sublime Text, etc), and many tools and
collaboration platforms (GitHub, Visual Studio Online) render the Markdown nicely.

## Release plan

- Proposed Preview
  - TODO

## Goals/Non-Goals

 Goals:

- Update the Help file schema
- Update the About_ schema
- Update the cmdlet parameters to remove/add improvements
- TODO

Non-goals:

- TODO

## Specification

The proposal is to update the functionality and schema of PlayPS to improve the module owners
experience building and updating Help.

### Schema for cmdlet reference

The following sections outline schema changes for the Help file.

|      Heading       | Level | Required? | Count |                                                                                                                    Description                                                                                                                     |
| ------------------ | ----- | --------- | ----- | -------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| Title              | H1    | Y         | 1     | - No changes                                                                                                                                                                                                                                       |
| Synopsis           | H2    | Y         | 1     | - No changes                                                                                                                                                                                                                                       |
| Aliases            | H2    | Y         | 1     | - New header to support documentation for cmdlet aliases                                                                                                                                                                                           |
| Syntax             | H2    | Y         | 1     | - No changes                                                                                                                                                                                                                                       |
| Parameter Set Name | H3    | Y         | 1-N   | - Parameter Sets - should be sorted starting with Default, then by Alpha<br>- Parameters (in syntax block) - Should be sorted Positional, Required, then by Alpha                                                                                  |
| Description        | H2    | Y         | 1     | - Allow any content type<br>- No Headers H2, H3, H4                                                                                                                                                                                                |
| Examples           | H2    | Y         | 1     | - No changes                                                                                                                                                                                                                                       |
| Example            | H3    | Y         | 1-N   | - Should require one code block at minimum per example<br>- Should not be restricted on elements or size                                                                                                                                           |
| Parameters         | H2    | Y         | 1     | - Parameters Should be sorted Alpha. Currently PlatyPS has a switch to force Alpha versus enumerated. The default is off, please change to On.                                                                                                     |
| Parameter          | H3    | Y         | 1-N   | - Yaml block should include:<br>- Parameter Set Name<br>- AcceptedValues - Should display enumerated values of parameter (like ValidateSet)<br>- ConfirmImpact - Impact severity should be reflected and displayed to inform defaults for -Confirm |
| Common Parameters  | H3    | Y         | 1     | - Change the fwlink to remove local and version                                                                                                                                                                                                    |
| Inputs             | H2    | Y         | 1     | - Add cross reference link to the API reference for the input/output object type<br>- [xref link docs][06]                                                                                                                                         |
| Input type         | H3    | N         | 0-N   |                                                                                                                                                                                                                                                    |
| Outputs            | H2    | Y         | 1     | - Add cross reference link to the API reference for the input/output object type<br>- [xref link docs][06]                                                                                                                                         |
| Output type        | H3    | N         | 0-N   |                                                                                                                                                                                                                                                    |
| Notes              | H2    | N         | 1     | Mandatory for schema but not rendered by OPS if empty                                                                                                                                                                                              |
| Related links      | H3    | Y         | 1     | - Link list should use bullets<br>- PlatyPS needs to support bullets<br>- Should support Text (prose), not just links                                                                                                                              |

#### Updated YAML structure for parameters

The following example shows the markdown for the Yaml metadata of a parameter. The Yaml block
contains new metadata items.

~~~markdown
```yaml
Type: System.Management.Automation.ScopedItemOptions
Parameter Sets: (All)
Aliases:
Required: False
Position: Named
Default value: None
Accept pipeline input: ByValue (False), ByName (False)
Accept wildcard characters: False
Dynamic: True
Providers: Alias, Function
Values from remaining args: False
Do not show: False
Release status: Feature Preview
```
~~~

New metadata

- `Dynamic`
  - Type: boolean
  - Required
- `Providers`
  - Type: string containing one or more provider names (comma-separated)
  - Optional - expected when `Dynamic` is true
- `Values from remaining args`
  - Type: boolean
  - Required
- `Do not show`
  - Type: boolean
  - Required
- `Release status`
  - Type: string representation of one enum value from:
    - 'Preview', 'Feature Preview', 'Experimental', 'Deprecated'
  - Optional

The OPS build pipeline doesn't need to validate these values. Any validation should be done by
PlatyPS.

### Schema updates for cmdlet frontmatter

#### Cmdlet alias metadata

Have a way to document aliases for the cmdlet. Today we only document aliases for parameters. This
is difficult to discover, so don't need `*-MarkdownHelp` cmdlets to create it but the schema needs
to support it. The author can add the information to the YAML frontmatter. For example:

```yaml
aliases:
  all: [dir, gci]
  windows: [ls]
  macOS:
  linux:
```

Add new H2 for documenting aliases.

```markdown
## ALIASES

PowerShell includes the following aliases for `Get-ChildItem`:

- All platforms: `dir`, `gci`
- Windows: `ls`
```

See [alias-prototype][02] for a more complete example.

> [!NOTE]
> The `aliases` metadata is optional. The information will be added by the documentation author. The
> ALIASES H2 section is optional and will be added by the documentation author as necessary. The
> reference pipeline shouldn't try to validate this content or metadata values. The reference
> pipeline should add all aliases to the `displayname` property of the Yaml TOC entry for the
> cmdlet. For more information about the `displayname` property, see the [TOC docs][07].

See [Issue #585][04].

#### Cmdlet platform availability

The purpose of this is to represent the availability of the cmdlet for a platform (win/mac/linux).
Could be one or more. PlatyPS could default to Windows when creating the markdown. The value should
be edited by the author to ensure accuracy.

```yaml
- platforms: [win, macOS, linux]
```

- `platforms`
  - Type: string array containing one or more platform names (win, macOS, linux)
  - Required

The reference build pipeline should enforce the requirement but doesn't need to use the values

#### Cmdlet release status metadata

The purpose of this metadata is to represent the release status of the cmdlet. The value is not
required if the cmdlet is GA. This metadata will be added by the author as necessary.

```yaml
release:
  status  : Preview, Feature Preview, Experimental, Deprecated
  message : Text of a banner message.
```

- `release`
  - Type: string representation of one enum value from:
    - 'Preview', 'Feature Preview', 'Experimental', 'Deprecated'
  - Optional
- `status`
  - Type: string representation of one enum value from:
    - 'Preview' - used for cmdlets that are in preview modules
    - 'Feature Preview' - used for cmdlets that require Feature flags to be enabled to use
    - 'Experimental' - used for cmdlets that are enabled by a PowerShell experimental feature
    - 'Deprecated' - used for cmdlet that will be unsupported after a certain date are removed in a
      future release
  - Required
- `message`
  - Type: string - text of an alert message to be displayed in the documentation
    - The message should be a single sentence or short paragraph. Include end of support dates if
      applicable.
  - Required

The reference build pipeline will use:

- the `status` to select the badge displayed for the status.
- the `message` to render an alert box on the page containing the message

Matt Trilby-Bassett has some [Figma designs][08] showing the badge and alert box.

### Schema for Module file frontmatter

The `Module.md` file is the landing page for a module and is used during conversion to MAML. The
frontmatter for this file will also include the `release` metadata for non-GA modules.

See the [release status metadata][01] described above.

The reference build pipeline will use:

- the `status` to select the badge displayed for the status.
- the `message` to render an alert box on the page containing the message

### Schema for About_ files

The following sections outline schema changes for the About_ file.

|      Heading      | Level | Required? | Count |                                                        Description                                                        |
| ----------------- | ----- | --------- | ----- | ------------------------------------------------------------------------------------------------------------------------- |
| Title             | H1    | Y         | 1     | - Title should be Sentence Case - About Topic<br>- Title meta data 'about_<Topic>' should match the file basename         |
| Short Description | H2    | Y         | 1     | - Should be Sentence Case                                                                                                 |
| Long Description  | H2    | Y         | 1     | - Should be Sentence case<br>- Should allow multiple Long description subtopics<br>- Should support subtopics at H3 or H2 |
| See Also          | H2    | Y         | 1     | - This is required but may be empty                                                                                       |

General notes

- Should be rendered as plain text compatible with `Get-Help`
- `Get-Help` bug:Synopsis
- [Get-Help bug][05]
- Add switch to provide Cabs or Zips. Default: cabs
- Add switch to include markdown Default: off
- About_ schema does not say anything about line wrapping etc.
  - If left as text files, then wrap at 80 columns.
  - But if converted to schema-based line limit isn't a problem (for the future). Still a problem
    for previous versions.

### PlatyPS formatting changes

- General
  - Would like PlatyPS to add a line break after each Markdown block
  - Example - Headers, Code blocks, Lists
  - Bug: `Get-Help`: Requires that the first line of text be immediately following Synopsis header

## PlatyPS Cmdlets and Parameters

PlatyPS includes a set of cmdlets with parameters.

The following section is a review of the cmdlets for changes to the parameters and their defaults.
We need to decide to what cmdlets and features to keep. We should make a list breaking changes for
documentation.

In case of breaking changes, do we need a legacy mode?

- We don't know until we see what all the breaking changes are
- If we make changes that require changes to Get-Help we will need something that targets a older
  versions PowerShell.

#### Get-HelpPreview

|       ParamName        | DefaultValue |                                        Description                                         |
| ---------------------- | ------------ | ------------------------------------------------------------------------------------------ |
| Path                   | None         | Specifies an array of paths of MAML external help files                                    |
| ConvertNotesToList     | False        | Indicates that this cmldet formats multiple paragraph items in the NOTES section as single |
| ConvertDoubleDashLists | False        | Indicates that this cmldet converts double-hyphen list bullets into single-hyphen bullets  |

#### Get-MarkdownMetadata

| ParamName | DefaultValue |                       Description                        |
| --------- | ------------ | -------------------------------------------------------- |
| Path      | None         | Specifies an array of paths of markdown files or folders |
| Markdown  | None         | Specifies a string that contains markdown formatted text |

#### Merge-MarkdownHelp

|        ParamName        |   DefaultValue   |                                      Description                                      |
| ----------------------- | ---------------- | ------------------------------------------------------------------------------------- |
| Encoding                | UTF8 without BOM | Specifies the character encoding for your external help file. Specify a System.Text.E |
| ExplicitApplicableIfAll | False            | Always write out full list of applicable tags. By default cmdlets and parameters that |
| Force                   | False            | Indicates that this cmdlet overwrites an existing file that has the same name         |
| MergeMarker             | '!!! '           | String to be used as a merge text indicator. Applicable tag list would be included    |
| OutputPath              | None             | Specifies the path of the folder where this cmdlet creates the combined markdown help |
| Path                    | None             | Specifies an array of paths of markdown files or folders. This cmdlet creates         |

#### New-ExternalHelp

|   ParamName   |   DefaultValue   |                                           Description                                           |
| ------------- | ---------------- | ----------------------------------------------------------------------------------------------- |
| OutputPath    | None             | Specifies the path of a folder where this cmdlet saves your external help file. The folder name |
| Encoding      | UTF8 without BOM | Specifies the character encoding for your external help file. Specify a System.Text.Encoding    |
| Force         | False            | Indicates that this cmdlet overwrites an existing file that has the same name                   |
| Path          | None             | Specifies an array of paths of markdown files or folders. This cmdlet creates external help     |
| ApplicableTag | None             | Specify array of tags to use as a filter. If cmdlet has `applicable` in the yaml metadata       |
| MaxAboutWidth | 80               | Specifies the maximimum line length when generating "about" help text files.                    |
| ErrorLogFile  | None             | The path where this cmdlet will save formatted results log file                                 |
| ShowProgress  | False            | Display progress bars under parsing existing markdown files                                     |

#### New-ExternalHelpCab

|      ParamName       | DefaultValue |                                         Description                                         |
| -------------------- | ------------ | ------------------------------------------------------------------------------------------- |
| CabFilesFolder       | None         | Specifies the folder that contains the help content that this cmdlet packages into a .cab   |
| LandingPagePath      | None         | Specifies the full path of the Module Markdown file that contains all the metadata required |
| OutputFolder         | None         | Specifies the location of the .cab file and helpinfo.xml file that this cmdlet creates      |
| IncrementHelpVersion | False        | Automatically increment the help version in the module markdown file                        |

#### New-MarkdownAboutHelp

|  ParamName   | DefaultValue |                Description                 |
| ------------ | ------------ | ------------------------------------------ |
| AboutName    | None         | The name of the about topic                |
| OutputFolder | None         | The directory to create the about topic in |

#### New-MarkdownHelp

|       ParamName        |   DefaultValue   |                                       Description                                       |
| ---------------------- | ---------------- | --------------------------------------------------------------------------------------- |
| Command                | None             | Specifies the name of a command in your current session. This can be any command        |
| Encoding               | UTF8 without BOM | Specifies the character encoding for your markdown help files                           |
| Force                  | False            | Indicates that this cmdlet overwrites existing files that have the same names           |
| FwLink                 | None             | Specifies the forward link for the module page. This value is required for .cab         |
| HelpVersion            | None             | Specifies the version of your help. This value is required for .cab file creation       |
| Locale                 | None             | Specifies the locale of your help. This value is required for .cab file creation        |
| MamlFile               | None             | Specifies an array of paths path of MAML .xml help files                                |
| Metadata               | None             | Specifies metadata that this cmdlet includes in the help markdown files as a hash table |
| Module                 | None             | Specifies an array of names of modules for which this cmdlet creates help in markdown   |
| ModuleGuid             | None             | Specifies the GUID of the module of your help. This value is required for .cab file     |
| ModuleName             | None             | Specifies the name of the module of your help. This value is required for .cab file     |
| NoMetadata             | False            | Indicates that this cmdlet does not write any metadata in the generated markdown        |
| OnlineVersionUrl       | None             | Specifies the URL where the updatable help function downloads updated help              |
| OutputFolder           | None             | Specifies the path of the folder where this cmdlet creates the markdown help files      |
| WithModulePage         | False            | Indicates that this cmdlet creates a module page in the output folder                   |
| ConvertNotesToList     | False            | Indicates that this cmldet formats multiple paragraph items in the NOTES section        |
| ConvertDoubleDashLists | False            | Indicates that this cmldet converts double-hyphen list bullets into single-hyphen       |
| AlphabeticParamsOrder  | False            | Order parameters alphabetically by name in PARAMETERS section. There are 5 exceptions   |
| UseFullTypeName        | False            | Indicates that the target document will use a full type name instead of a short name    |
| Session                | None             | Provides support for remote commands. Pass the session that you used to create          |
| ModulePagePath         | None             | When WithModule parameter is used by default it puts .md file in same location as all   |
| ExcludeDontShow        | False            | Exclude the parameters marked with `DontShow` in the parameter attribute from the help  |

#### New-YamlHelp

|  ParamName   | DefaultValue |                                             Description                                             |
| ------------ | ------------ | --------------------------------------------------------------------------------------------------- |
| Encoding     | None         | Specifies the character encoding for your external help file. Specify a System.Text.Encoding object |
| Force        | False        | Indicates that this cmdlet overwrites an existing file that has the same name                       |
| Path         | None         | Specifies an array of paths of markdown files or folders                                            |
| OutputFolder | None         | Specifies the folder to create the YAML files                                                       |

#### Update-MarkdownHelp

|       ParamName       |   DefaultValue   |                                     Description                                      |
| --------------------- | ---------------- | ------------------------------------------------------------------------------------ |
| Encoding              | UTF8 without BOM | Specifies the character encoding for your markdown help files                        |
| LogAppend             | False            | Indicates that this cmdlet appends information to the log instead overwriting it     |
| LogPath               | None             | Specifies a file path for log information. The cmdlet writes the VERBOSE stream      |
| Path                  | None             | Specifies an array of paths of markdown files and folders to update                  |
| AlphabeticParamsOrder | False            | Order parameters alphabetically by name in PARAMETERS section                        |
| UseFullTypeName       | False            | Indicates that the target document will use a full type name instead of a short name |
| Session               | None             | Provides support for remote commands. Pass the session that you used to create       |
| UpdateInputOutput     | False            | Refreshes the Input and Output section to reflect the current state of the cmdlet    |
| Force                 | False            | Remove help files that no longer exists within sessions                              |
| ExcludeDontShow       | False            | Exclude the parameters marked with `DontShow` in the parameter attribute             |

#### Update-MarkdownHelpModule

|       ParamName       |   DefaultValue   |                                      Description                                       |
| --------------------- | ---------------- | -------------------------------------------------------------------------------------- |
| Encoding              | UTF8 without BOM | Specifies the character encoding for your markdown help files                          |
| LogAppend             | False            | Indicates that this cmdlet appends information to the log instead overwriting          |
| LogPath               | None             | Specifies a file path for log information. The cmdlet writes the VERBOSE stream        |
| Path                  | None             | Specifies an array of paths of markdown folders to update. The folder must contain     |
| RefreshModulePage     | False            | Update module page when updating the help module                                       |
| AlphabeticParamsOrder | False            | Order parameters alphabetically by name in PARAMETERS section                          |
| Session               | None             | Provides support for remote commands. Pass the session that you used to create         |
| UseFullTypeName       | False            | Indicates that the target document will use a full type name instead of a short name   |
| UpdateInputOutput     | False            | Refreshes the Input and Output sections to reflect the current state of the cmdlet     |
| ModulePagePath        | None             | When -RefreshModulePage is used by default it puts .md file in same location           |
| Force                 | False            | Remove help files that no longer exists within sessions                                |
| ExcludeDontShow       | False            | Exclude the parameters marked with `DontShow` in the parameter attribute from the help |

## Cmdlet Feature requests

1. I was wondering if we should default to zips instead, one package for both platforms. Cabs as
   legacy for < 5.1 content. We can fix PS 7.2 to look for zip first and fallback to cab. Add as
   question in spec — This should be a switch - cab, zip or both.

1. Need to know about Merge-MarkdownHelp which uses `applicable:` yaml syntax for version.
   `applicable:` should be mentioned in the schema. — this creates tags…for linux/windows
   parameters. Exchange uses the `applicable:` feature heavily. Need to maintain support for them.

1. Need way to increment the help version number of the CAB files

   - Could be automatic in every build?
   - Could be scheduled?
   - Could be a workflow step (scripted) done by the author
   - Need to figure out limits of version number fields
   - Need rules for when to increment major, minor, subminor, etc. (Semvar?)

### List of things that break with PS5.1 or Exchange

TODO

## Issues for this milestone

[2.0-consider][03]

<!-- link references -->
[01]: #cmdlet-release-status-metadata
[02]: alias-prototype.md
[03]: https://github.com/PowerShell/platyPS/issues?q=is%3Aopen+is%3Aissue+milestone%3A2.0-consider
[04]: https://github.com/PowerShell/platyPS/issues/585
[05]: https://github.com/PowerShell/PowerShell/issues/9208
[06]: https://review.docs.microsoft.com/en-us/help/contribute/links-how-to?branch=master#xref-cross-reference-links
[07]: https://review.learn.microsoft.com/en-us/help/platform/navigation-toc-overview?branch=main#yaml-toc-format
[08]: https://www.figma.com/file/EP3wMMct92SqHwvWcorb9O/%5BDiscovery%5D-Reference-Development-Status-Information?node-id=897%3A3314&t=zy1ygL7Gh5ChSyh1-0
