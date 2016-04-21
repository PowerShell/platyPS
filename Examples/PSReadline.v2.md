# Get-PSReadlineKeyHandler
## SYNOPSIS
Gets the key bindings for the PSReadline module.

## SYNTAX

### Set 1
```
Get-PSReadlineKeyHandler [-Bound] [-Unbound] [<CommonParameters>]
```

## DESCRIPTION
Gets the key bindings for the PSReadline module.
If neither -Bound nor -Unbound is specified, returns all bound keys and unbound functions.
If -Bound is specified and -Unbound is not specified, only bound keys are returned.
If -Unound is specified and -Bound is not specified, only unbound keys are returned.
If both -Bound and -Unound are specified, returns all bound keys and unbound functions.

## EXAMPLES

## PARAMETERS

### Bound
Include functions that are bound.

```yaml
Type: switch
Parameter Sets: Set 1
Aliases: 

Required: False
Position: Named
Default value: 
Accept pipeline input: false
Accept wildcard characters: False
```

### Unbound
Include functions that are unbound.

```yaml
Type: switch
Parameter Sets: Set 1
Aliases: 

Required: False
Position: Named
Default value: 
Accept pipeline input: false
Accept wildcard characters: False
```

## INPUTS

### None
You cannot pipe objects to Get-PSReadlineKeyHandler

## OUTPUTS

### PSConsoleUtilities.KeyHandler
Returns one entry for each key binding (or chord) for bound functions and/or one entry for each unbound function

## RELATED LINKS

[about_PSReadline]()

# Get-PSReadlineOption
## SYNOPSIS
Returns the values for the options that can be configured.

## SYNTAX

### Set 1
```
Get-PSReadlineOption [<CommonParameters>]
```

## DESCRIPTION
Get-PSReadlineOption returns the current state of the settings that can be configured by Set-PSReadlineOption.
The object returned can be used to change PSReadline options. This provides a slightly simpler way of setting syntax coloring options for multiple kinds of tokens.

## EXAMPLES

## PARAMETERS

## INPUTS

### None
You cannot pipe objects to Get-PSReadlineOption

## OUTPUTS

###  


## RELATED LINKS

[about_PSReadline]()

# Set-PSReadlineKeyHandler
## SYNOPSIS
Binds or rebinds keys to user defined or PSReadline provided key handlers.

## SYNTAX

### Set 1
```
Set-PSReadlineKeyHandler [-Chord] <String[]> [-ScriptBlock] <ScriptBlock>
 [-BriefDescription <String>] [-Description <String>] [<CommonParameters>]
```

### Set 2
```
Set-PSReadlineKeyHandler [-Chord] <String[]> [-Function] <String>
 [<CommonParameters>]
```

## DESCRIPTION
This cmdlet is used to customize what happens when a particular key or sequence of keys is pressed while PSReadline is reading input.
With user defined key bindings, you can do nearly anything that is possible from a PowerShell script. Typically you might just edit the command line in some novel way, but because the handlers are just PowerShell scripts, you can do interesting things like change directories, launch programs, etc.

## EXAMPLES

### --------------  Example 1  --------------
```
PS C:\> Set-PSReadlineKeyHandler -Key UpArrow -Function HistorySearchBackward
```

This command binds the up arrow key to the function HistorySearchBackward which will use the currently entered command line as the beginning of the search string when searching through history.

### --------------  Example 2  --------------
```
PS C:\> Set-PSReadlineKeyHandler -Chord Shift+Ctrl+B -ScriptBlock {
    [PSConsoleUtilities.PSConsoleReadLine]::RevertLine()
    [PSConsoleUtilities.PSConsoleReadLine]::Insert('build')
>>>     [PSConsoleUtilities.PSConsoleReadLine]::AcceptLine()
}
```

This example binds the key Ctrl+Shift+B to a script block that clears the line, inserts build, then accepts the line. This example shows how a single key can be used to execute a command.

## PARAMETERS

### Chord
The key or sequence of keys to be bound to a Function or ScriptBlock. A single binding is specified with a single string. If the binding is a sequence of keys, the keys are separated with a comma, e.g. "Ctrl+X,Ctrl+X". Note that this parameter accepts multiple strings. Each string is a separate binding, not a sequence of keys for a single binding.

