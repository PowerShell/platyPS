---
ms.date: 10/22/2024
---
# Microsoft.PowerShell.PlatyPS design notes

## Design notes

### Features/Functionality

- When PlatyPS writes file output, it should create folder structure of module-named folders
  containing cmdlet markdown for the cmdlets in those modules
- PlatyPS should populate the markdown with all the facts that it can reasonably gather about the
  command:
  - Name
  - Syntax of all parameter sets
  - Parameter names and metadata (type, all attributes, etc. arranged by parameter set)
  - Common parameters (if supported)
  - Input types - types that can be piped to the command
  - Output types - as defined by the cmdlet attributes
- PlatyPS should insert placeholder text for all content that must be provided by the author

### Module notes

- Total rewrite in C# to improve performance, object model, and extensibility
  - Cmdlets are focused on the object model and provide a single purpose
  - Chain cmdlets in a pipeline to perform complex operations
  - Workflow style cmdlets will be script-based to improve usability for authors
    - Script-based provide readable code model that allows end-users to customize their own
      workflows as needed
    - Workflows are easier to code in script rather than in C#
- Total breaking change from previous versions
- New module name - Microsoft.PowerShell.PlatyPS
  - Allows you to install side-by-side with previous versions
  - No common cmdlet names between modules
  - No intent to provide proxies for old cmdlets
- Cmdlet names better reflect what they do
  - Import-SomeFormat
  - Export-SomeFormat
- New markdown parsing and rendering powered by markdig
- Module provides Yaml output to be used by the Learn reference pipeline
- Object model is the heart of the new module
  - Establishes a common object model for cmdlets and module files
  - Markdown is the source of truth for the documentation
  - Cmdlets import the markdown in the the CommandHelp object model
  - CommandHelp objects can be exported to Markdown, Yaml, or MAML
  - CommandHelp objects can be imported from Markdown, Yaml, or MAML
  - Conversion to/from MAML is lower fidelity other formats - results in loss of data and accuracy
    of the documentation

### Markdown notes

- Markdown is the source of truth for the documentation
- Markdown has a strict H2/H3 structure
  - You can't add headers or reorder them
  - Not all H2/H3 sections require content. The H2/H3 line is required, but the content is optional.
    This is done to ensure that the markdown structure is consistent with the object model.
- Metadata names have been changed to match PowerShell property names and common terminology.
  - This causes less confusion for authors and makes it easier to troubleshoot validation and
    rendering issues
- Import command can read old (0.14) format or new format and converts Markdown into CommandHelp
  object
- Export commands convert CommandHelp object into other formats
  - Export to markdown command can writes new markdown format
  - Export to Yaml is used by the Learn reference pipeline and reflects the new object model
  - Export to MAML is lower fidelity and results in loss of data and accuracy of the documentation
    due to limitation in Get-Help and the MAML specification
- The Yaml frontmatter is used to store metadata that is not part of the markdown content
  - The object model has a set of required key/value pairs
  - The `document type` and `PlatyPS schema version` keys are inviolate (cannot be changed)
  - You can add custom metadata to the frontmatter
  - Any key/value pairs not managed by PlatyPS are passed through unaltered

### Authoring process

- Authoring is always a manual process requiring human intervention
- PlatyPS helps automate the process and provides a consistent structure for the documentation but
  can't fill in descriptions, examples, or related links
  - PlatyPS inserts placeholder text to show you where to add content
- Authors must review and edit the markdown files to ensure that the content is accurate for every
  authoring step
  - Creating new markdown for new modules/commands
  - Migrating existing markdown to the new format
  - Updating existing markdown based on new versions of modules/commands

### Converting to/from other formats

- Markdown is the source of truth for the documentation
- Converting from the old Markdown format to the new format is a one-way process
  - The new format has more structure and metadata than the old format
  - The new format is more consistent and easier to validate
  - The old format does not reflect all of the correct facts about parameters
    - Converting from the old to the new format does not improve the accuracy of the parameter
      metadata; you must run the update process
- The update process requires that the commands/modules be available in the current session so
   that the complete parameter metadata can be discovered
- Converting to Yaml
  - There is no loss of fidelity when converting from the Markdown to Yaml (or vice versa)
  - The Yaml is just a serialization of the CommandHelp object model
  - All properties are preserved even if the value is null
  - Rendering to HTML should handle null values gracefully (e.g. conditional formatting to omit null
    values)
- Converting to/from MAML
  - There is a loss of fidelity when converting to MAML due to limitations in the MAML specification
    and Get-Help cmdlet
- Importing from MAML is supported as a method of last resort.

## Frontmatter

| FileType | v1 Key             | v1 Status                                           | v2 key                 | v2 Status                                          |
|----------|--------------------|-----------------------------------------------------|------------------------|----------------------------------------------------|
| cmdlet   |                    | n/a                                                 | document type          | Required                                           |
| cmdlet   | external help file | Required - unique to cmdlet                         | external help file     | Required                                           |
| cmdlet   | online version     | Key required, value can be null  - unique to cmdlet | HelpUri                | Key required, value can be null - unique to cmdlet |
| cmdlet   | Locale             | Required                                            | Locale                 | Required                                           |
| cmdlet   | Module Name        | Key required, value can be null                     | Module Name            | Key required, value can be null                    |
| cmdlet   | ms.date            | Optional                                            | ms.date                | Optional                                           |
| cmdlet   | schema             | Required                                            | PlatyPS schema version | Required                                           |
| cmdlet   | title              | Required                                            | title                  | Required                                           |
| cmdlet   |                    | n/a                                                 | aliases                | Optional                                           |
| module   |                    | n/a                                                 | document type          | Required                                           |
| module   | Help Version       | Key required, value can be null - unique to module  | Help Version           | Key required, value can be null - unique to module |
| module   | Download Help Link | Key required, value can be null - unique to module  | HelpInfoUri            | Key required, value can be null - unique to module |
| module   | Locale             | Required                                            | Locale                 | Required                                           |
| module   | Module Guid        | Required - unique to module                         | Module Guid            | Required - unique to module                        |
| module   | Module Name        | Required                                            | Module Name            | Required                                           |
| module   | ms.date            | Optional                                            | ms.date                | Optional                                           |
| module   | schema             | Required                                            | PlatyPS schema version | Required                                           |
| module   | title              | Required                                            | title                  | Required                                           |

## Scenarios to support OPS

1. Workflow script to convert all markdown files to YAML in a folder
1. Workflow script for CabGen
1. Make rendering decisions for HTML presentation

These items to be done by Sean and Jason working with DanniX and team.

## Future ideas

- Create a method to convert CommandHelp object to the Get-Help object model
  - This could be an easy way to ship markdown instead of MAML for downlevel systems that don't have
    or cant support Help v2.
- Add commands to stream conversion to Markdown to support Markdown rendering in the console (e.g.
  pipe to `glow.exe`)
- Create workflow convenience scripts to include in module
- Compare-MarkdownCommandHelp - can't compare module files

