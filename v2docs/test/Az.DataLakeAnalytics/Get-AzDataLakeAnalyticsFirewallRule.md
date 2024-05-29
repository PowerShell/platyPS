---
content type: cmdlet
title: Get-AzDataLakeAnalyticsFirewallRule
Module Name: Az.DataLakeAnalytics
Locale: {{ fill in locale }}
PlatyPS schema version: 2024-05-01
HelpUri: https://learn.microsoft.com/powershell/module/az.datalakeanalytics/get-azdatalakeanalyticsfirewallrule
ms.date: 05/24/2024
external help file: Microsoft.Azure.PowerShell.Cmdlets.DataLakeAnalytics.dll-Help.xml
---

# Get-AzDataLakeAnalyticsFirewallRule

## SYNOPSIS

Retrieves a firewall rule or list of firewall rules from a Data Lake Analytics account.

## SYNTAX

### __AllParameterSets

```
Get-AzDataLakeAnalyticsFirewallRule [-Account] <System.String> [[-Name] <System.String>]
 [[-ResourceGroupName] <System.String>]
 [-DefaultProfile <Microsoft.Azure.Commands.Common.Authentication.Abstractions.Core.IAzureContextContainer>]
 [<CommonParameters>]
```

## ALIASES

This cmdlet has the following aliases,
  {{Insert list of aliases}}

## DESCRIPTION

The Get-AzDataLakeAnalyticsFirewallRule cmdlet retrieves a firewall rule or list of firewall rules from an Azure Data Lake Analytics account.


## EXAMPLES

### Get a firewall rule

This command gets the firewall rule named "my firewall rule" from account "ContosoAdlAcct"




### List all firewall rules

This command gets all firewall rules from account "ContosoAdlAcct"




## PARAMETERS

### -Account

The Data Lake Analytics account to get the firewall rule from

```yaml
Type: System.String
DefaultValue: None
VariableLength: true
Globbing: false
ParameterValue: []
Aliases:
- AccountName
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

The name of the firewall rule.

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
  IsRequired: false
  ValueByPipeline: false
  ValueByPipelineByPropertyName: true
  ValueFromRemainingArguments: false
DontShow: false
AcceptedValues: []
HelpMessage: ''
```

### -ResourceGroupName

Name of resource group under which want to retrieve the account.

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

### CommonParameters

This cmdlet supports the common parameters: -Debug, -ErrorAction, -ErrorVariable,
-InformationAction, -InformationVariable, -OutBuffer, -OutVariable, -PipelineVariable,
-ProgressAction, -Verbose, -WarningAction, -WarningVariable.
For more information, see
[about_CommonParameters](https://go.microsoft.com/fwlink/?LinkID=113216).

## INPUTS

### System.String

## OUTPUTS

### Microsoft.Azure.Commands.DataLakeAnalytics.Models.DataLakeAnalyticsFirewallRule

## NOTES




## RELATED LINKS

[Online Version:](https://learn.microsoft.com/powershell/module/az.datalakeanalytics/get-azdatalakeanalyticsfirewallrule)

