---
content type: cmdlet
title: Submit-AzDataLakeAnalyticsJob
Module Name: Az.DataLakeAnalytics
Locale: {{ fill in locale }}
PlatyPS schema version: 2024-05-01
HelpUri: https://learn.microsoft.com/powershell/module/az.datalakeanalytics/submit-azdatalakeanalyticsjob
ms.date: 05/24/2024
external help file: Microsoft.Azure.PowerShell.Cmdlets.DataLakeAnalytics.dll-Help.xml
---

# Submit-AzDataLakeAnalyticsJob

## SYNOPSIS

Submits a job.

## SYNTAX

### SubmitUSqlJobWithScriptPath

```
Submit-AzDataLakeAnalyticsJob [-Account] <System.String> [-Name] <System.String>
 [-ScriptPath] <System.String> [[-Runtime] <System.String>] [[-CompileMode] <System.String>]
 [[-CompileOnly] [[-AnalyticsUnits] <System.Int32>] [[-Priority] <System.Int32>]
 [-ScriptParameter <System.Collections.IDictionary>]
 [-DefaultProfile <Microsoft.Azure.Commands.Common.Authentication.Abstractions.Core.IAzureContextContainer>]
 [<CommonParameters>]
```

### SubmitUSqlJob

```
Submit-AzDataLakeAnalyticsJob [-Account] <System.String> [-Name] <System.String>
 [-Script] <System.String> [[-Runtime] <System.String>] [[-CompileMode] <System.String>]
 [[-CompileOnly] [[-AnalyticsUnits] <System.Int32>] [[-Priority] <System.Int32>]
 [-ScriptParameter <System.Collections.IDictionary>]
 [-DefaultProfile <Microsoft.Azure.Commands.Common.Authentication.Abstractions.Core.IAzureContextContainer>]
 [<CommonParameters>]
```

### SubmitUSqlJobWithScriptPathAndRecurrence

```
Submit-AzDataLakeAnalyticsJob [-Account] <System.String> [-Name] <System.String>
 [-ScriptPath] <System.String> [[-Runtime] <System.String>] [[-CompileMode] <System.String>]
 [[-CompileOnly] [[-AnalyticsUnits] <System.Int32>] [[-Priority] <System.Int32>]
 [-ScriptParameter <System.Collections.IDictionary>] -RecurrenceId <System.Guid>
 [-RecurrenceName <System.String>]
 [-DefaultProfile <Microsoft.Azure.Commands.Common.Authentication.Abstractions.Core.IAzureContextContainer>]
 [<CommonParameters>]
```

### SubmitUSqlJobWithRecurrence

```
Submit-AzDataLakeAnalyticsJob [-Account] <System.String> [-Name] <System.String>
 [-Script] <System.String> [[-Runtime] <System.String>] [[-CompileMode] <System.String>]
 [[-CompileOnly] [[-AnalyticsUnits] <System.Int32>] [[-Priority] <System.Int32>]
 [-ScriptParameter <System.Collections.IDictionary>] -RecurrenceId <System.Guid>
 [-RecurrenceName <System.String>]
 [-DefaultProfile <Microsoft.Azure.Commands.Common.Authentication.Abstractions.Core.IAzureContextContainer>]
 [<CommonParameters>]
```

### SubmitUSqlJobWithScriptPathAndPipeline

```
Submit-AzDataLakeAnalyticsJob [-Account] <System.String> [-Name] <System.String>
 [-ScriptPath] <System.String> [[-Runtime] <System.String>] [[-CompileMode] <System.String>]
 [[-CompileOnly] [[-AnalyticsUnits] <System.Int32>] [[-Priority] <System.Int32>]
 [-ScriptParameter <System.Collections.IDictionary>] -RecurrenceId <System.Guid>
 [-RecurrenceName <System.String>] -PipelineId <System.Guid> [-PipelineName <System.String>]
 [-PipelineUri <System.String>] [-RunId <System.Nullable<System.Guid>>]
 [-DefaultProfile <Microsoft.Azure.Commands.Common.Authentication.Abstractions.Core.IAzureContextContainer>]
 [<CommonParameters>]
```