```yaml
Type: String[]
Parameter Sets: Set 1, Set 2
Aliases: 

Required: True
Position: 0
Default value: 
Accept pipeline input: false
Accept wildcard characters: False
```

### ScriptBlock
The ScriptBlock is called when the Chord is entered. The ScriptBlock is passed one or sometimes two arguments. The first argument is the key pressed (a ConsoleKeyInfo.)  The second argument could be any object depending on the context.

```yaml
Type: ScriptBlock
Parameter Sets: Set 1
Aliases: 

Required: True
Position: 1
Default value: 
Accept pipeline input: false
Accept wildcard characters: False
```

### BriefDescription
A brief description of the key binding. Used in the output of cmdlet Get-PSReadlineKeyHandler.

```yaml
Type: String
Parameter Sets: Set 1
Aliases: 

Required: False
Position: Named
Default value: 
Accept pipeline input: false
Accept wildcard characters: False
```

### Description
A more verbose description of the key binding. Used in the output of the cmdlet Get-PSReadlineKeyHandler.

```yaml
Type: String
Parameter Sets: Set 1
Aliases: 

Required: False
Position: Named
Default value: 
Accept pipeline input: false
Accept wildcard characters: False
```

### Function
The name of an existing key handler provided by PSReadline. This parameter allows one to rebind existing key bindings or to bind a handler provided by PSReadline that is currently unbound.
Using the ScriptBlock parameter, one can achieve equivalent functionality by calling the method directly from the ScriptBlock. This parameter is preferred though - it makes it easier to determine which functions are bound and unbound.

```yaml
Type: String
Parameter Sets: Set 2
Aliases: 

Required: True
Position: 1
Default value: 
Accept pipeline input: false
Accept wildcard characters: False
```

## INPUTS

### None
You cannot pipe objects to Set-PSReadlineKeyHandler

## OUTPUTS

###  


## RELATED LINKS

[about_PSReadline]()

# Set-PSReadlineOption
## SYNOPSIS
Customizes the behavior of command line editing in PSReadline.

## SYNTAX

### Set 1
```
Set-PSReadlineOption [-EditMode <EditMode>] [-ContinuationPrompt <String>]
 [-ContinuationPromptForegroundColor <ConsoleColor>]
 [-ContinuationPromptBackgroundColor <ConsoleColor>]
 [-EmphasisForegroundColor <ConsoleColor>]
 [-EmphasisBackgroundColor <ConsoleColor>] [-ErrorForegroundColor <ConsoleColor>]
 [-ErrorBackgroundColor <ConsoleColor>] [-HistoryNoDuplicates]
 [-AddToHistoryHandler <Func[String, Boolean]>]
 [-ValidationHandler <Func[String, Object]>] [-HistorySearchCursorMovesToEnd]
 [-MaximumHistoryCount <Int32>] [-MaximumKillRingCount <Int32>]
 [-ResetTokenColors] [-ShowToolTips] [-ExtraPromptLineCount <Int32>]
 [-DingTone <Int32>] [-DingDuration <Int32>] [-BellStyle <BellStyle>]
 [-CompletionQueryItems <Int32>] [-WordDelimiters <string>]
 [-HistorySearchCaseSensitive] [-HistorySaveStyle <HistorySaveStyle>]
 [-HistorySavePath <String>] [<CommonParameters>]
```

### Set 2
```
Set-PSReadlineOption [-TokenKind] <TokenClassification>
 [[-ForegroundColor] <ConsoleColor>] [[-BackgroundColor] <ConsoleColor>]
 [<CommonParameters>]
```

## DESCRIPTION
The Set-PSReadlineOption cmdlet is used to customize the behavior of the PSReadline module when editing the command line.

## EXAMPLES

## PARAMETERS

### EditMode
Specifies the command line editing mode. This will reset any key bindings set by Set-PSReadlineKeyHandler.
Valid values are:
-- Windows: Key bindings emulate PowerShell/cmd with some bindings emulating Visual Studio.
-- Emacs: Key bindings emulate Bash or Emacs.

```yaml
Type: EditMode
Parameter Sets: Set 1
Aliases: 

Required: False
Position: Named
Default value: 
Accept pipeline input: false
Accept wildcard characters: False
```

