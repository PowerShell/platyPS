---
content type: cmdlet
title: New-AzDataLakeAnalyticsCatalogCredential
Module Name: Az.DataLakeAnalytics
Locale: {{ fill in locale }}
PlatyPS schema version: 2024-05-01
HelpUri: https://learn.microsoft.com/powershell/module/az.datalakeanalytics/new-azdatalakeanalyticscatalogcredential
ms.date: 05/24/2024
external help file: Microsoft.Azure.PowerShell.Cmdlets.DataLakeAnalytics.dll-Help.xml
---

# New-AzDataLakeAnalyticsCatalogCredential

## SYNOPSIS

Creates a new Azure Data Lake Analytics catalog credential.

## SYNTAX

### CreateByHostNameAndPort (Default)

```
New-AzDataLakeAnalyticsCatalogCredential [-Account] <System.String> [-DatabaseName] <System.String>
 [-CredentialName] <System.String> [-Credential] <System.Management.Automation.PSCredential>
 [-Uri] <System.Uri>
 [-DefaultProfile <Microsoft.Azure.Commands.Common.Authentication.Abstractions.Core.IAzureContextContainer>]
 [-WhatIf] [-Confirm] [<CommonParameters>]
```

### CreateByFullURI

```
New-AzDataLakeAnalyticsCatalogCredential [-Account] <System.String> [-DatabaseName] <System.String>
 [-CredentialName] <System.String> [-Credential] <System.Management.Automation.PSCredential>
 [-DatabaseHost] <System.String> [-Port] <System.Int32>
 [-DefaultProfile <Microsoft.Azure.Commands.Common.Authentication.Abstractions.Core.IAzureContextContainer>]
 [-WhatIf] [-Confirm] [<CommonParameters>]
```

## ALIASES

This cmdlet has the following aliases,
  {{Insert list of aliases}}

## DESCRIPTION

The New-AzDataLakeAnalyticsCatalogCredential cmdlet creates a new credential to use in an Azure Data Lake Analytics catalog for connecting to external data sources.


## EXAMPLES

### Create a credential for a catalog specifying host and port

This command creates the specified credential for the specified account, database, host and port using https protocol.




### Create a credential for a catalog specifying full URI

This command creates the specified credential for the specified account, database and external data source URI.




## PARAMETERS

### -Account

Specifies the Data Lake Analytics account name.

```yaml
Type: System.String
DefaultValue: None
VariableLength: true
Globbing: false
ParameterValue: []
Aliases:
- AccountName
ParameterSets:
- Name: CreateByFullURI
  Position: 0
  IsRequired: true
  ValueByPipeline: false
  ValueByPipelineByPropertyName: true
  ValueFromRemainingArguments: false
- Name: CreateByHostNameAndPort
  Position: 0
  IsRequired: true
  ValueByPipeline: false
  ValueByPipelineByPropertyName: true
  ValueFromRemainingArguments: false
DontShow: false
AcceptedValues: []
HelpMessage: ''
```

### -Confirm

Prompts you for confirmation before running the cmdlet.

```yaml
Type: System.Management.Automation.SwitchParameter
DefaultValue: False
VariableLength: true
Globbing: false
ParameterValue: []
Aliases:
- cf
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

### -Credential

Specifies the user name and corresponding password of the credential.

```yaml
Type: System.Management.Automation.PSCredential
DefaultValue: None
VariableLength: true
Globbing: false
ParameterValue: []
Aliases: []
ParameterSets:
- Name: CreateByFullURI
  Position: 3
  IsRequired: true
  ValueByPipeline: false
  ValueByPipelineByPropertyName: true
  ValueFromRemainingArguments: false
- Name: CreateByHostNameAndPort
  Position: 3
  IsRequired: true
  ValueByPipeline: false
  ValueByPipelineByPropertyName: true
  ValueFromRemainingArguments: false
DontShow: false
AcceptedValues: []
HelpMessage: ''
```

### -CredentialName

Specifies the name and password of the credential.

```yaml
Type: System.String
DefaultValue: None
VariableLength: true
Globbing: false
ParameterValue: []
Aliases: []
ParameterSets:
- Name: CreateByFullURI
  Position: 2
  IsRequired: true
  ValueByPipeline: false
  ValueByPipelineByPropertyName: true
  ValueFromRemainingArguments: false
- Name: CreateByHostNameAndPort
  Position: 2
  IsRequired: true
  ValueByPipeline: false
  ValueByPipelineByPropertyName: true
  ValueFromRemainingArguments: false
DontShow: false
AcceptedValues: []
HelpMessage: ''
```

### -DatabaseHost

Specifies the host name of the external data source the credential can connect to in the format mydatabase.contoso.com.

```yaml
Type: System.String
DefaultValue: None
VariableLength: true
Globbing: false
ParameterValue: []
Aliases: []
ParameterSets:
- Name: CreateByFullURI
  Position: 4
  IsRequired: true
  ValueByPipeline: false
  ValueByPipelineByPropertyName: true
  ValueFromRemainingArguments: false
DontShow: false
AcceptedValues: []
HelpMessage: ''
```

### -DatabaseName

Specifies the name of the database in the Data Lake Analytics account that the credential will be stored in.

```yaml
Type: System.String
DefaultValue: None
VariableLength: true
Globbing: false
ParameterValue: []
Aliases: []
ParameterSets:
- Name: CreateByFullURI
  Position: 1
  IsRequired: true
  ValueByPipeline: false
  ValueByPipelineByPropertyName: true
  ValueFromRemainingArguments: false
- Name: CreateByHostNameAndPort
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

### -Port

Specifies the port number used to connect to the specified DatabaseHost using this credential.

```yaml
Type: System.Int32
DefaultValue: None
VariableLength: true
Globbing: false
ParameterValue: []
Aliases: []
ParameterSets:
- Name: CreateByFullURI
  Position: 5
  IsRequired: true
  ValueByPipeline: false
  ValueByPipelineByPropertyName: true
  ValueFromRemainingArguments: false
DontShow: false
AcceptedValues: []
HelpMessage: ''
```

### -Uri

Specifies the full Uniform Resource Identifier (URI) of the external data source this credential can connect to.

```yaml
Type: System.Uri
DefaultValue: None
VariableLength: true
Globbing: false
ParameterValue: []
Aliases: []
ParameterSets:
- Name: CreateByHostNameAndPort
  Position: 4
  IsRequired: true
  ValueByPipeline: false
  ValueByPipelineByPropertyName: true
  ValueFromRemainingArguments: false
DontShow: false
AcceptedValues: []
HelpMessage: ''
```

### -WhatIf



```yaml
Type: System.Management.Automation.SwitchParameter
DefaultValue: False
VariableLength: true
Globbing: false
ParameterValue: []
Aliases:
- wi
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

### CommonParameters

This cmdlet supports the common parameters: -Debug, -ErrorAction, -ErrorVariable,
-InformationAction, -InformationVariable, -OutBuffer, -OutVariable, -PipelineVariable,
-ProgressAction, -Verbose, -WarningAction, -WarningVariable.
For more information, see
[about_CommonParameters](https://go.microsoft.com/fwlink/?LinkID=113216).

## INPUTS

###

## OUTPUTS

### Microsoft.Azure.Management.DataLake.Analytics.Models.USqlCredential

## NOTES




## RELATED LINKS

[Online Version:](https://learn.microsoft.com/powershell/module/az.datalakeanalytics/new-azdatalakeanalyticscatalogcredential)