### SubmitUSqlJobWithPipeline

```
Submit-AzDataLakeAnalyticsJob [-Account] <System.String> [-Name] <System.String>
 [-Script] <System.String> [[-Runtime] <System.String>] [[-CompileMode] <System.String>]
 [[-CompileOnly] [[-AnalyticsUnits] <System.Int32>] [[-Priority] <System.Int32>]
 [-ScriptParameter <System.Collections.IDictionary>] -RecurrenceId <System.Guid>
 [-RecurrenceName <System.String>] -PipelineId <System.Guid> [-PipelineName <System.String>]
 [-PipelineUri <System.String>] [-RunId <System.Nullable<System.Guid>>]
 [-DefaultProfile <Microsoft.Azure.Commands.Common.Authentication.Abstractions.Core.IAzureContextContainer>]
 [<CommonParameters>]
```

## ALIASES

This cmdlet has the following aliases,
  {{Insert list of aliases}}

## DESCRIPTION

The Submit-AzDataLakeAnalyticsJob cmdlet submits an Azure Data Lake Analytics job.


## EXAMPLES

### Submit a job

This command submits a Data Lake Analytics job.




### Submit a job with script parameters

U-SQL script parameters are prepended above the main script contents, e.g.: DECLARE @Department string = "Sales"; DECLARE @NumRecords int = 1000; DECLARE @StartDateTime DateTime = new DateTime(2017, 12, 6, 0, 0, 0, 0);




## PARAMETERS

### -Account

Name of Data Lake Analytics account under which the job will be submitted.

```yaml
Type: System.String
DefaultValue: None
VariableLength: true
Globbing: false
ParameterValue: []
Aliases:
- AccountName
ParameterSets:
- Name: SubmitUSqlJobWithScriptPath
  Position: 0
  IsRequired: true
  ValueByPipeline: false
  ValueByPipelineByPropertyName: true
  ValueFromRemainingArguments: false
- Name: SubmitUSqlJob
  Position: 0
  IsRequired: true
  ValueByPipeline: false
  ValueByPipelineByPropertyName: true
  ValueFromRemainingArguments: false
- Name: SubmitUSqlJobWithScriptPathAndRecurrence
  Position: 0
  IsRequired: true
  ValueByPipeline: false
  ValueByPipelineByPropertyName: true
  ValueFromRemainingArguments: false
- Name: SubmitUSqlJobWithRecurrence
  Position: 0
  IsRequired: true
  ValueByPipeline: false
  ValueByPipelineByPropertyName: true
  ValueFromRemainingArguments: false
- Name: SubmitUSqlJobWithScriptPathAndPipeline
  Position: 0
  IsRequired: true
  ValueByPipeline: false
  ValueByPipelineByPropertyName: true
  ValueFromRemainingArguments: false
- Name: SubmitUSqlJobWithPipeline
  Position: 0
  IsRequired: true
  ValueByPipeline: false
  ValueByPipelineByPropertyName: true
  ValueFromRemainingArguments: false
DontShow: false
AcceptedValues: []
HelpMessage: ''
```

### -AnalyticsUnits

The analytics units to use for this job.
Typically, more analytics units dedicated to a script results in faster script execution time.