### ContinuationPrompt
Specifies the string displayed at the beginning of the second and subsequent lines when multi-line input is being entered. Defaults to '>>> '. The empty string is valid.

```yaml
Type: String
Parameter Sets: Set 1
Aliases: 

Required: False
Position: Named
Default value: 
Accept pipeline input: false
Accept wildcard characters: False
```

### ContinuationPromptForegroundColor
Specifies the foreground color of the continuation prompt.

```yaml
Type: ConsoleColor
Parameter Sets: Set 1
Aliases: 

Required: False
Position: Named
Default value: 
Accept pipeline input: false
Accept wildcard characters: False
```

### ContinuationPromptBackgroundColor
Specifies the background color of the continuation prompt.

```yaml
Type: ConsoleColor
Parameter Sets: Set 1
Aliases: 

Required: False
Position: Named
Default value: 
Accept pipeline input: false
Accept wildcard characters: False
```

### EmphasisForegroundColor
Specifies the foreground color used for emphasis, e.g. to highlight search text.

```yaml
Type: ConsoleColor
Parameter Sets: Set 1
Aliases: 

Required: False
Position: Named
Default value: 
Accept pipeline input: false
Accept wildcard characters: False
```

### EmphasisBackgroundColor
Specifies the background color used for emphasis, e.g. to highlight search text.

```yaml
Type: ConsoleColor
Parameter Sets: Set 1
Aliases: 

Required: False
Position: Named
Default value: 
Accept pipeline input: false
Accept wildcard characters: False
```

### ErrorForegroundColor
Specifies the foreground color used for errors.

```yaml
Type: ConsoleColor
Parameter Sets: Set 1
Aliases: 

Required: False
Position: Named
Default value: 
Accept pipeline input: false
Accept wildcard characters: False
```

### ErrorBackgroundColor
Specifies the background color used for errors.

```yaml
Type: ConsoleColor
Parameter Sets: Set 1
Aliases: 

Required: False
Position: Named
Default value: 
Accept pipeline input: false
Accept wildcard characters: False
```

### HistoryNoDuplicates
Specifies that duplicate commands should not be added to PSReadline history.

```yaml
Type: switch
Parameter Sets: Set 1
Aliases: 

Required: False
Position: Named
Default value: 
Accept pipeline input: false
Accept wildcard characters: False
```

### AddToHistoryHandler
Specifies a ScriptBlock that can be used to control which commands get added to PSReadline history.

```yaml
Type: Func[String, Boolean]
Parameter Sets: Set 1
Aliases: 

Required: False
Position: Named
Default value: 
Accept pipeline input: false
Accept wildcard characters: False
```

### ValidationHandler
Specifies a ScriptBlock that is called from ValidateAndAcceptLine. If a non-null object is returned or an exception is thrown, validation fails and the error is reported. If the object returned/thrown has a Message property, it's value is used in the error message, and if there is an Offset property, the cursor is moved to that offset after reporting the error. If there is no Message property, the ToString method is called to report the error.

```yaml
Type: Func[String, Object]
Parameter Sets: Set 1
Aliases: 

Required: False
Position: Named
Default value: 
Accept pipeline input: false
Accept wildcard characters: False
```

### HistorySearchCursorMovesToEnd


```yaml
Type: switch
Parameter Sets: Set 1
Aliases: 

Required: False
Position: Named
Default value: 
Accept pipeline input: false
Accept wildcard characters: False
```

### MaximumHistoryCount
Specifies the maximum number of commands to save in PSReadline history. Note that PSReadline history is separate from PowerShell history.

```yaml
Type: Int32
Parameter Sets: Set 1
Aliases: 

Required: False
Position: Named
Default value: 
Accept pipeline input: false
Accept wildcard characters: False
```

### MaximumKillRingCount
Specifies the maximum number of items stored in the kill ring.

```yaml
Type: Int32
Parameter Sets: Set 1
Aliases: 

Required: False
Position: Named
Default value: 
Accept pipeline input: false
Accept wildcard characters: False
```

### ResetTokenColors
Restore the token colors to the default settings.

```yaml
Type: switch
Parameter Sets: Set 1
Aliases: 

Required: False
Position: Named
Default value: 
Accept pipeline input: false
Accept wildcard characters: False
```

### ShowToolTips
When displaying possible completions, show tooltips in the list of completions.

