---
content type: cmdlet
title: Get-AzDataLakeAnalyticsJobRecurrence
Module Name: Az.DataLakeAnalytics
Locale: {{ fill in locale }}
PlatyPS schema version: 2024-05-01
HelpUri: https://learn.microsoft.com/powershell/module/az.datalakeanalytics/get-azdatalakeanalyticsjobrecurrence
ms.date: 05/24/2024
external help file: Microsoft.Azure.PowerShell.Cmdlets.DataLakeAnalytics.dll-Help.xml
---

# Get-AzDataLakeAnalyticsJobRecurrence

## SYNOPSIS

Gets a Data Lake Analytics Job recurrence or recurrences.

## SYNTAX

### GetAllInAccount (Default)

```
Get-AzDataLakeAnalyticsJobRecurrence [-Account] <System.String>
 [-SubmittedAfter <System.Nullable<System.DateTimeOffset>>]
 [-SubmittedBefore <System.Nullable<System.DateTimeOffset>>]
 [-DefaultProfile <Microsoft.Azure.Commands.Common.Authentication.Abstractions.Core.IAzureContextContainer>]
 [<CommonParameters>]
```

### GetBySpecificJobRecurrence

```
Get-AzDataLakeAnalyticsJobRecurrence [-Account] <System.String> [-RecurrenceId] <System.Guid>
 [-SubmittedAfter <System.Nullable<System.DateTimeOffset>>]
 [-SubmittedBefore <System.Nullable<System.DateTimeOffset>>]
 [-DefaultProfile <Microsoft.Azure.Commands.Common.Authentication.Abstractions.Core.IAzureContextContainer>]
 [<CommonParameters>]
```

## ALIASES

This cmdlet has the following aliases,
  {{Insert list of aliases}}

## DESCRIPTION

The Get-AzDataLakeAnalyticsJobRecurrence gets a specified Azure Data Lake Analytics Job recurrence or a list of recurrence.


## EXAMPLES

### Get a specified recurrence

This command gets the specified job recurrence with id '83cb7ad2-3523-4b82-b909-d478b0d8aea3' in account 'contosoadla'.




### Get a list of all recurrences in the account

This command gets a list of all recurrences in the account "contosoadla"




## PARAMETERS

### -Account

Name of the Data Lake Analytics account name under which want to retrieve the job recurrence.

```yaml
Type: System.String
DefaultValue: None
VariableLength: true
Globbing: false
ParameterValue: []
Aliases:
- AccountName
ParameterSets:
- Name: GetAllInAccount
  Position: 0
  IsRequired: true
  ValueByPipeline: false
  ValueByPipelineByPropertyName: true
  ValueFromRemainingArguments: false
- Name: GetBySpecificJobRecurrence
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

### -RecurrenceId

ID of the specific job recurrence to return information for.

```yaml
Type: System.Guid
DefaultValue: None
VariableLength: true
Globbing: false
ParameterValue: []
Aliases:
- Id
ParameterSets:
- Name: GetBySpecificJobRecurrence
  Position: 1
  IsRequired: true
  ValueByPipeline: true
  ValueByPipelineByPropertyName: true
  ValueFromRemainingArguments: false
DontShow: false
AcceptedValues: []
HelpMessage: ''
```

### -SubmittedAfter

An optional filter which returns job recurrence(s) only submitted after the specified time.

```yaml
Type: System.Nullable<System.DateTimeOffset>
DefaultValue: None
VariableLength: true
Globbing: false
ParameterValue: []
Aliases: []
ParameterSets:
- Name: GetAllInAccount
  Position: Named
  IsRequired: false
  ValueByPipeline: false
  ValueByPipelineByPropertyName: true
  ValueFromRemainingArguments: false
- Name: GetBySpecificJobRecurrence
  Position: Named
  IsRequired: false
  ValueByPipeline: false
  ValueByPipelineByPropertyName: true
  ValueFromRemainingArguments: false
DontShow: false
AcceptedValues: []
HelpMessage: ''
```

### -SubmittedBefore

An optional filter which returns job recurrence(s) only submitted before the specified time.

```yaml
Type: System.Nullable<System.DateTimeOffset>
DefaultValue: None
VariableLength: true
Globbing: false
ParameterValue: []
Aliases: []
ParameterSets:
- Name: GetAllInAccount
  Position: Named
  IsRequired: false
  ValueByPipeline: false
  ValueByPipelineByPropertyName: true
  ValueFromRemainingArguments: false
- Name: GetBySpecificJobRecurrence
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

### Microsoft.Azure.Commands.DataLakeAnalytics.Models.PSJobRecurrenceInformation

## NOTES




## RELATED LINKS

[Online Version:](https://learn.microsoft.com/powershell/module/az.datalakeanalytics/get-azdatalakeanalyticsjobrecurrence)

