---
content type: cmdlet
title: Get-AzDataLakeAnalyticsCatalogItemAclEntry
Module Name: Az.DataLakeAnalytics
Locale: {{ fill in locale }}
PlatyPS schema version: 2024-05-01
HelpUri: https://learn.microsoft.com/powershell/module/az.datalakeanalytics/get-azdatalakeanalyticscatalogitemaclentry
ms.date: 05/24/2024
external help file: Microsoft.Azure.PowerShell.Cmdlets.DataLakeAnalytics.dll-Help.xml
---

# Get-AzDataLakeAnalyticsCatalogItemAclEntry

## SYNOPSIS

Gets an entry in the ACL of a catalog or catalog item in Data Lake Analytics.

## SYNTAX

### GetCatalogAclEntry (Default)

```
Get-AzDataLakeAnalyticsCatalogItemAclEntry [-Account] <System.String>
 [-DefaultProfile <Microsoft.Azure.Commands.Common.Authentication.Abstractions.Core.IAzureContextContainer>]
 [<CommonParameters>]
```

### GetCatalogAclEntryForUserOwner

```
Get-AzDataLakeAnalyticsCatalogItemAclEntry [-Account] <System.String> -UserOwner]
 [-DefaultProfile <Microsoft.Azure.Commands.Common.Authentication.Abstractions.Core.IAzureContextContainer>]
 [<CommonParameters>]
```

### GetCatalogAclEntryForGroupOwner

```
Get-AzDataLakeAnalyticsCatalogItemAclEntry [-Account] <System.String> -GroupOwner]
 [-DefaultProfile <Microsoft.Azure.Commands.Common.Authentication.Abstractions.Core.IAzureContextContainer>]
 [<CommonParameters>]
```

### GetCatalogItemAclEntry

```
Get-AzDataLakeAnalyticsCatalogItemAclEntry [-Account] <System.String> -ItemType <System.String>
 -Path <Microsoft.Azure.Commands.DataLakeAnalytics.Models.CatalogPathInstance>
 [-DefaultProfile <Microsoft.Azure.Commands.Common.Authentication.Abstractions.Core.IAzureContextContainer>]
 [<CommonParameters>]
```

### GetCatalogItemAclEntryForUserOwner

```
Get-AzDataLakeAnalyticsCatalogItemAclEntry [-Account] <System.String> -UserOwner]
 -ItemType <System.String>
 -Path <Microsoft.Azure.Commands.DataLakeAnalytics.Models.CatalogPathInstance>
 [-DefaultProfile <Microsoft.Azure.Commands.Common.Authentication.Abstractions.Core.IAzureContextContainer>]
 [<CommonParameters>]
```

### GetCatalogItemAclEntryForGroupOwner

```
Get-AzDataLakeAnalyticsCatalogItemAclEntry [-Account] <System.String> -GroupOwner]
 -ItemType <System.String>
 -Path <Microsoft.Azure.Commands.DataLakeAnalytics.Models.CatalogPathInstance>
 [-DefaultProfile <Microsoft.Azure.Commands.Common.Authentication.Abstractions.Core.IAzureContextContainer>]
 [<CommonParameters>]
```

## ALIASES

This cmdlet has the following aliases,
  {{Insert list of aliases}}

## DESCRIPTION

The Get-AzDataLakeAnalyticsCatalogItemAclEntry cmdlet gets a list of entries (ACEs) in the access control list (ACL) of a catalog or catalog item in Data Lake Analytics.


## EXAMPLES

### Get the ACL for a catalog

This command gets the ACL for the catalog of the specified Data Lake Analytics account




### Get the ACL entry of user owner for a catalog

This command gets ACL entry of the user owner for the catalog of the specified Data Lake Analytics account




### Get the ACL entry of group owner for a catalog

This command gets ACL entry of the group owner for the catalog of the specified Data Lake Analytics account




### Get the ACL for a database

This command gets the ACL for the database of the specified Data Lake Analytics account




### Get the ACL entry of user owner for a database

This command gets the ACL entry of the user owner for the database of the specified Data Lake Analytics account




### Get the ACL entry of group owner for a database

This command gets the ACL entry of the group owner for the database of the specified Data Lake Analytics account




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
- Name: GetCatalogAclEntry
  Position: 0
  IsRequired: true
  ValueByPipeline: false
  ValueByPipelineByPropertyName: true
  ValueFromRemainingArguments: false
- Name: GetCatalogAclEntryForUserOwner
  Position: 0
  IsRequired: true
  ValueByPipeline: false
  ValueByPipelineByPropertyName: true
  ValueFromRemainingArguments: false
- Name: GetCatalogAclEntryForGroupOwner
  Position: 0
  IsRequired: true
  ValueByPipeline: false
  ValueByPipelineByPropertyName: true
  ValueFromRemainingArguments: false
