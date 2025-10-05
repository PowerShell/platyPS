# Research: Null Section Error Handling

## Overview

This document consolidates research findings for implementing improved null section error handling in platyPS.

## Existing Error Handling Architecture

### Current Diagnostic System

**Decision**: Use the existing `DiagnosticMessage` system in `src/Diagnostics/`

**Rationale**:
- platyPS already has a structured diagnostic framework with `DiagnosticMessage`, `DiagnosticMessageSource`, and `DiagnosticSeverity`
- The system supports different severity levels (Error, Warning, Information)
- Messages include source file information and can be aggregated
- Integrates with PowerShell error streams naturally

**Implementation Details**:
- `DiagnosticMessage.cs` - Contains message text, severity, source location
- `DiagnosticSeverity.cs` - Enum with Error, Warning, Information
- `DiagnosticMessageSource.cs` - Tracks file path and location information
- `Diagnostics.cs` - Central diagnostic collection and reporting

**Alternatives Considered**:
- Create new validation framework: Rejected - unnecessary duplication, breaks consistency
- Use PowerShell ErrorRecord directly: Rejected - loses structured diagnostic capabilities
- Throw exceptions: Rejected - doesn't support collecting multiple errors per file

## Section Validation Requirements

### Required vs Optional Sections

**Decision**: At least one keyword section must be present (as clarified)

**Rationale**:
- Comment-based help in PowerShell has flexible structure
- Different cmdlet types require different sections
- At minimum, one identifying section is needed for the help to be meaningful
- Sections are flat (not nested), simplifying validation logic

**Common PowerShell Help Sections**:
- SYNOPSIS - Brief description (typically required)
- DESCRIPTION - Detailed description (typically required)
- PARAMETERS - Parameter documentation (required if cmdlet has parameters)
- EXAMPLES - Usage examples (highly recommended)
- INPUTS - Pipeline input types (optional)
- OUTPUTS - Output types (optional)
- NOTES - Additional information (optional)
- LINK - Related links (optional)

**Alternatives Considered**:
- Enforce strict section requirements: Rejected - too rigid for varied cmdlet types
- Make all sections optional: Rejected - allows meaningless help files
- Different rules per cmdlet type: Rejected - adds complexity, hard to maintain

## Error Message Format

### Structured Error Format

**Decision**: Use format `[ERROR] [file]:[line] - [section] is null. Fix: Add [section] content.`

**Rationale**:
- Provides clear severity indicator
- Includes precise location (file and line number)
- States the problem explicitly
- Offers actionable guidance
- Follows common compiler/linter error format conventions

**Format Components**:
1. **Severity prefix**: `[ERROR]` - Clearly indicates blocking issue
2. **Location**: `[file]:[line]` - Standard format for jump-to-error in editors
3. **Problem description**: `[section] is null` - Explicit, no jargon
4. **Guidance**: `Fix: Add [section] content.` - Actionable next step

**Alternatives Considered**:
- Simple format "Section X is missing": Rejected - no location, no severity, no guidance
- PowerShell style `Write-Error`: Rejected - less parseable, not IDE-friendly
- JSON structured output: Rejected - not human-readable in console, better for tooling integration (can add later)

## Validation Timing

### When to Validate

**Decision**: Validate during Import operations and Test-MarkdownCommandHelp

**Rationale**:
- Import cmdlets parse markdown/YAML → natural validation point
- Test-MarkdownCommandHelp explicitly designed for validation
- Early detection prevents propagation of invalid documentation
- Streaming output allows per-file reporting as files are processed

**Validation Points**:
1. **Import-MarkdownCommandHelp** - Validate sections when parsing markdown
2. **Import-YamlCommandHelp** - Validate sections when parsing YAML
3. **Import-MamlHelp** - Validate sections when parsing MAML
4. **Test-MarkdownCommandHelp** - Primary validation cmdlet (enhanced)
5. **Update-MarkdownCommandHelp** - Validate after update operations
6. **New-MarkdownCommandHelp** - Validate newly generated files

**Alternatives Considered**:
- Validate only on Test-* cmdlets: Rejected - allows invalid imports to proceed
- Validate only on Export: Rejected - too late, harder to trace source
- Optional validation flag: Rejected - errors should always be reported

## Line Number Tracking

### Implementation Approach

**Decision**: Enhance Markdig parser integration to track line numbers during parsing

