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
Is credential: False
Is obsolete: False
Release status: Feature Preview
```
~~~

Notes

- Values for most items should be obtained by reflection.
- Provider information will have to be tested individually based on the currently loaded providers.
- The `Default value` should be obtained by reflection, if possible.
  - The default value for **SwitchParameter** types should be `False`.
- If possible, the `Parameter sets` and `Providers` values should be simplified to show `(All)` when
  the value is the same for all parameter sets or providers.

New metadata

- `Dynamic`
  - Type: boolean
    -  Obtained by reflection of parameter attributes
  - Required
- `Providers`
  - Type: string containing one or more provider names (comma-separated)
  - Optional - expected when `Dynamic` is true
- `Values from remaining args`
  - Type: boolean
    -  Obtained by reflection of parameter attributes
  - Required
- `Do not show`
  - Type: boolean
    -  Obtained by reflection of parameter attributes
  - Required
- `Is credential`
  - Type: boolean
    -  Obtained by reflection of parameter attributes
  - Required
- `Is obsolete`
  - Type: boolean
    -  Obtained by reflection of parameter attributes
    - Required
- `Release status`
  - Type: string representation of one enum value from:
    - 'Preview' - typically not used for parameters
    - 'Feature Preview' - typically not used for parameters
    - 'Experimental' - used for parameters that are enabled by a PowerShell experimental feature
    - 'Deprecated' - used for cmdlet that will be unsupported after a certain date are removed in a
      future release
    - Null value - indicates the parameter is GA (currently supported)
  - Optional - the value can be null or the key-value pair omitted

The OPS build pipeline should validate that the values match the type. `Release status` should only
be rendered on the page if the key has a value (not null).

### Schema updates for cmdlet frontmatter

#### Cmdlet alias metadata

Have a way to document aliases for the cmdlet. Today we only document aliases for parameters. This
is difficult to discover, so don't need `*-MarkdownHelp` cmdlets to create it but the schema needs
to support it. The author can add the information to the YAML frontmatter. For example:

```yaml
aliases: [dir, gci, ls]
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

The purpose of this metadata is to represent the release status of the cmdlet. The value isn't
required if the cmdlet is GA. This metadata will be added by the author as necessary.

PlatyPS should create the metadata as placeholders in the frontmatter. The author should delete the
keys or update the values as appropriate.

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
    - 'Preview' - used for cmdlets from preview modules where the version is 0.x.y with no
      Prerelease label
    - 'Feature Preview' - used for cmdlets from feature preview modules where the version is 1.x.y
      or higher and has a Prerelease label
    - 'Experimental' - used for cmdlets that are enabled by a PowerShell experimental feature
    - 'Deprecated' - used for cmdlet that will be unsupported after a certain date are removed in a
      future release
  - Required
- `message`
  - Type: string - text of an alert message to be displayed in the documentation
    - The message should be a single sentence or short paragraph. Include end of support dates if
      applicable.
  - Required

The reference build pipeline should do the following:

- Use the `status` to select the badge displayed for the status.
- Use the `message` to render an alert box on the page containing the message
- Validate that the values match the types.

Matt Trilby-Bassett has some [Figma designs][08] showing the badge and alert box.

### Schema for Module file frontmatter

The `Module.md` file is the landing page for a module and is used during conversion to MAML. The
frontmatter for this file will also include the `release` metadata for non-GA modules.

See the [release status metadata][01] described above.

The reference build pipeline will use:

- the `status` to select the badge displayed for the status.
- the `message` to render an alert box on the page containing the message

### PlatyPS formatting changes

- General
  - Would like PlatyPS to add a line break after each Markdown block
  - Example - Headers, Code blocks, Lists
  - Bug: `Get-Help`: Requires that the first line of text be immediately following Synopsis header

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
