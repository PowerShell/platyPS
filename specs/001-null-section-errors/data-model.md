# Data Model: Null Section Error Handling

## Overview

This document defines the data structures and relationships for null section error handling in platyPS.

## Core Entities

### 1. HelpSection

Represents a section within PowerShell help documentation.

**Properties**:
- `Name` (string): Section identifier (e.g., "SYNOPSIS", "DESCRIPTION", "PARAMETERS")
- `Content` (string): Section content text
- `LineNumber` (int): Starting line number in source file
- `IsRequired` (bool): Whether this section must be present
- `ValidationState` (enum): Null, Empty, Whitespace, Valid

**Validation Rules**:
- Name must not be null or empty
- Content null → ValidationState = Null
- Content empty string → ValidationState = Empty
- Content whitespace-only → ValidationState = Whitespace
- Content has non-whitespace → ValidationState = Valid
- At least one section must have ValidationState = Valid (keyword section requirement)

**State Transitions**:
```
Initial → [Parse] → Null/Empty/Whitespace/Valid
Valid → [Update] → Valid/Null/Empty/Whitespace
```

### 2. ValidationDiagnostic

Represents a validation error or warning for a help section.

**Properties**:
- `Severity` (enum): Error, Warning, Information
- `FilePath` (string): Path to file being validated
- `LineNumber` (int): Line number where issue occurs
- `SectionName` (string): Name of section with issue
- `Message` (string): Formatted error message
- `Guidance` (string): Remediation guidance

**Validation Rules**:
- Severity must be Error for null/empty required sections
- FilePath must not be null or empty
- LineNumber must be > 0
- SectionName must match valid help section names
- Message must follow format: `[{Severity}] {FilePath}:{LineNumber} - {SectionName} is null. Fix: {Guidance}`

**Message Format Template**:
```
[ERROR] file.md:10 - SYNOPSIS is null. Fix: Add SYNOPSIS content.
```

### 3. SectionValidator

Orchestrates validation logic for help sections.

**Properties**:
- `RequiredSections` (List<string>): Section names that must be present
- `OptionalSections` (List<string>): Section names that are optional
- `Diagnostics` (List<ValidationDiagnostic>): Accumulated validation errors
- `ValidatedFileCount` (int): Number of files processed
- `ErrorCount` (int): Total errors found

**Methods**:
- `ValidateSection(HelpSection section)`: Validates a single section
- `ValidateCommandHelp(CommandHelp help)`: Validates all sections in CommandHelp
- `HasRequiredKeywordSection(CommandHelp help)`: Checks at least one keyword section present
- `ReportDiagnostics()`: Outputs diagnostics to PowerShell error stream
- `Reset()`: Clears diagnostics for new validation run

**Validation Logic**:
1. Check if section is null → Create diagnostic
2. Check if section content is empty → Create diagnostic
3. Check if section content is whitespace-only → Create diagnostic
4. Verify at least one keyword section has valid content
5. Add diagnostic with file path, line number, severity, guidance

### 4. Enhanced DiagnosticMessage

Extension of existing `DiagnosticMessage` class to support null section errors.

**New Properties**:
- `LineNumber` (int): Line number in source file
- `SectionName` (string): Section that failed validation
- `Guidance` (string): Actionable fix guidance

**Existing Properties** (preserved):
- `Message` (string): Error message text
- `Severity` (DiagnosticSeverity): Error/Warning/Information
- `Source` (DiagnosticMessageSource): Source file information

**Format Method**:
```csharp
public string FormatStructuredMessage()
{
    return $"[{Severity.ToUpper()}] {Source.FilePath}:{LineNumber} - {SectionName} is null. Fix: {Guidance}";
}
```

## Relationships

```
CommandHelp (existing)
    ↓ contains
    ├─ Synopsis (HelpSection)
    ├─ Description (HelpSection)
    ├─ Parameters (List<ParameterHelp>)
    ├─ Examples (List<ExampleHelp>)
    └─ ... (other sections)

SectionValidator
    ↓ validates
    HelpSection
    ↓ produces
    ValidationDiagnostic
    ↓ aggregates into
    DiagnosticMessage

MarkdownReader/YamlReader
    ↓ creates
    HelpSection (with LineNumber)
    ↓ passes to
    SectionValidator
    ↓ collects
    ValidationDiagnostic
    ↓ emits to
    PowerShell Error Stream
```

## Section Types

### Keyword Sections (Required - at least one)

These are the primary content sections that define the help content:

- **SYNOPSIS**: Brief description of command
- **DESCRIPTION**: Detailed explanation of command functionality
- **EXAMPLES**: Usage examples with explanations

### Structural Sections (Conditional)

Required if cmdlet has corresponding elements:

- **PARAMETERS**: Required if cmdlet has parameters
- **INPUTS**: Describes pipeline input types
- **OUTPUTS**: Describes output types

### Optional Sections

Informational sections that enhance but aren't required:

- **NOTES**: Additional information
- **LINK**: Related links and references
- **COMPONENT**: Component association
- **ROLE**: Role association
- **FUNCTIONALITY**: Functionality description

## Validation Rules Summary

| Condition | Severity | Action |
|-----------|----------|--------|
| No keyword sections present | Error | Block processing, report missing sections |
| Required section is null | Error | Block processing, report with line number |
| Required section is empty | Error | Block processing, report with line number |
| Required section is whitespace-only | Error | Block processing, report with line number |
| Optional section is null | None | Continue processing (no error) |
| All required sections valid | None | Processing succeeds |

## Integration with CommandHelp Object Model

The existing `CommandHelp` class is the central model in platyPS. This feature enhances validation without modifying the core model:

**Existing Structure**:
```csharp
class CommandHelp
{
    public string Synopsis { get; set; }
    public string Description { get; set; }
    public List<ParameterHelp> Parameters { get; set; }
    public List<ExampleHelp> Examples { get; set; }
    // ... other properties
}
```

**Validation Extension** (new):
```csharp
static class CommandHelpValidationExtensions
{
    public static List<ValidationDiagnostic> ValidateSections(this CommandHelp help, string filePath)
    {
        var validator = new SectionValidator();
        
        // Validate keyword sections
        validator.ValidateSection(new HelpSection { 
            Name = "SYNOPSIS", 
            Content = help.Synopsis,
            LineNumber = help.SynopsisLineNumber  // new tracking
        });
        
        validator.ValidateSection(new HelpSection { 
            Name = "DESCRIPTION", 
            Content = help.Description,
            LineNumber = help.DescriptionLineNumber
        });
        
        // Validate at least one keyword section present
        validator.HasRequiredKeywordSection(help);
        
        return validator.Diagnostics;
    }
}
```

## Performance Considerations

**Memory Footprint**:
- `HelpSection`: ~50 bytes per instance
- `ValidationDiagnostic`: ~200 bytes per error
- Typical file: 10-20 sections, 0-5 errors
- 1000 files: <1MB memory for validation data

**Computational Complexity**:
- Section validation: O(1) per section
- Null check: O(1)
- Whitespace check: O(n) where n = content length, but typically small
- Total per file: O(sections) = O(10-20) → negligible

**Streaming Impact**:
- No buffering of all errors (stream as encountered)
- Memory usage constant regardless of file count
- GC pressure minimal (diagnostic objects short-lived)

## Testing Scenarios

### Unit Tests (per entity)

**HelpSection**:
- Null content → ValidationState.Null
- Empty content → ValidationState.Empty
- Whitespace content → ValidationState.Whitespace
- Valid content → ValidationState.Valid

**ValidationDiagnostic**:
- Message format correctness
- Line number inclusion
- Guidance text generation
- Severity level assignment

**SectionValidator**:
- Single null section detection
- Multiple null sections detection
- At least one keyword section validation
- Mixed valid/invalid sections
- Optional sections ignored

### Integration Tests

**CommandHelp Validation**:
- CommandHelp with null Synopsis → Error
- CommandHelp with all null sections → Multiple errors
- CommandHelp with valid sections → No errors
- CommandHelp with optional null sections → No errors

### Contract Tests

**Error Message Format**:
- Matches specification: `[ERROR] file:line - section is null. Fix: guidance`
- Line numbers accurate
- File paths correct
- Section names match source

## Migration Path

**Phase 1** (this feature):
- Add validation to existing parsing
- Emit structured error messages
- Non-breaking (adds validation, doesn't remove features)

**Phase 2** (future):
- Configurable severity levels
- Custom validation rules
- JSON structured output for tooling
- Validation profiles (strict/relaxed)

**Phase 3** (future):
- Auto-fix suggestions
- Template generation for missing sections
- IDE integration (Language Server Protocol)

## Summary

The data model extends platyPS's existing architecture with minimal changes to core objects. Validation logic is encapsulated in new `SectionValidator` and `ValidationDiagnostic` classes that integrate with the existing `DiagnosticMessage` system. The approach maintains backward compatibility while adding comprehensive null section detection and reporting.
