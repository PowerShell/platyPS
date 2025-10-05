# Quickstart: Null Section Error Handling

## Purpose

This quickstart validates the null section error handling feature by walking through real-world usage scenarios as defined in the specification.

## Prerequisites

- platyPS module installed with null section validation feature
- PowerShell 7.4 or higher
- Test markdown help files (created below)

## Test Environment Setup

```powershell
# Create test directory
$testDir = New-Item -ItemType Directory -Path "TestNullValidation" -Force
Push-Location $testDir

# Import platyPS
Import-Module platyPS
```

## Scenario 1: Null SYNOPSIS Detection

**Given**: A markdown help file with a null SYNOPSIS section

### Create Test File

```powershell
@"
# Get-TestCommand

## DESCRIPTION

This command does something useful.

## EXAMPLES

### Example 1

``````powershell
Get-TestCommand
``````
"@ | Out-File -FilePath "NullSynopsis.md" -Encoding UTF8
```

### Execute Test

```powershell
# This should generate an error
$result = Test-MarkdownCommandHelp -Path "NullSynopsis.md" -ErrorVariable validationErrors -ErrorAction SilentlyContinue

# Verify error was generated
$validationErrors | Should -Not -BeNullOrEmpty

# Verify error format
$errorMessage = $validationErrors[0].Exception.Message
$errorMessage | Should -Match '^\[ERROR\] .*NullSynopsis\.md:\d+ - SYNOPSIS is null\. Fix: Add SYNOPSIS content\.$'
```

**Expected Result**: 
```
[ERROR] NullSynopsis.md:3 - SYNOPSIS is null. Fix: Add SYNOPSIS content.
```

## Scenario 2: Multiple Null Sections

**Given**: A markdown help file with multiple null sections (DESCRIPTION, EXAMPLES)

### Create Test File

```powershell
@"
# Get-MultiNullCommand

## SYNOPSIS

Brief description.

## PARAMETERS

### Name

Parameter description.
"@ | Out-File -FilePath "MultiNull.md" -Encoding UTF8
```

### Execute Test

```powershell
$result = Test-MarkdownCommandHelp -Path "MultiNull.md" -ErrorVariable validationErrors -ErrorAction SilentlyContinue

# Verify multiple errors generated
$validationErrors.Count | Should -BeGreaterThan 1

# Verify both errors are present
$errorMessages = $validationErrors | ForEach-Object { $_.Exception.Message }
$errorMessages | Should -Match 'DESCRIPTION is null'
$errorMessages | Should -Match 'EXAMPLES is null'
```

**Expected Result**:
```
[ERROR] MultiNull.md:7 - DESCRIPTION is null. Fix: Add DESCRIPTION content.
[ERROR] MultiNull.md:7 - EXAMPLES is null. Fix: Add EXAMPLES content.
```

## Scenario 3: All Required Sections Present

**Given**: A markdown help file with all required sections populated

### Create Test File

```powershell
@"
# Get-ValidCommand

## SYNOPSIS

Retrieves something useful.

## DESCRIPTION

This command retrieves useful information from the system.
It processes input and returns formatted output.

## PARAMETERS

### Name

Specifies the name to query.

``````yaml
Type: String
Parameter Sets: (All)
Aliases:

Required: True
Position: 0
Default value: None
Accept pipeline input: True (ByValue)
Accept wildcard characters: False
``````

## EXAMPLES

### Example 1: Basic usage

``````powershell
Get-ValidCommand -Name "Test"
``````

Retrieves information for the "Test" entity.

## INPUTS

### System.String

You can pipe strings to this cmdlet.

## OUTPUTS

### System.Object

Returns custom objects with query results.

## NOTES

Additional notes about the command.
"@ | Out-File -FilePath "ValidCommand.md" -Encoding UTF8
```

### Execute Test

