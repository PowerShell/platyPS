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