```yaml
Type: System.Int32
DefaultValue: None
VariableLength: true
Globbing: false
ParameterValue: []
Aliases:
- DegreeOfParallelism
ParameterSets:
- Name: SubmitUSqlJobWithScriptPath
  Position: 6
  IsRequired: false
  ValueByPipeline: false
  ValueByPipelineByPropertyName: true
  ValueFromRemainingArguments: false
- Name: SubmitUSqlJob
  Position: 6
  IsRequired: false
  ValueByPipeline: false
  ValueByPipelineByPropertyName: true
  ValueFromRemainingArguments: false
- Name: SubmitUSqlJobWithScriptPathAndRecurrence
  Position: 6
  IsRequired: false
  ValueByPipeline: false
  ValueByPipelineByPropertyName: true
  ValueFromRemainingArguments: false
- Name: SubmitUSqlJobWithRecurrence
  Position: 6
  IsRequired: false
  ValueByPipeline: false
  ValueByPipelineByPropertyName: true
  ValueFromRemainingArguments: false
- Name: SubmitUSqlJobWithScriptPathAndPipeline
  Position: 6
  IsRequired: false
  ValueByPipeline: false
  ValueByPipelineByPropertyName: true
  ValueFromRemainingArguments: false
- Name: SubmitUSqlJobWithPipeline
  Position: 6
  IsRequired: false
  ValueByPipeline: false
  ValueByPipelineByPropertyName: true
  ValueFromRemainingArguments: false
DontShow: false
AcceptedValues: []
HelpMessage: ''
```

### -CompileMode

The type of compilation to be done on this job.
 Valid values:  - Semantic (Only performs semantic checks and necessary sanity checks)

- Full (Full compilation)

- SingleBox (Full compilation performed locally)

```yaml
Type: System.String
DefaultValue: None
VariableLength: true
Globbing: false
ParameterValue: []
Aliases: []
ParameterSets:
- Name: SubmitUSqlJobWithScriptPath
  Position: 4
  IsRequired: false
  ValueByPipeline: false
  ValueByPipelineByPropertyName: true
  ValueFromRemainingArguments: false
- Name: SubmitUSqlJob
  Position: 4
  IsRequired: false
  ValueByPipeline: false
  ValueByPipelineByPropertyName: true
  ValueFromRemainingArguments: false
- Name: SubmitUSqlJobWithScriptPathAndRecurrence
  Position: 4
  IsRequired: false
  ValueByPipeline: false
  ValueByPipelineByPropertyName: true
  ValueFromRemainingArguments: false
- Name: SubmitUSqlJobWithRecurrence
  Position: 4
  IsRequired: false
  ValueByPipeline: false
  ValueByPipelineByPropertyName: true
  ValueFromRemainingArguments: false
- Name: SubmitUSqlJobWithScriptPathAndPipeline
  Position: 4
  IsRequired: false
  ValueByPipeline: false
  ValueByPipelineByPropertyName: true
  ValueFromRemainingArguments: false
- Name: SubmitUSqlJobWithPipeline
  Position: 4
  IsRequired: false
  ValueByPipeline: false
  ValueByPipelineByPropertyName: true
  ValueFromRemainingArguments: false
DontShow: false
AcceptedValues: []
HelpMessage: ''
```

### -CompileOnly

Indicates that the submission should only build the job and not execute if set to true.

```yaml
Type: System.Management.Automation.SwitchParameter
DefaultValue: False
VariableLength: true
Globbing: false
ParameterValue: []
Aliases: []
ParameterSets:
- Name: SubmitUSqlJobWithScriptPath
  Position: 5
  IsRequired: false
  ValueByPipeline: false
  ValueByPipelineByPropertyName: true
  ValueFromRemainingArguments: false
- Name: SubmitUSqlJob
  Position: 5
  IsRequired: false
  ValueByPipeline: false
  ValueByPipelineByPropertyName: true
  ValueFromRemainingArguments: false
- Name: SubmitUSqlJobWithScriptPathAndRecurrence
  Position: 5
  IsRequired: false
  ValueByPipeline: false
  ValueByPipelineByPropertyName: true
  ValueFromRemainingArguments: false
- Name: SubmitUSqlJobWithRecurrence
  Position: 5
  IsRequired: false
  ValueByPipeline: false
  ValueByPipelineByPropertyName: true
  ValueFromRemainingArguments: false
- Name: SubmitUSqlJobWithScriptPathAndPipeline
  Position: 5
  IsRequired: false
  ValueByPipeline: false
  ValueByPipelineByPropertyName: true
  ValueFromRemainingArguments: false
- Name: SubmitUSqlJobWithPipeline
  Position: 5
  IsRequired: false
  ValueByPipeline: false
  ValueByPipelineByPropertyName: true
  ValueFromRemainingArguments: false
DontShow: false
AcceptedValues: []
HelpMessage: ''
```