```powershell
$result = Test-MarkdownCommandHelp -Path "ValidCommand.md" -ErrorVariable validationErrors -ErrorAction SilentlyContinue

# Verify no errors generated
$validationErrors | Should -BeNullOrEmpty

# Verify successful processing
$result | Should -Not -BeNull
```

**Expected Result**: No errors, processing succeeds.

## Scenario 4: Empty PARAMETERS Section

**Given**: A markdown help file with an empty PARAMETERS section

### Create Test File

```powershell
@"
# Get-EmptyParamsCommand

## SYNOPSIS

Command with empty parameters section.

## DESCRIPTION

This command has a PARAMETERS section but no actual parameters.

## PARAMETERS

## EXAMPLES

### Example 1

``````powershell
Get-EmptyParamsCommand
``````
"@ | Out-File -FilePath "EmptyParams.md" -Encoding UTF8
```

### Execute Test

```powershell
$result = Test-MarkdownCommandHelp -Path "EmptyParams.md" -ErrorVariable validationErrors -ErrorAction SilentlyContinue

# Verify error was generated
$validationErrors | Should -Not -BeNullOrEmpty

# Verify error message
$errorMessage = $validationErrors[0].Exception.Message
$errorMessage | Should -Match 'PARAMETERS is empty or null'
```

**Expected Result**:
```
[ERROR] EmptyParams.md:11 - PARAMETERS is empty or null. Fix: Add PARAMETERS content.
```

## Scenario 5: Whitespace-Only Section

**Given**: A markdown help file with a section containing only whitespace

### Create Test File

```powershell
@"
# Get-WhitespaceCommand

## SYNOPSIS
   
   

## DESCRIPTION

Valid description here.

## EXAMPLES

### Example 1

``````powershell
Get-WhitespaceCommand
``````
"@ | Out-File -FilePath "WhitespaceSection.md" -Encoding UTF8
```

### Execute Test

```powershell
$result = Test-MarkdownCommandHelp -Path "WhitespaceSection.md" -ErrorVariable validationErrors -ErrorAction SilentlyContinue

# Verify error was generated
$validationErrors | Should -Not -BeNullOrEmpty

# Verify whitespace detected as null
$errorMessage = $validationErrors[0].Exception.Message
$errorMessage | Should -Match 'SYNOPSIS is null'
```

**Expected Result**:
```
[ERROR] WhitespaceSection.md:3 - SYNOPSIS is null. Fix: Add SYNOPSIS content.
```

## Scenario 6: Multi-File Processing (Streaming Output)

**Given**: Multiple markdown files with various null section issues

### Create Test Files

```powershell
# File 1: Missing SYNOPSIS
@"
# Get-File1Command
## DESCRIPTION
Description only.
"@ | Out-File -FilePath "File1.md" -Encoding UTF8

# File 2: Missing DESCRIPTION  
@"
# Get-File2Command
## SYNOPSIS
Synopsis only.
"@ | Out-File -FilePath "File2.md" -Encoding UTF8

# File 3: Valid
@"
# Get-File3Command
## SYNOPSIS
Valid synopsis.
## DESCRIPTION
Valid description.
## EXAMPLES
### Example 1
``````powershell
Get-File3Command
``````
"@ | Out-File -FilePath "File3.md" -Encoding UTF8
```

### Execute Test

```powershell
# Test all files - should stream errors as encountered
$files = Get-ChildItem -Filter "File*.md"
$files | ForEach-Object {
    Write-Host "Processing $($_.Name)..." -ForegroundColor Cyan
    Test-MarkdownCommandHelp -Path $_.FullName -ErrorAction Continue
}

# Capture errors
$allErrors = @()
$files | ForEach-Object {
    Test-MarkdownCommandHelp -Path $_.FullName -ErrorVariable fileErrors -ErrorAction SilentlyContinue
    $allErrors += $fileErrors
}

# Verify errors from File1 and File2, but not File3
$allErrors.Count | Should -Be 2
$allErrors[0].Exception.Message | Should -Match 'File1.md.*SYNOPSIS'
$allErrors[1].Exception.Message | Should -Match 'File2.md.*DESCRIPTION'
```

