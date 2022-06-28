This the current layout of the markdown for a Cmdlet with notes:

YAML Frontmatter

- Need to not error on PlatyPS required keys (external help file, Locale, Module Name, ms.date,
  online version, schema, title)
- What should we do about description? - currently copied by OPS from Description below
- Mapping to existing JSON schema: properties/metadata

Title

- Type: H1
- Required: Y
- Count: 1
- Mapping to existing JSON schema: properties/name

Synopsis

- Type: H2
- Required: Y
- Count: 1
- Mapping to existing JSON schema: properties/summary

Syntax

- Type: H2
- Required: Y
- Count: 1
- Required: No changes
- Mapping to existing JSON schema: properties/syntaxes
- Notes: Contains 1 or more of the following:
  - Parameter Set name
    - Type: H3
    - Required: Y
    - Count: 1-N
    - Parameter Sets - should be sorted starting with Default, then by Alpha
- Parameters (in syntax block)
  - Syntax block
    - What format do you want? - Currently MAML2Yaml is using the strings from the code block instead
      of a Yaml data model. Prefer the string model.

Description

- Type: H2
- Required: Y
- Count: 1
- Mapping to existing JSON schema: properties/description
- Allow any markdown content type except Headers

Examples

- Type: H2
- Required: Y
- Count: 1
- Mapping to existing JSON schema: properties/examples
- Notes: Contains 1 or more of the following
  - Example
    - Type: H3
    - Required: Y
    - Count: 1-N
    - Should require one code block at minimum per example
    - Should allow a mix of markdown code blocks and paragraphs in any order

Parameters

- Type: H2
- Required: Y
- Count: 1
- Mapping to existing JSON schema: properties/parameters
- Parameters should be sorted Alphabetically followed by `-Confirm` and `-WhatIf` (when included).
  PlatyPS has the `-AlphabeticParamsOrder` parameter that produces this order.
- Notes: Contains 1 or more of the following
  - Parameter
    - Type: H3
    - Required: Y
    - Count: 1-N
    - Required: Yaml block should include:
      - Parameter Set Name
      - Position can be different per parameter set
      - Add **AcceptedValues** - Should display enumerated values of parameter (like ValidateSet)
        - maps to properties/parameters/parameterValueGroup?
      - Add **ConfirmImpact** - Impact severity should be reflected and displayed to inform defaults
        for `-Confirm`
      - what is **valueFrom** in JSON schema?
      - **pipelineInput** is currently a boolean. The value can be different per parameter set to
        the type needs to be a string

Common Parameters

- Type: H3
- Required: Y
- Count: 1
- Notes:
  - this is missing from the schema - need to define it
  - The content should be the markdown description provided in the file

Inputs

- Type: H2
- Required: Y
- Count: 1
- Mapping to existing JSON schema: properties/inputs
- Notes: Contains 0 or more of the following:
  - Input type
    - Type: H3
    - Required: N
    - Count: 0-N

Outputs

- Type: H2
- Required: Y
- Count: 1
- Mapping to existing JSON schema: properties/outputs
- Notes: Contains 0 or more of the following:
  - Output type
    - Type: H3
    - Required: N
    - Count: 0-N

Notes

- Type: H2
- Required: N
- Mapping to existing JSON schema: properties/notes
- Count: 1 header with 0 or more markdown elements (excluding header types)

Related links

- Type: H3
- Required: Y
- Count: 1
- Mapping to existing JSON schema: properties/links
- Link list should use a bullet list
- Should support markdown paragraphs not just links