```yaml
Type: switch
Parameter Sets: Set 1
Aliases: 

Required: False
Position: Named
Default value: 
Accept pipeline input: false
Accept wildcard characters: False
```

### ExtraPromptLineCount
Use this option if your prompt spans more than one line and you want the extra lines to appear when PSReadline displays the prompt after showing some output, e.g. when showing a list of completions.

```yaml
Type: Int32
Parameter Sets: Set 1
Aliases: 

Required: False
Position: Named
Default value: 
Accept pipeline input: false
Accept wildcard characters: False
```

### DingTone
When BellStyle is set to Audible, specifies the tone of the beep.

```yaml
Type: Int32
Parameter Sets: Set 1
Aliases: 

Required: False
Position: Named
Default value: 
Accept pipeline input: false
Accept wildcard characters: False
```

### DingDuration
When BellStyle is set to Audible, specifies the duration of the beep.

```yaml
Type: Int32
Parameter Sets: Set 1
Aliases: 

Required: False
Position: Named
Default value: 
Accept pipeline input: false
Accept wildcard characters: False
```

### BellStyle
Specifies how PSReadLine should respond to various error and ambiguous conditions.
Valid values are:
-- Audible: a short beep
-- Visible: a brief flash is performed
-- None: no feedback

```yaml
Type: BellStyle
Parameter Sets: Set 1
Aliases: 

Required: False
Position: Named
Default value: 
Accept pipeline input: false
Accept wildcard characters: False
```

### CompletionQueryItems
Specifies the maximum number of completion items that will be shown without prompting. If the number of items to show is greater than this value, PSReadline will prompt y/n before displaying the completion items.

```yaml
Type: Int32
Parameter Sets: Set 1
Aliases: 

Required: False
Position: Named
Default value: 
Accept pipeline input: false
Accept wildcard characters: False
```

### WordDelimiters
Specifies the characters that delimit words for functions like ForwardWord or KillWord.

```yaml
Type: string
Parameter Sets: Set 1
Aliases: 

Required: False
Position: Named
Default value: 
Accept pipeline input: false
Accept wildcard characters: False
```

### HistorySearchCaseSensitive
Specifies the searching history is case sensitive in functions like ReverseSearchHistory or HistorySearchBackward.

```yaml
Type: switch
Parameter Sets: Set 1
Aliases: 

Required: False
Position: Named
Default value: 
Accept pipeline input: false
Accept wildcard characters: False
```

### HistorySaveStyle
Specifies how PSReadLine should save history.
Valid values are:
-- SaveIncrementally: save history after each command is executed - and share across multiple instances of PowerShell
-- SaveAtExit: append history file when PowerShell exits
-- SaveNothing: don't use a history file

```yaml
Type: HistorySaveStyle
Parameter Sets: Set 1
Aliases: 

Required: False
Position: Named
Default value: 
Accept pipeline input: false
Accept wildcard characters: False
```

### HistorySavePath
Specifies the path to the history file.

```yaml
Type: String
Parameter Sets: Set 1
Aliases: 

Required: False
Position: Named
Default value: 
Accept pipeline input: false
Accept wildcard characters: False
```

### TokenKind
Specifies the kind of token when setting token coloring options with the -ForegroundColor and -BackgroundColor parameters.

```yaml
Type: TokenClassification
Parameter Sets: Set 2
Aliases: 

Required: True
Position: 0
Default value: 
Accept pipeline input: false
Accept wildcard characters: False
```

### ForegroundColor
Specifies the foreground color for the token kind specified by the parameter -TokenKind.

```yaml
Type: ConsoleColor
Parameter Sets: Set 2
Aliases: 

Required: False
Position: 1
Default value: 
Accept pipeline input: false
Accept wildcard characters: False
```

### BackgroundColor
Specifies the background color for the token kind specified by the parameter -TokenKind.

```yaml
Type: ConsoleColor
Parameter Sets: Set 2
Aliases: 

Required: False
Position: 2
Default value: 
Accept pipeline input: false
Accept wildcard characters: False
```

## INPUTS

### None
You cannot pipe objects to Set-PSReadlineOption

## OUTPUTS

### None
This cmdlet does not generate any output.

## RELATED LINKS

[about_PSReadline]()