**Expected Result** (streaming output):
```
Processing File1.md...
[ERROR] File1.md:2 - SYNOPSIS is null. Fix: Add SYNOPSIS content.

Processing File2.md...
[ERROR] File2.md:4 - DESCRIPTION is null. Fix: Add DESCRIPTION content.

Processing File3.md...
(no errors)
```

## Scenario 7: Import-MarkdownCommandHelp Validation

**Given**: Importing a markdown file with null sections

### Execute Test

```powershell
# Import file with null sections - should generate errors during import
$importResult = Import-MarkdownCommandHelp -Path "NullSynopsis.md" -ErrorVariable importErrors -ErrorAction SilentlyContinue

# Verify errors generated during import
$importErrors | Should -Not -BeNullOrEmpty
$importErrors[0].Exception.Message | Should -Match '\[ERROR\].*SYNOPSIS is null'

# Verify import still completes (returns CommandHelp object with diagnostic)
$importResult | Should -Not -BeNull
$importResult.GetType().Name | Should -Be 'CommandHelp'
```

**Expected Result**: Error reported during import, but CommandHelp object still returned for further processing.

## Scenario 8: Line Number Accuracy

**Given**: A complex markdown file with null sections at specific lines

### Create Test File

```powershell
@"
# Get-LineNumberTest

## SYNOPSIS

Valid synopsis on line 5.

## DESCRIPTION

## PARAMETERS

### Name

Parameter description.

## EXAMPLES

## NOTES

Some notes here.
"@ | Out-File -FilePath "LineNumbers.md" -Encoding UTF8
```

### Execute Test

```powershell
$result = Test-MarkdownCommandHelp -Path "LineNumbers.md" -ErrorVariable validationErrors -ErrorAction SilentlyContinue

# Verify line numbers are accurate
$validationErrors[0].Exception.Message | Should -Match 'LineNumbers\.md:9 - DESCRIPTION'
$validationErrors[1].Exception.Message | Should -Match 'LineNumbers\.md:15 - EXAMPLES'

# Line 9 is where DESCRIPTION section should have content
# Line 15 is where EXAMPLES section should have content
```

**Expected Result**: Errors report correct line numbers where null content occurs.

## Cleanup

```powershell
Pop-Location
Remove-Item -Path "TestNullValidation" -Recurse -Force
```

## Success Criteria

All scenarios should:

1. ✅ Generate errors with correct structured format: `[ERROR] [file]:[line] - [section] is null. Fix: Add [section] content.`
2. ✅ Include accurate line numbers for null sections
3. ✅ Report all null sections per file (not just first one)
4. ✅ Stream errors per file as encountered
5. ✅ Validate at least one keyword section is present
6. ✅ Detect null, empty, and whitespace-only sections
7. ✅ Block processing with Error severity level
8. ✅ Provide actionable guidance in error messages

## Integration with CI/CD

```powershell
# Example CI script usage
$errors = @()
Get-ChildItem -Path "docs" -Filter "*.md" -Recurse | ForEach-Object {
    Test-MarkdownCommandHelp -Path $_.FullName -ErrorVariable fileErrors -ErrorAction SilentlyContinue
    $errors += $fileErrors
}

if ($errors.Count -gt 0) {
    Write-Error "Documentation validation failed with $($errors.Count) errors"
    exit 1
}

Write-Host "✅ All documentation files validated successfully" -ForegroundColor Green
exit 0
```

## Notes

- All error messages use Error severity (blocks processing)
- Streaming output allows real-time feedback during validation
- Line numbers enable jump-to-error in IDEs
- Validation integrates with existing platyPS cmdlets (Import, Test, Export)
- Compatible with PowerShell error handling (`-ErrorAction`, `-ErrorVariable`)
