---
content type: cmdlet
title: Remove-AzDataLakeAnalyticsCatalogItemAclEntry
Module Name: Az.DataLakeAnalytics
Locale: {{ fill in locale }}
PlatyPS schema version: 2024-05-01
HelpUri: https://learn.microsoft.com/powershell/module/az.datalakeanalytics/remove-azdatalakeanalyticscatalogitemaclentry
ms.date: 05/24/2024
external help file: Microsoft.Azure.PowerShell.Cmdlets.DataLakeAnalytics.dll-Help.xml
---

# Remove-AzDataLakeAnalyticsCatalogItemAclEntry

## SYNOPSIS

Deletes an entry from the ACL of a catalog or catalog item in Data Lake Analytics.

## SYNTAX

### RemoveCatalogAclEntryForUser (Default)

```
Remove-AzDataLakeAnalyticsCatalogItemAclEntry [-Account] <System.String> -User]
 -ObjectId <System.Guid> [-PassThru]
 [-DefaultProfile <Microsoft.Azure.Commands.Common.Authentication.Abstractions.Core.IAzureContextContainer>]
 [-WhatIf] [-Confirm] [<CommonParameters>]
```

### RemoveCatalogItemAclEntryForUser

```
Remove-AzDataLakeAnalyticsCatalogItemAclEntry [-Account] <System.String> -User]
 -ObjectId <System.Guid> -ItemType <System.String>
 -Path <Microsoft.Azure.Commands.DataLakeAnalytics.Models.CatalogPathInstance> [-PassThru]
 [-DefaultProfile <Microsoft.Azure.Commands.Common.Authentication.Abstractions.Core.IAzureContextContainer>]
 [-WhatIf] [-Confirm] [<CommonParameters>]
```

### RemoveCatalogAclEntryForGroup

```
Remove-AzDataLakeAnalyticsCatalogItemAclEntry [-Account] <System.String> -Group]
 -ObjectId <System.Guid> [-PassThru]
 [-DefaultProfile <Microsoft.Azure.Commands.Common.Authentication.Abstractions.Core.IAzureContextContainer>]
 [-WhatIf] [-Confirm] [<CommonParameters>]
```

### RemoveCatalogItemAclEntryForGroup

```
Remove-AzDataLakeAnalyticsCatalogItemAclEntry [-Account] <System.String> -Group]
 -ObjectId <System.Guid> -ItemType <System.String>
 -Path <Microsoft.Azure.Commands.DataLakeAnalytics.Models.CatalogPathInstance> [-PassThru]
 [-DefaultProfile <Microsoft.Azure.Commands.Common.Authentication.Abstractions.Core.IAzureContextContainer>]
 [-WhatIf] [-Confirm] [<CommonParameters>]
```

## ALIASES

This cmdlet has the following aliases,
  {{Insert list of aliases}}

## DESCRIPTION

The Remove-AzDataLakeAnalyticsCatalogItemAclEntry cmdlet removes an entry (ACE) from the access control list (ACL) of a catalog or catalog item in Data Lake Analytics.


## EXAMPLES

### Remove the user ACL for a catalog

This command removes the catalog ACL for Patti Fuller of the contosoadla account.




### Remove the user ACL for a database

This command removes the database ACL for Patti Fuller of the contosoadla account.




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

### -Group

Remove ACL entry of catalog for group.

```yaml
Type: System.Management.Automation.SwitchParameter
DefaultValue: False
VariableLength: true
Globbing: false
ParameterValue: []
Aliases: []
ParameterSets:
- Name: RemoveCatalogAclEntryForGroup
  Position: Named
  IsRequired: true
  ValueByPipeline: false
  ValueByPipelineByPropertyName: false
  ValueFromRemainingArguments: false
- Name: RemoveCatalogItemAclEntryForGroup
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
- Name: RemoveCatalogItemAclEntryForUser
  Position: Named
  IsRequired: true
  ValueByPipeline: false
  ValueByPipelineByPropertyName: true
  ValueFromRemainingArguments: false
- Name: RemoveCatalogItemAclEntryForGroup
  Position: Named
  IsRequired: true
  ValueByPipeline: false
  ValueByPipelineByPropertyName: true
  ValueFromRemainingArguments: false
DontShow: false
AcceptedValues: []
HelpMessage: ''
```

### -ObjectId

The identity of the user to remove.

```yaml
Type: System.Guid
DefaultValue: None
VariableLength: true
Globbing: false
ParameterValue: []
Aliases:
- Id
- UserId
ParameterSets:
- Name: RemoveCatalogAclEntryForUser
  Position: Named
  IsRequired: true
  ValueByPipeline: false
  ValueByPipelineByPropertyName: true
  ValueFromRemainingArguments: false
- Name: RemoveCatalogAclEntryForGroup
  Position: Named
  IsRequired: true
  ValueByPipeline: false
  ValueByPipelineByPropertyName: true
  ValueFromRemainingArguments: false
- Name: RemoveCatalogItemAclEntryForUser
  Position: Named
  IsRequired: true
  ValueByPipeline: false
  ValueByPipelineByPropertyName: true
  ValueFromRemainingArguments: false
- Name: RemoveCatalogItemAclEntryForGroup
  Position: Named
  IsRequired: true
  ValueByPipeline: false
  ValueByPipelineByPropertyName: true
  ValueFromRemainingArguments: false
DontShow: false
AcceptedValues: []
HelpMessage: ''
```

### -PassThru

Indicates a boolean response should be returned indicating the result of the delete operation.

```yaml
Type: System.Management.Automation.SwitchParameter
DefaultValue: False
VariableLength: true
Globbing: false
ParameterValue: []
Aliases: []
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
- Name: RemoveCatalogItemAclEntryForUser
  Position: Named
  IsRequired: true
  ValueByPipeline: false
  ValueByPipelineByPropertyName: true
  ValueFromRemainingArguments: false
- Name: RemoveCatalogItemAclEntryForGroup
  Position: Named
  IsRequired: true
  ValueByPipeline: false
  ValueByPipelineByPropertyName: true
  ValueFromRemainingArguments: false
DontShow: false
AcceptedValues: []
HelpMessage: ''
```

### -User

Remove ACL entry of catalog for user.

```yaml
Type: System.Management.Automation.SwitchParameter
DefaultValue: False
VariableLength: true
Globbing: false
ParameterValue: []
Aliases: []
ParameterSets:
- Name: RemoveCatalogAclEntryForUser
  Position: Named
  IsRequired: true
  ValueByPipeline: false
  ValueByPipelineByPropertyName: false
  ValueFromRemainingArguments: false
- Name: RemoveCatalogItemAclEntryForUser
  Position: Named
  IsRequired: true
  ValueByPipeline: false
  ValueByPipelineByPropertyName: false
  ValueFromRemainingArguments: false
DontShow: false
AcceptedValues: []
HelpMessage: ''
```

### -WhatIf

Shows what would happen if the cmdlet runs.
The cmdlet is not run.

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

### System.Boolean

## NOTES




## RELATED LINKS

[Online Version:](https://learn.microsoft.com/powershell/module/az.datalakeanalytics/remove-azdatalakeanalyticscatalogitemaclentry)

[U-SQL now offers database level access control](https://github.com/Azure/AzureDataLake/blob/master/docs/Release_Notes/2016/2016_08_01/USQL_Release_Notes_2016_08_01.md#u-sql-now-offers-database-level-access-control)

[Get-AzDataLakeAnalyticsCatalogItemAclEntry]()

[Set-AzDataLakeAnalyticsCatalogItemAclEntry]()