- Name: GetCatalogItemAclEntry
  Position: 0
  IsRequired: true
  ValueByPipeline: false
  ValueByPipelineByPropertyName: true
  ValueFromRemainingArguments: false
- Name: GetCatalogItemAclEntryForUserOwner
  Position: 0
  IsRequired: true
  ValueByPipeline: false
  ValueByPipelineByPropertyName: true
  ValueFromRemainingArguments: false
- Name: GetCatalogItemAclEntryForGroupOwner
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

The credentials, account, tenant, and subscription used for communication with Azure.

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

### -GroupOwner

Get ACL entry of catalog for group owner

```yaml
Type: System.Management.Automation.SwitchParameter
DefaultValue: False
VariableLength: true
Globbing: false
ParameterValue: []
Aliases: []
ParameterSets:
- Name: GetCatalogAclEntryForGroupOwner
  Position: Named
  IsRequired: true
  ValueByPipeline: false
  ValueByPipelineByPropertyName: false
  ValueFromRemainingArguments: false
- Name: GetCatalogItemAclEntryForGroupOwner
  Position: Named
  IsRequired: true
  ValueByPipeline: false
  ValueByPipelineByPropertyName: false
  ValueFromRemainingArguments: false
DontShow: false
AcceptedValues: []
HelpMessage: ''
```

### -ItemType

Specifies the type of the catalog or catalog item(s).
The acceptable values for this parameter are: - Catalog

- Database

```yaml
Type: System.String
DefaultValue: None
VariableLength: true
Globbing: false
ParameterValue: []
Aliases: []
ParameterSets:
- Name: GetCatalogItemAclEntry
  Position: Named
  IsRequired: true
  ValueByPipeline: false
  ValueByPipelineByPropertyName: true
  ValueFromRemainingArguments: false
- Name: GetCatalogItemAclEntryForUserOwner
  Position: Named
  IsRequired: true
  ValueByPipeline: false
  ValueByPipelineByPropertyName: true
  ValueFromRemainingArguments: false
- Name: GetCatalogItemAclEntryForGroupOwner
  Position: Named
  IsRequired: true
  ValueByPipeline: false
  ValueByPipelineByPropertyName: true
  ValueFromRemainingArguments: false
DontShow: false
AcceptedValues: []
HelpMessage: ''
```

### -Path

Specifies the Data Lake Analytics path of an catalog or catalog item.
The parts of the path should be separated by a period (.).

```yaml
Type: Microsoft.Azure.Commands.DataLakeAnalytics.Models.CatalogPathInstance
DefaultValue: None
VariableLength: true
Globbing: false
ParameterValue: []
Aliases: []
ParameterSets:
- Name: GetCatalogItemAclEntry
  Position: Named
  IsRequired: true
  ValueByPipeline: false
  ValueByPipelineByPropertyName: true
  ValueFromRemainingArguments: false
- Name: GetCatalogItemAclEntryForUserOwner
  Position: Named
  IsRequired: true
  ValueByPipeline: false
  ValueByPipelineByPropertyName: true
  ValueFromRemainingArguments: false
- Name: GetCatalogItemAclEntryForGroupOwner
  Position: Named
  IsRequired: true
  ValueByPipeline: false
  ValueByPipelineByPropertyName: true
  ValueFromRemainingArguments: false
DontShow: false
AcceptedValues: []
HelpMessage: ''
```

### -UserOwner

Get ACL entry of catalog for user owner.

```yaml
Type: System.Management.Automation.SwitchParameter
DefaultValue: False
VariableLength: true
Globbing: false
ParameterValue: []
Aliases: []
ParameterSets:
- Name: GetCatalogAclEntryForUserOwner
  Position: Named
  IsRequired: true
  ValueByPipeline: false
  ValueByPipelineByPropertyName: false
  ValueFromRemainingArguments: false
- Name: GetCatalogItemAclEntryForUserOwner
  Position: Named
  IsRequired: true
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

### Microsoft.Azure.Commands.DataLakeAnalytics.Models.PSDataLakeAnalyticsAcl

## NOTES




## RELATED LINKS

[Online Version:](https://learn.microsoft.com/powershell/module/az.datalakeanalytics/get-azdatalakeanalyticscatalogitemaclentry)

[U-SQL now offers database level access control](https://github.com/Azure/AzureDataLake/blob/master/docs/Release_Notes/2016/2016_08_01/USQL_Release_Notes_2016_08_01.md#u-sql-now-offers-database-level-access-control)

[Remove-AzDataLakeAnalyticsCatalogItemAclEntry]()

[Set-AzDataLakeAnalyticsCatalogItemAclEntry]()

