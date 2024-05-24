---
content type: cmdlet
title: Get-AzDataLakeAnalyticsJobPipeline
Module Name: Az.DataLakeAnalytics
Locale: {{ fill in locale }}
PlatyPS schema version: 2024-05-01
HelpUri: https://learn.microsoft.com/powershell/module/az.datalakeanalytics/get-azdatalakeanalyticsjobpipeline
ms.date: 05/24/2024
external help file: Microsoft.Azure.PowerShell.Cmdlets.DataLakeAnalytics.dll-Help.xml
---

# Get-AzDataLakeAnalyticsJobPipeline

## SYNOPSIS

Gets a Data Lake Analytics Job pipeline or pipelines.

## SYNTAX

### GetAllInAccount (Default)

```
Get-AzDataLakeAnalyticsJobPipeline [-Account] <System.String>
 [-SubmittedAfter <System.Nullable<System.DateTimeOffset>>]
 [-SubmittedBefore <System.Nullable<System.DateTimeOffset>>]
 [-DefaultProfile <Microsoft.Azure.Commands.Common.Authentication.Abstractions.Core.IAzureContextContainer>]
 [<CommonParameters>]
```

### GetBySpecificJobPipeline

```
Get-AzDataLakeAnalyticsJobPipeline [-Account] <System.String> [-PipelineId] <System.Guid>
 [-SubmittedAfter <System.Nullable<System.DateTimeOffset>>]
 [-SubmittedBefore <System.Nullable<System.DateTimeOffset>>]
 [-DefaultProfile <Microsoft.Azure.Commands.Common.Authentication.Abstractions.Core.IAzureContextContainer>]
 [<CommonParameters>]
```

## ALIASES

This cmdlet has the following aliases,
  {{Insert list of aliases}}

## DESCRIPTION

The Get-AzDataLakeAnalyticsJobPipeline gets a specified Azure Data Lake Analytics Job pipeline or a list of pipelines.


## EXAMPLES

### Get a specified pipeline

This command gets the specified pipeline with id '83cb7ad2-3523-4b82-b909-d478b0d8aea3' in account 'contosoadla'.




### Get a list of all pipelines in the account

This command gets a list of all pipelines in the account "contosoadla"




## PARAMETERS

### -Account

Name of the Data Lake Analytics account name under which want to retrieve the job pipeline.

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
- Name: GetBySpecificJobPipeline
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

### -PipelineId

ID of the specific job pipeline to return information for.

```yaml
Type: System.Guid
DefaultValue: None
VariableLength: true
Globbing: false
ParameterValue: []
Aliases:
- Id
ParameterSets:
- Name: GetBySpecificJobPipeline
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

An optional filter which returns job pipeline(s) only submitted after the specified time.

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
- Name: GetBySpecificJobPipeline
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

An optional filter which returns job pipeline(s) only submitted before the specified time.

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
- Name: GetBySpecificJobPipeline
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

### Microsoft.Azure.Commands.DataLakeAnalytics.Models.PSJobPipelineInformation

## NOTES




## RELATED LINKS

[Online Version:](https://learn.microsoft.com/powershell/module/az.datalakeanalytics/get-azdatalakeanalyticsjobpipeline)

