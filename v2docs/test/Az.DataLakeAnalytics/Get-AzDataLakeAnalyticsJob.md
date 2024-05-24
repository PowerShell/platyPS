---
content type: cmdlet
title: Get-AzDataLakeAnalyticsJob
Module Name: Az.DataLakeAnalytics
Locale: {{ fill in locale }}
PlatyPS schema version: 2024-05-01
HelpUri: https://learn.microsoft.com/powershell/module/az.datalakeanalytics/get-azdatalakeanalyticsjob
ms.date: 05/24/2024
external help file: Microsoft.Azure.PowerShell.Cmdlets.DataLakeAnalytics.dll-Help.xml
---

# Get-AzDataLakeAnalyticsJob

## SYNOPSIS

Gets a Data Lake Analytics job.

## SYNTAX

### GetAllInResourceGroupAndAccount (Default)

```
Get-AzDataLakeAnalyticsJob [-Account] <System.String> [[-Name] <System.String>]
 [[-Submitter] <System.String>] [[-SubmittedAfter] <System.Nullable<System.DateTimeOffset>>]
 [[-SubmittedBefore] <System.Nullable<System.DateTimeOffset>>]
 [[-State] <Microsoft.Azure.Management.DataLake.Analytics.Models.JobState[]>]
 [[-Result] <Microsoft.Azure.Management.DataLake.Analytics.Models.JobResult[]>]
 [-Top <System.Nullable<System.Int32>>] [-PipelineId <System.Nullable<System.Guid>>]
 [-RecurrenceId <System.Nullable<System.Guid>>]
 [-DefaultProfile <Microsoft.Azure.Commands.Common.Authentication.Abstractions.Core.IAzureContextContainer>]
 [<CommonParameters>]
```

### GetBySpecificJobInformation

```
Get-AzDataLakeAnalyticsJob [-Account] <System.String> [-JobId] <System.Guid>
 [[-Include] <Microsoft.Azure.Commands.DataLakeAnalytics.Models.DataLakeAnalyticsEnums+ExtendedJobData>]
 [-DefaultProfile <Microsoft.Azure.Commands.Common.Authentication.Abstractions.Core.IAzureContextContainer>]
 [<CommonParameters>]
```

## ALIASES

This cmdlet has the following aliases,
  {{Insert list of aliases}}

## DESCRIPTION

The Get-AzDataLakeAnalyticsJob cmdlet gets an Azure Data Lake Analytics job.
If you do not specify a job, this cmdlet gets all jobs.


## EXAMPLES

### Get a specified job

This command gets the job with the specified ID.




### Get jobs submitted in the past week

This command gets jobs submitted in the past week.




## PARAMETERS

### -Account

Specifies the name of a Data Lake Analytics account.

```yaml
Type: System.String
DefaultValue: None
VariableLength: true
Globbing: false
ParameterValue: []
Aliases:
- AccountName
ParameterSets:
- Name: GetAllInResourceGroupAndAccount
  Position: 0
  IsRequired: true
  ValueByPipeline: false
  ValueByPipelineByPropertyName: true
  ValueFromRemainingArguments: false
- Name: GetBySpecificJobInformation
  Position: 0
  IsRequired: true
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

### -Include

Specifies options that indicate the type of additional information to retrieve about the job.
The acceptable values for this parameter are: - None

- DebugInfo

- Statistics

- All

```yaml
Type: Microsoft.Azure.Commands.DataLakeAnalytics.Models.DataLakeAnalyticsEnums+ExtendedJobData
DefaultValue: None
VariableLength: true
Globbing: false
ParameterValue: []
Aliases: []
ParameterSets:
- Name: GetBySpecificJobInformation
  Position: 2
  IsRequired: false
  ValueByPipeline: false
  ValueByPipelineByPropertyName: true
  ValueFromRemainingArguments: false
DontShow: false
AcceptedValues: []
HelpMessage: ''
```

### -JobId

Specifies the ID of the job to get.

```yaml
Type: System.Guid
DefaultValue: None
VariableLength: true
Globbing: false
ParameterValue: []
Aliases: []
ParameterSets:
- Name: GetBySpecificJobInformation
  Position: 1
  IsRequired: true
  ValueByPipeline: true
  ValueByPipelineByPropertyName: true
  ValueFromRemainingArguments: false
DontShow: false
AcceptedValues: []
HelpMessage: ''
```

### -Name

Specifies a name to use to filter the job list results.
The acceptable values for this parameter are: - None

- DebugInfo

- Statistics

- All

```yaml
Type: System.String
DefaultValue: None
VariableLength: true
Globbing: false
ParameterValue: []
Aliases: []
ParameterSets:
- Name: GetAllInResourceGroupAndAccount
  Position: 1
  IsRequired: false
  ValueByPipeline: false
  ValueByPipelineByPropertyName: true
  ValueFromRemainingArguments: false
