---
content type: cmdlet
title: Set-AzDataLakeAnalyticsAccount
Module Name: Az.DataLakeAnalytics
Locale: {{ fill in locale }}
PlatyPS schema version: 2024-05-01
HelpUri: https://learn.microsoft.com/powershell/module/az.datalakeanalytics/set-azdatalakeanalyticsaccount
ms.date: 05/24/2024
external help file: C:\Users\sewhee\Documents\PowerShell\Modules\Az.DataLakeAnalytics\1.0.3\Microsoft.Azure.PowerShell.Cmdlets.DataLakeAnalytics.dll-Help.xml
---

# Set-AzDataLakeAnalyticsAccount

## SYNOPSIS

Modifies a Data Lake Analytics account.

## SYNTAX

### __AllParameterSets

```
Set-AzDataLakeAnalyticsAccount [-Name] <System.String> [[-Tag] <System.Collections.Hashtable>]
 [[-ResourceGroupName] <System.String>] [-MaxAnalyticsUnits <System.Nullable<System.Int32>>]
 [-MaxJobCount <System.Nullable<System.Int32>>]
 [-QueryStoreRetention <System.Nullable<System.Int32>>]
 [-Tier <System.Nullable<Microsoft.Azure.Management.DataLake.Analytics.Models.TierType>>]
 [-FirewallState <System.Nullable<Microsoft.Azure.Management.DataLake.Analytics.Models.FirewallState>>]
 [-AllowAzureIpState <System.Nullable<Microsoft.Azure.Management.DataLake.Analytics.Models.FirewallAllowAzureIpsState>>]
 [-DefaultProfile <Microsoft.Azure.Commands.Common.Authentication.Abstractions.Core.IAzureContextContainer>]
 [<CommonParameters>]
```

## ALIASES

This cmdlet has the following aliases,
  {{Insert list of aliases}}

## DESCRIPTION

The Set-AzDataLakeAnalyticsAccount cmdlet modifies an Azure Data Lake Analytics account.


## EXAMPLES

### Modify the data source of an account

This command changes the default data source and the Tags property of the account.




### Example 2






## PARAMETERS

### -AllowAzureIpState

Optionally allow/block Azure originating IPs through the firewall.

```yaml
Type: System.Nullable<Microsoft.Azure.Management.DataLake.Analytics.Models.FirewallAllowAzureIpsState>
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

### -FirewallState

Optionally enable/disable existing firewall rules.

```yaml
Type: System.Nullable<Microsoft.Azure.Management.DataLake.Analytics.Models.FirewallState>
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

### -MaxAnalyticsUnits

The optional maximum supported analytics units to update the account with.

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

The optional maximum supported jobs running under the account at the same time to set.

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
  Position: 0
  IsRequired: true
  ValueByPipeline: false
  ValueByPipelineByPropertyName: true
  ValueFromRemainingArguments: false
DontShow: false
AcceptedValues: []
HelpMessage: ''
```

### -QueryStoreRetention

The optional number of days that job metadata is retained to set in the account.

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
  IsRequired: false
  ValueByPipeline: false
  ValueByPipelineByPropertyName: true
  ValueFromRemainingArguments: false
DontShow: false
AcceptedValues: []
HelpMessage: ''
```

### -Tag

A string,string dictionary of tags associated with this account that should replace the current set of tags

```yaml
Type: System.Collections.Hashtable
DefaultValue: None
VariableLength: true
Globbing: false
ParameterValue: []
Aliases: []
ParameterSets:
- Name: (All)
  Position: 1
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

[Online Version:](https://learn.microsoft.com/powershell/module/az.datalakeanalytics/set-azdatalakeanalyticsaccount)

[Get-AzDataLakeAnalyticsAccount]()

[New-AzDataLakeAnalyticsAccount]()

[Remove-AzDataLakeAnalyticsAccount]()

[Test-AzDataLakeAnalyticsAccount]()