### -DefaultProfile

The credentials, account, tenant, and subscription used for communication with azure

```yaml
Type: Microsoft.Azure.Commands.Common.Authentication.Abstractions.Core.IAzureContextContainer
DefaultValue: None
VariableLength: true
Globbing: false
ParameterValue: []
Aliases:
- AzContext
- AzureRmContext
- AzureCredential
ParameterSets:
- Name: (All)
  Position: Named
  IsRequired: false
  ValueByPipeline: false
  ValueByPipelineByPropertyName: false
  ValueFromRemainingArguments: false
DontShow: false
AcceptedValues: []
HelpMessage: ''
```

### -Name

The friendly name of the job to submit.

```yaml
Type: System.String
DefaultValue: None
VariableLength: true
Globbing: false
ParameterValue: []
Aliases: []
ParameterSets:
- Name: SubmitUSqlJobWithScriptPath
  Position: 1
  IsRequired: true
  ValueByPipeline: false
  ValueByPipelineByPropertyName: true
  ValueFromRemainingArguments: false
- Name: SubmitUSqlJob
  Position: 1
  IsRequired: true
  ValueByPipeline: false
  ValueByPipelineByPropertyName: true
  ValueFromRemainingArguments: false
- Name: SubmitUSqlJobWithScriptPathAndRecurrence
  Position: 1
  IsRequired: true
  ValueByPipeline: false
  ValueByPipelineByPropertyName: true
  ValueFromRemainingArguments: false
- Name: SubmitUSqlJobWithRecurrence
  Position: 1
  IsRequired: true
  ValueByPipeline: false
  ValueByPipelineByPropertyName: true
  ValueFromRemainingArguments: false
- Name: SubmitUSqlJobWithScriptPathAndPipeline
  Position: 1
  IsRequired: true
  ValueByPipeline: false
  ValueByPipelineByPropertyName: true
  ValueFromRemainingArguments: false
- Name: SubmitUSqlJobWithPipeline
  Position: 1
  IsRequired: true
  ValueByPipeline: false
  ValueByPipelineByPropertyName: true
  ValueFromRemainingArguments: false
DontShow: false
AcceptedValues: []
HelpMessage: ''
```

### -PipelineId

An ID that indicates the submission of this job is a part of a set of recurring jobs and also associated with a job pipeline.

```yaml
Type: System.Guid
DefaultValue: None
VariableLength: true
Globbing: false
ParameterValue: []
Aliases: []
ParameterSets:
- Name: SubmitUSqlJobWithScriptPathAndPipeline
  Position: Named
  IsRequired: true
  ValueByPipeline: false
  ValueByPipelineByPropertyName: true
  ValueFromRemainingArguments: false
- Name: SubmitUSqlJobWithPipeline
  Position: Named
  IsRequired: true
  ValueByPipeline: false
  ValueByPipelineByPropertyName: true
  ValueFromRemainingArguments: false
DontShow: false
AcceptedValues: []
HelpMessage: ''
```

### -PipelineName

An optional friendly name for the pipeline associated with this job.

```yaml
Type: System.String
DefaultValue: None
VariableLength: true
Globbing: false
ParameterValue: []
Aliases: []
ParameterSets:
- Name: SubmitUSqlJobWithScriptPathAndPipeline
  Position: Named
  IsRequired: false
  ValueByPipeline: false
  ValueByPipelineByPropertyName: true
  ValueFromRemainingArguments: false
- Name: SubmitUSqlJobWithPipeline
  Position: Named
  IsRequired: false
  ValueByPipeline: false
  ValueByPipelineByPropertyName: true
  ValueFromRemainingArguments: false
DontShow: false
AcceptedValues: []
HelpMessage: ''
```