DontShow: false
AcceptedValues: []
HelpMessage: ''
```

### -PipelineId

An optional ID that indicates only jobs part of the specified pipeline should be returned.

```yaml
Type: System.Nullable<System.Guid>
DefaultValue: None
VariableLength: true
Globbing: false
ParameterValue: []
Aliases: []
ParameterSets:
- Name: GetAllInResourceGroupAndAccount
  Position: Named
  IsRequired: false
  ValueByPipeline: false
  ValueByPipelineByPropertyName: true
  ValueFromRemainingArguments: false
DontShow: false
AcceptedValues: []
HelpMessage: ''
```

### -RecurrenceId

An optional ID that indicates only jobs part of the specified recurrence should be returned.

```yaml
Type: System.Nullable<System.Guid>
DefaultValue: None
VariableLength: true
Globbing: false
ParameterValue: []
Aliases: []
ParameterSets:
- Name: GetAllInResourceGroupAndAccount
  Position: Named
  IsRequired: false
  ValueByPipeline: false
  ValueByPipelineByPropertyName: true
  ValueFromRemainingArguments: false
DontShow: false
AcceptedValues: []
HelpMessage: ''
```

### -Result

Specifies a result filter for the job results.
The acceptable values for this parameter are: - None

- Cancelled

- Failed

- Succeeded

```yaml
Type: Microsoft.Azure.Management.DataLake.Analytics.Models.JobResult[]
DefaultValue: None
VariableLength: true
Globbing: false
ParameterValue: []
Aliases: []
ParameterSets:
- Name: GetAllInResourceGroupAndAccount
  Position: 6
  IsRequired: false
  ValueByPipeline: false
  ValueByPipelineByPropertyName: true
  ValueFromRemainingArguments: false
DontShow: false
AcceptedValues: []
HelpMessage: ''
```

### -State

Specifies a state filter for the job results.
The acceptable values for this parameter are: - Accepted

- New

- Compiling

- Scheduling

- Queued

- Starting

- Paused

- Running

- Ended

```yaml
Type: Microsoft.Azure.Management.DataLake.Analytics.Models.JobState[]
DefaultValue: None
VariableLength: true
Globbing: false
ParameterValue: []
Aliases: []
ParameterSets:
- Name: GetAllInResourceGroupAndAccount
  Position: 5
  IsRequired: false
  ValueByPipeline: false
  ValueByPipelineByPropertyName: true
  ValueFromRemainingArguments: false
DontShow: false
AcceptedValues: []
HelpMessage: ''
```

### -SubmittedAfter

Specifies a date filter.
Use this parameter to filter the job list result to jobs submitted after the specified date.

```yaml
Type: System.Nullable<System.DateTimeOffset>
DefaultValue: None
VariableLength: true
Globbing: false
ParameterValue: []
Aliases: []
ParameterSets:
- Name: GetAllInResourceGroupAndAccount
  Position: 3
  IsRequired: false
  ValueByPipeline: false
  ValueByPipelineByPropertyName: true
  ValueFromRemainingArguments: false
DontShow: false
AcceptedValues: []
HelpMessage: ''
```

### -SubmittedBefore

Specifies a date filter.
Use this parameter to filter the job list result to jobs submitted before the specified date.

```yaml
Type: System.Nullable<System.DateTimeOffset>
DefaultValue: None
VariableLength: true
Globbing: false
ParameterValue: []
Aliases: []
ParameterSets:
- Name: GetAllInResourceGroupAndAccount
  Position: 4
  IsRequired: false
  ValueByPipeline: false
  ValueByPipelineByPropertyName: true
  ValueFromRemainingArguments: false
DontShow: false
AcceptedValues: []
HelpMessage: ''
```

### -Submitter

Specifies the email address of a user.
Use this parameter to filter the job list results to jobs submitted by a specified user.

```yaml
Type: System.String
DefaultValue: None
VariableLength: true
Globbing: false
ParameterValue: []
Aliases: []
ParameterSets:
- Name: GetAllInResourceGroupAndAccount
  Position: 2
  IsRequired: false
  ValueByPipeline: false
  ValueByPipelineByPropertyName: true
  ValueFromRemainingArguments: false
DontShow: false
AcceptedValues: []
HelpMessage: ''
```

### -Top

An optional value which indicates the number of jobs to return.
Default value is 500

```yaml
Type: System.Nullable<System.Int32>
DefaultValue: None
VariableLength: true
Globbing: false
ParameterValue: []
Aliases: []
ParameterSets:
- Name: GetAllInResourceGroupAndAccount
  Position: Named
  IsRequired: false
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

[Online Version:](https://learn.microsoft.com/powershell/module/az.datalakeanalytics/get-azdatalakeanalyticsjob)

[Stop-AzDataLakeAnalyticsJob]()

[Submit-AzDataLakeAnalyticsJob]()

[Wait-AzDataLakeAnalyticsJob]()

