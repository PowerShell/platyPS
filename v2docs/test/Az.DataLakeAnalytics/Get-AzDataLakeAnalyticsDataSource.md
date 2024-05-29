---
content type: cmdlet
title: Get-AzDataLakeAnalyticsDataSource
Module Name: Az.DataLakeAnalytics
Locale: {{ fill in locale }}
PlatyPS schema version: 2024-05-01
HelpUri: https://learn.microsoft.com/powershell/module/az.datalakeanalytics/get-azdatalakeanalyticsdatasource
ms.date: 05/24/2024
external help file: Microsoft.Azure.PowerShell.Cmdlets.DataLakeAnalytics.dll-Help.xml
---

# Get-AzDataLakeAnalyticsDataSource

## SYNOPSIS

Gets a Data Lake Analytics data source.

## SYNTAX

### GetAllDataSources (Default)

```
Get-AzDataLakeAnalyticsDataSource [-Account] <System.String> [[-ResourceGroupName] <System.String>]
 [-DefaultProfile <Microsoft.Azure.Commands.Common.Authentication.Abstractions.Core.IAzureContextContainer>]
 [<CommonParameters>]
```

### GetDataLakeStoreAccount

```
Get-AzDataLakeAnalyticsDataSource [-Account] <System.String> [-DataLakeStore] <System.String>
 [[-ResourceGroupName] <System.String>]
 [-DefaultProfile <Microsoft.Azure.Commands.Common.Authentication.Abstractions.Core.IAzureContextContainer>]
 [<CommonParameters>]
```

### GetBlobStorageAccount

```
Get-AzDataLakeAnalyticsDataSource [-Account] <System.String> [-Blob] <System.String>
 [[-ResourceGroupName] <System.String>]
 [-DefaultProfile <Microsoft.Azure.Commands.Common.Authentication.Abstractions.Core.IAzureContextContainer>]
 [<CommonParameters>]
```

## ALIASES

This cmdlet has the following aliases,
  {{Insert list of aliases}}

## DESCRIPTION

The Get-AzDataLakeAnalyticsDataSource cmdlet gets an Azure Data Lake Analytics data source.


## EXAMPLES

### Get a data source from an account

This command gets a Data Lake Store data source named ContosoAdls from a Data Lake Analytics account.




### Get the list of Data Lake Store accounts in a Data Lake Analytics account

This command gets all Data Lake Store accounts from a Data Lake Analytics account.




## PARAMETERS

### -Account

Specifies the Data Lake Analytics account that this cmdlet gets data sources.

```yaml
Type: System.String
DefaultValue: None
VariableLength: true
Globbing: false
ParameterValue: []
Aliases:
- AccountName
ParameterSets:
- Name: GetDataLakeStoreAccount
  Position: 0
  IsRequired: true
  ValueByPipeline: false
  ValueByPipelineByPropertyName: true
  ValueFromRemainingArguments: false
- Name: GetBlobStorageAccount
  Position: 0
  IsRequired: true
  ValueByPipeline: false
  ValueByPipelineByPropertyName: true
  ValueFromRemainingArguments: false
- Name: GetAllDataSources
  Position: 0
  IsRequired: true
  ValueByPipeline: false
  ValueByPipelineByPropertyName: true
  ValueFromRemainingArguments: false
DontShow: false
AcceptedValues: []
HelpMessage: ''
```

### -Blob

Specifies the name of the Azure Blob Storage data source.

```yaml
Type: System.String
DefaultValue: None
VariableLength: true
Globbing: false
ParameterValue: []
Aliases:
- AzureBlob
ParameterSets:
- Name: GetBlobStorageAccount
  Position: 1
  IsRequired: true
  ValueByPipeline: false
  ValueByPipelineByPropertyName: true
  ValueFromRemainingArguments: false
DontShow: false
AcceptedValues: []
HelpMessage: ''
```

### -DataLakeStore

Specifies the name of the Data Lake Store account.

```yaml
Type: System.String
DefaultValue: None
VariableLength: true
Globbing: false
ParameterValue: []
Aliases: []
ParameterSets:
- Name: GetDataLakeStoreAccount
  Position: 1
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

### -ResourceGroupName

Specifies the resource group name that contains the data source.

```yaml
Type: System.String
DefaultValue: None
VariableLength: true
Globbing: false
ParameterValue: []
Aliases: []
ParameterSets:
- Name: GetDataLakeStoreAccount
  Position: 2
  IsRequired: false
  ValueByPipeline: false
  ValueByPipelineByPropertyName: true
  ValueFromRemainingArguments: false
- Name: GetBlobStorageAccount
  Position: 2
  IsRequired: false
  ValueByPipeline: false
  ValueByPipelineByPropertyName: true
  ValueFromRemainingArguments: false
- Name: GetAllDataSources
  Position: 1
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

### System.String

## OUTPUTS

###

## NOTES




## RELATED LINKS

[Online Version:](https://learn.microsoft.com/powershell/module/az.datalakeanalytics/get-azdatalakeanalyticsdatasource)

[Add-AzDataLakeAnalyticsDataSource]()

[Remove-AzDataLakeAnalyticsDataSource]()

[Set-AzDataLakeAnalyticsDataSource]()