### -PipelineUri

An optional uri that links to the originating service associated with this pipeline.

```yaml
Type: System.String
DefaultValue: None
VariableLength: true
Globbing: false
ParameterValue: []
Aliases: []
ParameterSets:
- Name: SubmitUSqlJobWithScriptPathAndPipeline
  Position: Named
  IsRequired: false
  ValueByPipeline: false
  ValueByPipelineByPropertyName: true
  ValueFromRemainingArguments: false
- Name: SubmitUSqlJobWithPipeline
  Position: Named
  IsRequired: false
  ValueByPipeline: false
  ValueByPipelineByPropertyName: true
  ValueFromRemainingArguments: false
DontShow: false
AcceptedValues: []
HelpMessage: ''
```

### -Priority

The priority of the job.
If not specified, the priority is 1000.
A lower number indicates a higher job priority.

```yaml
Type: System.Int32
DefaultValue: None
VariableLength: true
Globbing: false
ParameterValue: []
Aliases: []
ParameterSets:
- Name: SubmitUSqlJobWithScriptPath
  Position: 7
  IsRequired: false
  ValueByPipeline: false
  ValueByPipelineByPropertyName: true
  ValueFromRemainingArguments: false
- Name: SubmitUSqlJob
  Position: 7
  IsRequired: false
  ValueByPipeline: false
  ValueByPipelineByPropertyName: true
  ValueFromRemainingArguments: false
- Name: SubmitUSqlJobWithScriptPathAndRecurrence
  Position: 7
  IsRequired: false
  ValueByPipeline: false
  ValueByPipelineByPropertyName: true
  ValueFromRemainingArguments: false
- Name: SubmitUSqlJobWithRecurrence
  Position: 7
  IsRequired: false
  ValueByPipeline: false
  ValueByPipelineByPropertyName: true
  ValueFromRemainingArguments: false
- Name: SubmitUSqlJobWithScriptPathAndPipeline
  Position: 7
  IsRequired: false
  ValueByPipeline: false
  ValueByPipelineByPropertyName: true
  ValueFromRemainingArguments: false
- Name: SubmitUSqlJobWithPipeline
  Position: 7
  IsRequired: false
  ValueByPipeline: false
  ValueByPipelineByPropertyName: true
  ValueFromRemainingArguments: false
DontShow: false
AcceptedValues: []
HelpMessage: ''
```

### -RecurrenceId

An ID that indicates the submission of this job is a part of a set of recurring jobs with the same recurrence ID.

```yaml
Type: System.Guid
DefaultValue: None
VariableLength: true
Globbing: false
ParameterValue: []
Aliases: []
ParameterSets:
- Name: SubmitUSqlJobWithScriptPathAndRecurrence
  Position: Named
  IsRequired: true
  ValueByPipeline: false
  ValueByPipelineByPropertyName: true
  ValueFromRemainingArguments: false
- Name: SubmitUSqlJobWithRecurrence
  Position: Named
  IsRequired: true
  ValueByPipeline: false
  ValueByPipelineByPropertyName: true
  ValueFromRemainingArguments: false
- Name: SubmitUSqlJobWithScriptPathAndPipeline
  Position: Named
  IsRequired: true
  ValueByPipeline: false
  ValueByPipelineByPropertyName: true
  ValueFromRemainingArguments: false
- Name: SubmitUSqlJobWithPipeline
  Position: Named
  IsRequired: true
  ValueByPipeline: false
  ValueByPipelineByPropertyName: true
  ValueFromRemainingArguments: false
DontShow: false
AcceptedValues: []
HelpMessage: ''
```

### -RecurrenceName

An optional friendly name for the recurrence correlation between jobs.