**Rationale**:
- Markdig (markdown parser) provides line information via `LineNumber` property on syntax nodes
- Line numbers essential for structured error format
- Enables IDE integration (jump-to-error)
- Required for FR-006 compliance

**Implementation Strategy**:
- Capture line numbers during markdown AST traversal
- Store line information in temporary validation context
- Include in DiagnosticMessageSource when creating error messages
- Handle YAML line numbers via YamlDotNet's MarkEventListener

**Alternatives Considered**:
- Skip line numbers, use only file names: Rejected - violates FR-006
- Approximate line numbers: Rejected - inaccurate, frustrating for users
- Require pre-processing pass: Rejected - performance impact

## Multi-File Processing

### Streaming Output Strategy

**Decision**: Report errors per file as encountered (clarified as option C)

**Rationale**:
- Provides immediate feedback during long validation runs
- Users can start fixing while validation continues
- Aligns with streaming patterns in CI/CD pipelines
- Reduces memory footprint (no need to buffer all errors)

**Implementation**:
- Write diagnostic messages to PowerShell error stream as they occur
- Continue processing remaining files even if errors found
- Aggregate summary at end (total files processed, total errors)
- Support `-ErrorAction` parameter for control

**Alternatives Considered**:
- Buffer all errors, report at end: Rejected - poor UX for large modules
- Stop on first error: Rejected - requires multiple validation runs
- Configurable mode: Deferred - can add later if users request

## Performance Considerations

### Validation Overhead

**Target**: <10% performance overhead (NFR-002)

**Strategy**:
- Validation during existing parsing (no extra pass)
- Efficient null/whitespace checks (single pass)
- Avoid regex where simple string checks suffice
- Lazy evaluation where possible

**Measurement**:
- Benchmark existing Import-MarkdownCommandHelp performance
- Add validation, re-measure
- Use PowerShell `Measure-Command` for validation
- Target: Original time * 1.1 or less

## Testing Strategy

### TDD Approach

**Red Phase** (Write failing tests first):
1. Test for null SYNOPSIS detection
2. Test for multiple null sections in one file
3. Test for whitespace-only sections
4. Test for structured error message format
5. Test for line number accuracy
6. Test for multi-file streaming output
7. Test for Error severity level

**Green Phase** (Implement to pass tests):
1. Enhance DiagnosticMessage with null section detection
2. Integrate validation into Import cmdlets
3. Add line number tracking
4. Format error messages per specification
5. Implement streaming output

**Refactor Phase**:
1. Extract validation logic to reusable helpers
2. Optimize performance
3. Add debug/verbose output
4. Improve error message clarity

## Integration Points

### Affected Components

1. **Diagnostics System** (`src/Diagnostics/`)
   - Add null section error type
   - Enhance message formatting
   - Support line number tracking

2. **MarkdownReader** (`src/MarkdownReader/`)
   - Add section validation during parsing
   - Track line numbers from Markdig
   - Report diagnostic messages

3. **YamlWriter** (`src/YamlWriter/`)
   - Add section validation for YAML
   - Track line numbers from YamlDotNet
   - Report diagnostic messages

4. **Command Cmdlets** (`src/Command/`)
   - Integrate validation in Import commands
   - Enhance Test-MarkdownCommandHelp
   - Support streaming output

5. **Test Suite** (`test/Pester/`)
   - Add null section validation tests
   - Add error format tests
   - Add integration tests

## Risk Mitigation

### Backward Compatibility

**Risk**: Existing workflows may break if previously-tolerated null sections now error

**Mitigation**:
- Version bump: PATCH level (improves existing behavior)
- Clear release notes documenting new validation
- Provide examples of how to fix common errors
- Consider grace period with warnings before hard errors (future enhancement)

### Performance Impact

**Risk**: Validation adds processing time

**Mitigation**:
- Target <10% overhead per NFR-002
- Benchmark before and after
- Optimize hot paths
- Profile with large documentation sets

### False Positives

**Risk**: Valid edge cases incorrectly flagged as errors

**Mitigation**:
- Comprehensive test coverage for edge cases
- Whitespace handling (truly empty vs formatting whitespace)
- Optional sections correctly identified
- Beta testing with real-world documentation

## Conclusion

All research findings support the technical approach outlined in the specification. No blockers identified. Ready to proceed to Phase 1 (Design & Contracts).
