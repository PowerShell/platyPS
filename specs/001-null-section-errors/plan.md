# Implementation Plan: Null Section Error Handling

**Branch**: `001-null-section-errors` | **Date**: 2025-10-06 | **Spec**: [spec.md](./spec.md)
**Input**: Feature specification from `C:\Repos\GitHub\MariusStorhaug\platyPS\specs\001-null-section-errors\spec.md`

## Execution Flow (/plan command scope)

1. Load feature spec from Input path
   → If not found: ERROR "No feature spec at {path}"
2. Fill Technical Context (scan for NEEDS CLARIFICATION)
   → Detect Project Type from file system structure or context (web=frontend+backend, mobile=app+API)
   → Set Structure Decision based on project type
3. Fill the Constitution Check section based on the content of the constitution document.
4. Evaluate Constitution Check section below
   → If violations exist: Document them in Complexity Tracking
   → If no justification is possible: ERROR "Simplify approach first"
   → Update Progress Tracking: Initial Constitution Check
5. Execute Phase 0 → research.md
   → If NEEDS CLARIFICATION remain: ERROR "Resolve unknowns"
6. Execute Phase 1 → contracts, data-model.md, quickstart.md, agent-specific template file (e.g., `CLAUDE.md` for Claude Code, `.github/copilot-instructions.md` for GitHub Copilot, `GEMINI.md` for Gemini CLI, `QWEN.md` for Qwen Code or `AGENTS.md` for opencode).
7. Re-evaluate Constitution Check section
   → If new violations: Refactor design, return to Phase 1
   → Update Progress Tracking: Post-Design Constitution Check
8. Plan Phase 2 → Describe task generation approach (DO NOT create tasks.md)
9. STOP - Ready for /tasks command

**IMPORTANT**: The /plan command STOPS at step 7. Phases 2-4 are executed by other commands:

- Phase 2: /tasks command creates tasks.md
- Phase 3-4: Implementation execution (manual or via tools)

## Summary

Improve error handling in platyPS when help sections are null or missing. The feature will detect null/empty sections in markdown help files and generate clear, actionable error messages using a structured format: `[ERROR] [file]:[line] - [section] is null. Fix: Add [section] content.` Errors will be reported per file as encountered (streaming output), with Error severity level that blocks processing. The system will validate that at least one keyword section is present and distinguish between required and optional sections.

## Technical Context