```yaml
Type: System.String
DefaultValue: None
VariableLength: true
Globbing: false
ParameterValue: []
Aliases: []
ParameterSets:
- Name: SubmitUSqlJobWithScriptPathAndRecurrence
  Position: Named
  IsRequired: false
  ValueByPipeline: false
  ValueByPipelineByPropertyName: true
  ValueFromRemainingArguments: false
- Name: SubmitUSqlJobWithRecurrence
  Position: Named
  IsRequired: false
  ValueByPipeline: false
  ValueByPipelineByPropertyName: true
  ValueFromRemainingArguments: false
- Name: SubmitUSqlJobWithScriptPathAndPipeline
  Position: Named
  IsRequired: false
  ValueByPipeline: false
  ValueByPipelineByPropertyName: true
  ValueFromRemainingArguments: false
- Name: SubmitUSqlJobWithPipeline
  Position: Named
  IsRequired: false
  ValueByPipeline: false
  ValueByPipelineByPropertyName: true
  ValueFromRemainingArguments: false
DontShow: false
AcceptedValues: []
HelpMessage: ''
```

### -RunId

An ID that identifies this specific run iteration of the pipeline.

```yaml
Type: System.Nullable<System.Guid>
DefaultValue: None
VariableLength: true
Globbing: false
ParameterValue: []
Aliases: []
ParameterSets:
- Name: SubmitUSqlJobWithScriptPathAndPipeline
  Position: Named
  IsRequired: false
  ValueByPipeline: false
  ValueByPipelineByPropertyName: true
  ValueFromRemainingArguments: false
- Name: SubmitUSqlJobWithPipeline
  Position: Named
  IsRequired: false
  ValueByPipeline: false
  ValueByPipelineByPropertyName: true
  ValueFromRemainingArguments: false
DontShow: false
AcceptedValues: []
HelpMessage: ''
```

### -Runtime

Optionally set the version of the runtime to use for the job.
If left unset, the default runtime is used.

```yaml
Type: System.String
DefaultValue: None
VariableLength: true
Globbing: false
ParameterValue: []
Aliases: []
ParameterSets:
- Name: SubmitUSqlJobWithScriptPath
  Position: 3
  IsRequired: false
  ValueByPipeline: false
  ValueByPipelineByPropertyName: true
  ValueFromRemainingArguments: false
- Name: SubmitUSqlJob
  Position: 3
  IsRequired: false
  ValueByPipeline: false
  ValueByPipelineByPropertyName: true
  ValueFromRemainingArguments: false
- Name: SubmitUSqlJobWithScriptPathAndRecurrence
  Position: 3
  IsRequired: false
  ValueByPipeline: false
  ValueByPipelineByPropertyName: true
  ValueFromRemainingArguments: false
- Name: SubmitUSqlJobWithRecurrence
  Position: 3
  IsRequired: false
  ValueByPipeline: false
  ValueByPipelineByPropertyName: true
  ValueFromRemainingArguments: false
- Name: SubmitUSqlJobWithScriptPathAndPipeline
  Position: 3
  IsRequired: false
  ValueByPipeline: false
  ValueByPipelineByPropertyName: true
  ValueFromRemainingArguments: false
- Name: SubmitUSqlJobWithPipeline
  Position: 3
  IsRequired: false
  ValueByPipeline: false
  ValueByPipelineByPropertyName: true
  ValueFromRemainingArguments: false
DontShow: false
AcceptedValues: []
HelpMessage: ''
```

### -Script

Script to execute (written inline).

```yaml
Type: System.String
DefaultValue: None
VariableLength: true
Globbing: false
ParameterValue: []
Aliases: []
ParameterSets:
- Name: SubmitUSqlJob
  Position: 2
  IsRequired: true
  ValueByPipeline: true
  ValueByPipelineByPropertyName: true
  ValueFromRemainingArguments: false
- Name: SubmitUSqlJobWithRecurrence
  Position: 2
  IsRequired: true
  ValueByPipeline: true
  ValueByPipelineByPropertyName: true
  ValueFromRemainingArguments: false
- Name: SubmitUSqlJobWithPipeline
  Position: 2
  IsRequired: true
  ValueByPipeline: true
  ValueByPipelineByPropertyName: true
  ValueFromRemainingArguments: false
DontShow: false
AcceptedValues: []
HelpMessage: ''
```

