---
content type: cmdlet
title: New-AzDataLakeAnalyticsAccount
Module Name: Az.DataLakeAnalytics
Locale: {{ fill in locale }}
PlatyPS schema version: 2024-05-01
HelpUri: https://learn.microsoft.com/powershell/module/az.datalakeanalytics/new-azdatalakeanalyticsaccount
ms.date: 05/24/2024
external help file: Microsoft.Azure.PowerShell.Cmdlets.DataLakeAnalytics.dll-Help.xml
---

# New-AzDataLakeAnalyticsAccount

## SYNOPSIS

Creates a Data Lake Analytics account.

## SYNTAX

### __AllParameterSets

```
New-AzDataLakeAnalyticsAccount [-ResourceGroupName] <System.String> [-Name] <System.String>
 [-Location] <System.String> [-DefaultDataLakeStore] <System.String>
 [[-Tag] <System.Collections.Hashtable>] [-MaxAnalyticsUnits <System.Nullable<System.Int32>>]
 [-MaxJobCount <System.Nullable<System.Int32>>]
 [-QueryStoreRetention <System.Nullable<System.Int32>>]
 [-Tier <System.Nullable<Microsoft.Azure.Management.DataLake.Analytics.Models.TierType>>]
 [-DefaultProfile <Microsoft.Azure.Commands.Common.Authentication.Abstractions.Core.IAzureContextContainer>]
 [<CommonParameters>]
```

## ALIASES

This cmdlet has the following aliases,
  {{Insert list of aliases}}

## DESCRIPTION

The New-AzDataLakeAnalyticsAccount cmdlet creates an Azure Data Lake Analytics account.


## EXAMPLES

### Create a Data Lake Analytics account

This command creates a Data Lake Analytics account named ContosoAdlAccount that uses the ContosoAdlStore Data Store, in the resource group named ContosoOrg.




## PARAMETERS

### -DefaultDataLakeStore

Specifies the name of the Data Lake Store account to set as the default data source.

```yaml
Type: System.String
DefaultValue: None
VariableLength: true
Globbing: false
ParameterValue: []
Aliases: []
ParameterSets:
- Name: (All)
  Position: 3
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

### -Location

Specifies the location at which to create the Data Lake Analytics account.
Only East US 2 is supported at this time.

```yaml
Type: System.String
DefaultValue: None
VariableLength: true
Globbing: false
ParameterValue: []
Aliases: []
ParameterSets:
- Name: (All)
  Position: 2
  IsRequired: true
  ValueByPipeline: false
  ValueByPipelineByPropertyName: true
  ValueFromRemainingArguments: false
DontShow: false
AcceptedValues: []
HelpMessage: ''
```

### -MaxAnalyticsUnits

The optional maximum supported analytics units for this account.

```yaml
Type: System.Nullable<System.Int32>
DefaultValue: None
VariableLength: true
Globbing: false
ParameterValue: []
Aliases:
- MaxDegreeOfParallelism
ParameterSets:
- Name: (All)
  Position: Named
  IsRequired: false
  ValueByPipeline: false
  ValueByPipelineByPropertyName: true
  ValueFromRemainingArguments: false
DontShow: false
AcceptedValues: []
HelpMessage: ''
```

### -MaxJobCount

The optional maximum supported jobs running under the account at the same time.
If none is specified, defaults to 3

```yaml
Type: System.Nullable<System.Int32>
DefaultValue: None
VariableLength: true
Globbing: false
ParameterValue: []
Aliases: []
ParameterSets:
- Name: (All)
  Position: Named
  IsRequired: false
  ValueByPipeline: false
  ValueByPipelineByPropertyName: true
  ValueFromRemainingArguments: false
DontShow: false
AcceptedValues: []
HelpMessage: ''
```

### -Name

Specifies the Data Lake Analytics account name.

```yaml
Type: System.String
DefaultValue: None
VariableLength: true
Globbing: false
ParameterValue: []
Aliases: []
ParameterSets:
- Name: (All)
  Position: 1
  IsRequired: true
  ValueByPipeline: false
  ValueByPipelineByPropertyName: true
  ValueFromRemainingArguments: false
DontShow: false
AcceptedValues: []
HelpMessage: ''
```

### -QueryStoreRetention

The optional number of days that job metadata is retained.
If none specified, the default is 30 days.

```yaml
Type: System.Nullable<System.Int32>
DefaultValue: None
VariableLength: true
Globbing: false
ParameterValue: []
Aliases: []
ParameterSets:
- Name: (All)
  Position: Named
  IsRequired: false
  ValueByPipeline: false
  ValueByPipelineByPropertyName: true
  ValueFromRemainingArguments: false
DontShow: false
AcceptedValues: []
HelpMessage: ''
```

### -ResourceGroupName

Specifies the resource group name of the Data Lake Analytics account.
To create a resource group, use the New-AzResourceGroup cmdlet.

```yaml
Type: System.String
DefaultValue: None
VariableLength: true
Globbing: false
ParameterValue: []
Aliases: []
ParameterSets:
- Name: (All)
  Position: 0
  IsRequired: true
  ValueByPipeline: false
  ValueByPipelineByPropertyName: true
  ValueFromRemainingArguments: false
DontShow: false
AcceptedValues: []
HelpMessage: ''
```

### -Tag

A string,string dictionary of tags associated with this account

```yaml
Type: System.Collections.Hashtable
DefaultValue: None
VariableLength: true
Globbing: false
ParameterValue: []
Aliases: []
ParameterSets:
- Name: (All)
  Position: 4
  IsRequired: false
  ValueByPipeline: false
  ValueByPipelineByPropertyName: true
  ValueFromRemainingArguments: false
DontShow: false
AcceptedValues: []
HelpMessage: ''
```

### -Tier

The desired commitment tier for this account to use.

```yaml
Type: System.Nullable<Microsoft.Azure.Management.DataLake.Analytics.Models.TierType>
DefaultValue: None
VariableLength: true
Globbing: false
ParameterValue: []
Aliases: []
ParameterSets:
- Name: (All)
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

### Microsoft.Azure.Commands.DataLakeAnalytics.Models.PSDataLakeAnalyticsAccount

## NOTES




## RELATED LINKS

[Online Version:](https://learn.microsoft.com/powershell/module/az.datalakeanalytics/new-azdatalakeanalyticsaccount)

[Get-AzDataLakeAnalyticsAccount]()

[Remove-AzDataLakeAnalyticsAccount]()

[Set-AzDataLakeAnalyticsAccount]()

[Test-AzDataLakeAnalyticsAccount]()