| Aspect | Details |
|--------|---------|
| **Language/Version** | C# (.NET Framework 4.7.2), PowerShell 7.4+ |
| **Primary Dependencies** | Markdig (Markdown parsing), YamlDotNet (YAML), PowerShellStandard.Library |
| **Storage** | N/A (processes files, no persistent storage) |
| **Testing** | Pester 4.x (PowerShell), xUnit (C#), PSScriptAnalyzer |
| **Target Platform** | Cross-platform (Windows, Linux, macOS via PowerShell 7.4+) |
| **Project Type** | Single project (C# binary module with PowerShell cmdlets) |
| **Performance Goals** | Validation overhead <10%, streaming output for real-time feedback |
| **Constraints** | Must integrate with existing CommandHelp object model, maintain backward compatibility with existing error handling |
| **Scale/Scope** | Affects all 19 cmdlets that process markdown/YAML/MAML, particularly Import/Export/Test/Update commands |

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

### I. Workflow-First Design (NON-NEGOTIABLE)

- [x] Feature is implemented as reusable GitHub Actions workflow(s)
- [x] Workflows have clearly defined inputs and outputs
- [x] Workflows follow single responsibility principle
- [x] Matrix strategies used for parallel execution where appropriate
- [x] Workflows are independently testable via CI validation workflow
- [x] Logic delegated to reusable GitHub Actions (PSModule organization)
- [x] Inline PowerShell code avoided; action-based scripts used instead
- [x] Actions referenced by specific versions/tags

### II. Test-Driven Development (NON-NEGOTIABLE)

- [x] Tests will be written before implementation
- [x] Initial tests will fail (Red phase documented)
- [x] Implementation plan includes making tests pass (Green phase)
- [x] Refactoring phase planned while maintaining tests
- [x] PSScriptAnalyzer validation included
- [x] Manual testing documented if needed
- [x] CI validation workflow tests included

### III. Platform Independence with Modern PowerShell

- [x] PowerShell 7.4+ constructs used exclusively
- [x] Matrix testing across Linux, macOS, Windows included
- [x] Platform-specific behaviors documented
- [x] Skip mechanisms justified if platform-specific tests needed
- [x] No backward compatibility with PowerShell 5.1 required

### IV. Quality Gates and Observability

- [x] Test results captured in structured JSON format
- [x] Code coverage measurement included
- [x] Linting results captured and enforced
- [x] Quality gate thresholds defined
- [x] Clear error messages planned
- [x] Debug mode support included

### V. Continuous Delivery with Semantic Versioning

- [x] Version bump strategy documented (labels, SemVer)
- [x] Release automation compatible with existing workflow
- [x] Documentation updates included
- [x] GitHub Pages publishing considered if docs changes

## Project Structure

### Documentation (this feature)

```plaintext
specs/001-null-section-errors/
├── spec.md              # Feature specification
├── plan.md              # This file (/plan command output)
├── research.md          # Phase 0 output
├── data-model.md        # Phase 1 output
├── quickstart.md        # Phase 1 output
└── tasks.md             # Phase 2 output (/tasks command)
```

### Source Code (repository root)

```plaintext
src/
├── Diagnostics/
│   ├── DiagnosticMessage.cs          # Enhanced with null section errors
│   ├── DiagnosticMessageSource.cs
│   └── Diagnostics.cs
├── MarkdownReader/
│   └── [Markdown parsing - enhanced validation]
├── YamlWriter/
│   └── [YAML processing - enhanced validation]
├── Model/
│   └── CommandHelp.cs                # Central object model
└── Command/
    ├── TestMarkdownCommandHelp.cs    # Primary cmdlet for validation
    ├── ImportMarkdownCommand.cs      # Add validation
    ├── ImportYamlCommand.cs          # Add validation
    ├── ExportMarkdownHelpCommand.cs  # Add validation
    └── [other Import/Export commands]

test/Pester/
├── NullSectionValidation.Tests.ps1   # New test file
├── ErrorFormatting.Tests.ps1         # New test file
└── [existing test files]
```

**Structure Decision**: Single project structure. This is an enhancement to the existing platyPS C# binary module. Changes will primarily affect the Diagnostics system and validation logic in MarkdownReader/YamlWriter components. The CommandHelp object model remains central, with enhanced validation integrated into existing Import/Export/Test cmdlets.

## Phase 0: Outline & Research

1. **Extract unknowns from Technical Context** above:
   - For each NEEDS CLARIFICATION → research task
   - For each dependency → best practices task
   - For each integration → patterns task
2. **Generate and dispatch research agents**:
   ```plaintext
   For each unknown in Technical Context:
     Task: "Research {unknown} for {feature context}"
   For each technology choice:
     Task: "Find best practices for {tech} in {domain}"
   ```
3. **Consolidate findings** in `research.md` using format:
   - Decision: [what was chosen]
   - Rationale: [why chosen]
   - Alternatives considered: [what else evaluated]

**Output**: research.md with all NEEDS CLARIFICATION resolved

## Phase 1: Design & Contracts

*Prerequisites: research.md complete*

1. **Extract entities from feature spec** → `data-model.md`:
   - Entity name, fields, relationships
   - Validation rules from requirements
   - State transitions if applicable
2. **Generate API contracts** from functional requirements:
   - For each user action → endpoint
   - Use standard REST/GraphQL patterns
   - Output OpenAPI/GraphQL schema to `/contracts/`
3. **Generate contract tests** from contracts:
   - One test file per endpoint
   - Assert request/response schemas
   - Tests must fail (no implementation yet)
4. **Extract test scenarios** from user stories:
   - Each story → integration test scenario
   - Quickstart test = story validation steps
5. **Update agent file incrementally** (O(1) operation):
   - Run `.specify/scripts/powershell/update-agent-context.ps1 -AgentType copilot`
     **IMPORTANT**: Execute it exactly as specified above. Do not add or remove any arguments.
   - If exists: Add only NEW tech from current plan
   - Preserve manual additions between markers
   - Update recent changes (keep last 3)
   - Keep under 150 lines for token efficiency
   - Output to repository root

**Output**: data-model.md, /contracts/*, failing tests, quickstart.md, agent-specific file

## Phase 2: Task Planning Approach

*This section describes what the /tasks command will do - DO NOT execute during /plan*

**Task Generation Strategy**:

The /tasks command will generate tasks from Phase 1 design docs (data-model.md, quickstart.md) following TDD principles:

1. **Test Tasks** (Red Phase):
   - Unit tests for HelpSection validation state detection
   - Unit tests for ValidationDiagnostic message formatting
   - Unit tests for SectionValidator logic
   - Integration tests for Import-MarkdownCommandHelp with null sections
   - Integration tests for Test-MarkdownCommandHelp validation
   - Contract tests for structured error message format
   - Quickstart scenario tests (8 scenarios from quickstart.md)

2. **Implementation Tasks** (Green Phase):
   - Extend DiagnosticMessage class with line number tracking
   - Create SectionValidator class with validation logic
   - Create ValidationDiagnostic class for structured errors
   - Integrate validation into MarkdownReader (Add line number tracking)
   - Integrate validation into Import-MarkdownCommandHelp cmdlet
   - Integrate validation into Test-MarkdownCommandHelp cmdlet
   - Implement streaming error output
   - Add whitespace detection logic

3. **Refactoring Tasks**:
   - Extract validation helpers to reduce duplication
   - Optimize performance (ensure <10% overhead)
   - Add verbose/debug output support
   - PSScriptAnalyzer validation

**Ordering Strategy**:

- TDD order: All test tasks before implementation tasks
- Dependency order:
  1. Core model tests (HelpSection, ValidationDiagnostic) [P]
  2. Validator logic tests [P]
  3. Integration tests (depends on model tests)
  4. Model implementations (HelpSection, ValidationDiagnostic) [P]
  5. Validator implementation (depends on models)
  6. Cmdlet integration (depends on validator)
  7. Refactoring (depends on passing tests)
- Mark [P] for parallel execution (independent files/tests)

**Estimated Output**: 20-25 numbered, ordered tasks in tasks.md

**Key Dependencies**:
- MarkdownReader changes depend on DiagnosticMessage enhancements
- Cmdlet integration depends on SectionValidator completion
- All implementation depends on tests being written first (TDD)

**IMPORTANT**: This phase is executed by the /tasks command, NOT by /plan

## Phase 3+: Future Implementation

*These phases are beyond the scope of the /plan command*

**Phase 3**: Task execution (/tasks command creates tasks.md)
**Phase 4**: Implementation (execute tasks.md following constitutional principles)
**Phase 5**: Validation (run tests, execute quickstart.md, performance validation)

## Complexity Tracking

*No constitutional violations - all principles satisfied*

This feature enhances existing diagnostic infrastructure without adding complexity. Validation integrates into existing parsing workflows, following established patterns in the codebase.

## Progress Tracking

*This checklist is updated during execution flow*

**Phase Status**:

- [x] Phase 0: Research complete (/plan command)
- [x] Phase 1: Design complete (/plan command)
- [x] Phase 2: Task planning complete (/plan command - describe approach only)
- [ ] Phase 3: Tasks generated (/tasks command)
- [ ] Phase 4: Implementation complete
- [ ] Phase 5: Validation passed

**Gate Status**:

- [x] Initial Constitution Check: PASS
- [x] Post-Design Constitution Check: PASS
- [x] All NEEDS CLARIFICATION resolved
- [x] Complexity deviations documented (N/A - no violations)

---
*Based on Constitution - See `.specify/memory/constitution.md`*