### -ScriptParameter

The script parameters for this job, as a dictionary of parameter names (string) to values (any combination of byte, sbyte, int, uint (or uint32), long, ulong (or uint64), float, double, decimal, short (or int16), ushort (or uint16), char, string, DateTime, bool, Guid, or byte[]).

```yaml
Type: System.Collections.IDictionary
DefaultValue: None
VariableLength: true
Globbing: false
ParameterValue: []
Aliases: []
ParameterSets:
- Name: SubmitUSqlJobWithScriptPath
  Position: Named
  IsRequired: false
  ValueByPipeline: false
  ValueByPipelineByPropertyName: true
  ValueFromRemainingArguments: false
- Name: SubmitUSqlJob
  Position: Named
  IsRequired: false
  ValueByPipeline: false
  ValueByPipelineByPropertyName: true
  ValueFromRemainingArguments: false
- Name: SubmitUSqlJobWithScriptPathAndRecurrence
  Position: Named
  IsRequired: false
  ValueByPipeline: false
  ValueByPipelineByPropertyName: true
  ValueFromRemainingArguments: false
- Name: SubmitUSqlJobWithRecurrence
  Position: Named
  IsRequired: false
  ValueByPipeline: false
  ValueByPipelineByPropertyName: true
  ValueFromRemainingArguments: false
- Name: SubmitUSqlJobWithScriptPathAndPipeline
  Position: Named
  IsRequired: false
  ValueByPipeline: false
  ValueByPipelineByPropertyName: true
  ValueFromRemainingArguments: false
- Name: SubmitUSqlJobWithPipeline
  Position: Named
  IsRequired: false
  ValueByPipeline: false
  ValueByPipelineByPropertyName: true
  ValueFromRemainingArguments: false
DontShow: false
AcceptedValues: []
HelpMessage: ''
```

### -ScriptPath

Path to the script file to submit.

```yaml
Type: System.String
DefaultValue: None
VariableLength: true
Globbing: false
ParameterValue: []
Aliases: []
ParameterSets:
- Name: SubmitUSqlJobWithScriptPath
  Position: 2
  IsRequired: true
  ValueByPipeline: false
  ValueByPipelineByPropertyName: true
  ValueFromRemainingArguments: false
- Name: SubmitUSqlJobWithScriptPathAndRecurrence
  Position: 2
  IsRequired: true
  ValueByPipeline: false
  ValueByPipelineByPropertyName: true
  ValueFromRemainingArguments: false
- Name: SubmitUSqlJobWithScriptPathAndPipeline
  Position: 2
  IsRequired: true
  ValueByPipeline: false
  ValueByPipelineByPropertyName: true
  ValueFromRemainingArguments: false
DontShow: false
AcceptedValues: []
HelpMessage: ''
```

### CommonParameters

This cmdlet supports the common parameters: -Debug, -ErrorAction, -ErrorVariable,
-InformationAction, -InformationVariable, -OutBuffer, -OutVariable, -PipelineVariable,
-ProgressAction, -Verbose, -WarningAction, -WarningVariable.
For more information, see
[about_CommonParameters](https://go.microsoft.com/fwlink/?LinkID=113216).

## INPUTS

###

## OUTPUTS

### Microsoft.Azure.Management.DataLake.Analytics.Models.JobInformation

## NOTES




## RELATED LINKS

[Online Version:](https://learn.microsoft.com/powershell/module/az.datalakeanalytics/submit-azdatalakeanalyticsjob)

[Get-AzDataLakeAnalyticsJob]()

[Stop-AzDataLakeAnalyticsJob]()

[Wait-AzDataLakeAnalyticsJob]()

