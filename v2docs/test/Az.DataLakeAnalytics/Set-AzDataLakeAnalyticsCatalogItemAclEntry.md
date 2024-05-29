---
content type: cmdlet
title: Set-AzDataLakeAnalyticsCatalogItemAclEntry
Module Name: Az.DataLakeAnalytics
Locale: {{ fill in locale }}
PlatyPS schema version: 2024-05-01
HelpUri: https://learn.microsoft.com/powershell/module/az.datalakeanalytics/set-azdatalakeanalyticscatalogitemaclentry
ms.date: 05/24/2024
external help file: Microsoft.Azure.PowerShell.Cmdlets.DataLakeAnalytics.dll-Help.xml
---

# Set-AzDataLakeAnalyticsCatalogItemAclEntry

## SYNOPSIS

Modifies an entry in the ACL of a catalog or catalog item in Data Lake Analytics.

## SYNTAX

### SetCatalogAclEntryForUser (Default)

```
Set-AzDataLakeAnalyticsCatalogItemAclEntry [-Account] <System.String> -User] -ObjectId <System.Guid>
 -Permissions <Microsoft.Azure.Commands.DataLakeAnalytics.Models.DataLakeAnalyticsEnums+PermissionType>
 [-DefaultProfile <Microsoft.Azure.Commands.Common.Authentication.Abstractions.Core.IAzureContextContainer>]
 [-WhatIf] [-Confirm] [<CommonParameters>]
```

### SetCatalogItemAclEntryForUser

```
Set-AzDataLakeAnalyticsCatalogItemAclEntry [-Account] <System.String> -User] -ObjectId <System.Guid>
 -ItemType <System.String>
 -Path <Microsoft.Azure.Commands.DataLakeAnalytics.Models.CatalogPathInstance>
 -Permissions <Microsoft.Azure.Commands.DataLakeAnalytics.Models.DataLakeAnalyticsEnums+PermissionType>
 [-DefaultProfile <Microsoft.Azure.Commands.Common.Authentication.Abstractions.Core.IAzureContextContainer>]
 [-WhatIf] [-Confirm] [<CommonParameters>]
```

### SetCatalogAclEntryForGroup

```
Set-AzDataLakeAnalyticsCatalogItemAclEntry [-Account] <System.String> -Group]
 -ObjectId <System.Guid>
 -Permissions <Microsoft.Azure.Commands.DataLakeAnalytics.Models.DataLakeAnalyticsEnums+PermissionType>
 [-DefaultProfile <Microsoft.Azure.Commands.Common.Authentication.Abstractions.Core.IAzureContextContainer>]
 [-WhatIf] [-Confirm] [<CommonParameters>]
```

### SetCatalogItemAclEntryForGroup

```
Set-AzDataLakeAnalyticsCatalogItemAclEntry [-Account] <System.String> -Group]
 -ObjectId <System.Guid> -ItemType <System.String>
 -Path <Microsoft.Azure.Commands.DataLakeAnalytics.Models.CatalogPathInstance>
 -Permissions <Microsoft.Azure.Commands.DataLakeAnalytics.Models.DataLakeAnalyticsEnums+PermissionType>
 [-DefaultProfile <Microsoft.Azure.Commands.Common.Authentication.Abstractions.Core.IAzureContextContainer>]
 [-WhatIf] [-Confirm] [<CommonParameters>]
```

### SetCatalogAclEntryForOther

```
Set-AzDataLakeAnalyticsCatalogItemAclEntry [-Account] <System.String> -Other]
 -Permissions <Microsoft.Azure.Commands.DataLakeAnalytics.Models.DataLakeAnalyticsEnums+PermissionType>
 [-DefaultProfile <Microsoft.Azure.Commands.Common.Authentication.Abstractions.Core.IAzureContextContainer>]
 [-WhatIf] [-Confirm] [<CommonParameters>]
```

### SetCatalogItemAclEntryForOther

```
Set-AzDataLakeAnalyticsCatalogItemAclEntry [-Account] <System.String> -Other]
 -ItemType <System.String>
 -Path <Microsoft.Azure.Commands.DataLakeAnalytics.Models.CatalogPathInstance>
 -Permissions <Microsoft.Azure.Commands.DataLakeAnalytics.Models.DataLakeAnalyticsEnums+PermissionType>
 [-DefaultProfile <Microsoft.Azure.Commands.Common.Authentication.Abstractions.Core.IAzureContextContainer>]
 [-WhatIf] [-Confirm] [<CommonParameters>]
```

### SetCatalogAclEntryForUserOwner

```
Set-AzDataLakeAnalyticsCatalogItemAclEntry [-Account] <System.String> -UserOwner]
 -Permissions <Microsoft.Azure.Commands.DataLakeAnalytics.Models.DataLakeAnalyticsEnums+PermissionType>
 [-DefaultProfile <Microsoft.Azure.Commands.Common.Authentication.Abstractions.Core.IAzureContextContainer>]
 [-WhatIf] [-Confirm] [<CommonParameters>]
```

### SetCatalogItemAclEntryForUserOwner

```
Set-AzDataLakeAnalyticsCatalogItemAclEntry [-Account] <System.String> -UserOwner]
 -ItemType <System.String>
 -Path <Microsoft.Azure.Commands.DataLakeAnalytics.Models.CatalogPathInstance>
 -Permissions <Microsoft.Azure.Commands.DataLakeAnalytics.Models.DataLakeAnalyticsEnums+PermissionType>
 [-DefaultProfile <Microsoft.Azure.Commands.Common.Authentication.Abstractions.Core.IAzureContextContainer>]
 [-WhatIf] [-Confirm] [<CommonParameters>]
```

### SetCatalogAclEntryForGroupOwner

```
Set-AzDataLakeAnalyticsCatalogItemAclEntry [-Account] <System.String> -GroupOwner]
 -Permissions <Microsoft.Azure.Commands.DataLakeAnalytics.Models.DataLakeAnalyticsEnums+PermissionType>
 [-DefaultProfile <Microsoft.Azure.Commands.Common.Authentication.Abstractions.Core.IAzureContextContainer>]
 [-WhatIf] [-Confirm] [<CommonParameters>]
```

### SetCatalogItemAclEntryForGroupOwner

```
Set-AzDataLakeAnalyticsCatalogItemAclEntry [-Account] <System.String> -GroupOwner]
 -ItemType <System.String>
 -Path <Microsoft.Azure.Commands.DataLakeAnalytics.Models.CatalogPathInstance>
 -Permissions <Microsoft.Azure.Commands.DataLakeAnalytics.Models.DataLakeAnalyticsEnums+PermissionType>
 [-DefaultProfile <Microsoft.Azure.Commands.Common.Authentication.Abstractions.Core.IAzureContextContainer>]
 [-WhatIf] [-Confirm] [<CommonParameters>]
```

## ALIASES

This cmdlet has the following aliases,
  {{Insert list of aliases}}

## DESCRIPTION

The Set-AzDataLakeAnalyticsCatalogItemAclEntry cmdlet adds or modifies an entry (ACE) in the access control list (ACL) of a catalog or catalog item in Data Lake Analytics.


## EXAMPLES

### Modify user permissions for a catalog

This command modifies the catalog ACE for Patti Fuller to have read permissions.




### Modify user Permissions for a database

This command modifies the database ACE for Patti Fuller to have read permissions.




### Modify other permissions for a catalog

This command modifies the catalog ACE for other to have read permissions.




### Modify other Permissions for a database






### Modify user owner permissions for a catalog

This command sets the owner permission for the account to Read.




### Modify user owner Permissions for a database

This command sets the owner permission for the database to Read.




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

Set ACL entry of catalog for group.

```yaml
Type: System.Management.Automation.SwitchParameter
DefaultValue: False
VariableLength: true
Globbing: false
ParameterValue: []
Aliases: []
ParameterSets:
- Name: SetCatalogAclEntryForGroup
  Position: Named
  IsRequired: true
  ValueByPipeline: false
  ValueByPipelineByPropertyName: false
  ValueFromRemainingArguments: false
- Name: SetCatalogItemAclEntryForGroup
  Position: Named
  IsRequired: true
  ValueByPipeline: false
  ValueByPipelineByPropertyName: false
  ValueFromRemainingArguments: false
DontShow: false
AcceptedValues: []
HelpMessage: ''
```

### -GroupOwner

Set ACL entry of catalog for group owner.

```yaml
Type: System.Management.Automation.SwitchParameter
DefaultValue: False
VariableLength: true
Globbing: false
ParameterValue: []
Aliases: []
ParameterSets:
- Name: SetCatalogAclEntryForGroupOwner
  Position: Named
  IsRequired: true
  ValueByPipeline: false
  ValueByPipelineByPropertyName: false
  ValueFromRemainingArguments: false
- Name: SetCatalogItemAclEntryForGroupOwner
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
- Name: SetCatalogItemAclEntryForUser
  Position: Named
  IsRequired: true
  ValueByPipeline: false
  ValueByPipelineByPropertyName: true
  ValueFromRemainingArguments: false
- Name: SetCatalogItemAclEntryForGroup
  Position: Named
  IsRequired: true
  ValueByPipeline: false
  ValueByPipelineByPropertyName: true
  ValueFromRemainingArguments: false
- Name: SetCatalogItemAclEntryForOther
  Position: Named
  IsRequired: true
  ValueByPipeline: false
  ValueByPipelineByPropertyName: true
  ValueFromRemainingArguments: false
- Name: SetCatalogItemAclEntryForUserOwner
  Position: Named
  IsRequired: true
  ValueByPipeline: false
  ValueByPipelineByPropertyName: true
  ValueFromRemainingArguments: false
- Name: SetCatalogItemAclEntryForGroupOwner
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

The identity of the user to set.

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
- Name: SetCatalogAclEntryForUser
  Position: Named
  IsRequired: true
  ValueByPipeline: false
  ValueByPipelineByPropertyName: true
  ValueFromRemainingArguments: false
- Name: SetCatalogAclEntryForGroup
  Position: Named
  IsRequired: true
  ValueByPipeline: false
  ValueByPipelineByPropertyName: true
  ValueFromRemainingArguments: false
- Name: SetCatalogItemAclEntryForUser
  Position: Named
  IsRequired: true
  ValueByPipeline: false
  ValueByPipelineByPropertyName: true
  ValueFromRemainingArguments: false
- Name: SetCatalogItemAclEntryForGroup
  Position: Named
  IsRequired: true
  ValueByPipeline: false
  ValueByPipelineByPropertyName: true
  ValueFromRemainingArguments: false
DontShow: false
AcceptedValues: []
HelpMessage: ''
```

### -Other

Set ACL entry of catalog for other.

```yaml
Type: System.Management.Automation.SwitchParameter
DefaultValue: False
VariableLength: true
Globbing: false
ParameterValue: []
Aliases: []
ParameterSets:
- Name: SetCatalogAclEntryForOther
  Position: Named
  IsRequired: true
  ValueByPipeline: false
  ValueByPipelineByPropertyName: false
  ValueFromRemainingArguments: false
- Name: SetCatalogItemAclEntryForOther
  Position: Named
  IsRequired: true
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
- Name: SetCatalogItemAclEntryForUser
  Position: Named
  IsRequired: true
  ValueByPipeline: false
  ValueByPipelineByPropertyName: true
  ValueFromRemainingArguments: false
- Name: SetCatalogItemAclEntryForGroup
  Position: Named
  IsRequired: true
  ValueByPipeline: false
  ValueByPipelineByPropertyName: true
  ValueFromRemainingArguments: false
- Name: SetCatalogItemAclEntryForOther
  Position: Named
  IsRequired: true
  ValueByPipeline: false
  ValueByPipelineByPropertyName: true
  ValueFromRemainingArguments: false
- Name: SetCatalogItemAclEntryForUserOwner
  Position: Named
  IsRequired: true
  ValueByPipeline: false
  ValueByPipelineByPropertyName: true
  ValueFromRemainingArguments: false
- Name: SetCatalogItemAclEntryForGroupOwner
  Position: Named
  IsRequired: true
  ValueByPipeline: false
  ValueByPipelineByPropertyName: true
  ValueFromRemainingArguments: false
DontShow: false
AcceptedValues: []
HelpMessage: ''
```

### -Permissions

Specifies the permissions for the ACE.
The acceptable values for this parameter are: - None

- Read

- ReadWrite

```yaml
Type: Microsoft.Azure.Commands.DataLakeAnalytics.Models.DataLakeAnalyticsEnums+PermissionType
DefaultValue: None
VariableLength: true
Globbing: false
ParameterValue: []
Aliases: []
ParameterSets:
- Name: SetCatalogAclEntryForUser
  Position: Named
  IsRequired: true
  ValueByPipeline: false
  ValueByPipelineByPropertyName: true
  ValueFromRemainingArguments: false
- Name: SetCatalogAclEntryForGroup
  Position: Named
  IsRequired: true
  ValueByPipeline: false
  ValueByPipelineByPropertyName: true
  ValueFromRemainingArguments: false
- Name: SetCatalogAclEntryForOther
  Position: Named
  IsRequired: true
  ValueByPipeline: false
  ValueByPipelineByPropertyName: true
  ValueFromRemainingArguments: false
- Name: SetCatalogAclEntryForUserOwner
  Position: Named
  IsRequired: true
  ValueByPipeline: false
  ValueByPipelineByPropertyName: true
  ValueFromRemainingArguments: false
- Name: SetCatalogAclEntryForGroupOwner
  Position: Named
  IsRequired: true
  ValueByPipeline: false
  ValueByPipelineByPropertyName: true
  ValueFromRemainingArguments: false
- Name: SetCatalogItemAclEntryForUser
  Position: Named
  IsRequired: true
  ValueByPipeline: false
  ValueByPipelineByPropertyName: true
  ValueFromRemainingArguments: false
- Name: SetCatalogItemAclEntryForGroup
  Position: Named
  IsRequired: true
  ValueByPipeline: false
  ValueByPipelineByPropertyName: true
  ValueFromRemainingArguments: false
- Name: SetCatalogItemAclEntryForOther
  Position: Named
  IsRequired: true
  ValueByPipeline: false
  ValueByPipelineByPropertyName: true
  ValueFromRemainingArguments: false
- Name: SetCatalogItemAclEntryForUserOwner
  Position: Named
  IsRequired: true
  ValueByPipeline: false
  ValueByPipelineByPropertyName: true
  ValueFromRemainingArguments: false
- Name: SetCatalogItemAclEntryForGroupOwner
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

Set ACL entry of catalog for user.

```yaml
Type: System.Management.Automation.SwitchParameter
DefaultValue: False
VariableLength: true
Globbing: false
ParameterValue: []
Aliases: []
ParameterSets:
- Name: SetCatalogAclEntryForUser
  Position: Named
  IsRequired: true
  ValueByPipeline: false
  ValueByPipelineByPropertyName: false
  ValueFromRemainingArguments: false
- Name: SetCatalogItemAclEntryForUser
  Position: Named
  IsRequired: true
  ValueByPipeline: false
  ValueByPipelineByPropertyName: false
  ValueFromRemainingArguments: false
DontShow: false
AcceptedValues: []
HelpMessage: ''
```

### -UserOwner

Set ACL entry of catalog for user owner.

```yaml
Type: System.Management.Automation.SwitchParameter
DefaultValue: False
VariableLength: true
Globbing: false
ParameterValue: []
Aliases: []
ParameterSets:
- Name: SetCatalogAclEntryForUserOwner
  Position: Named
  IsRequired: true
  ValueByPipeline: false
  ValueByPipelineByPropertyName: false
  ValueFromRemainingArguments: false
- Name: SetCatalogItemAclEntryForUserOwner
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

### Microsoft.Azure.Commands.DataLakeAnalytics.Models.PSDataLakeAnalyticsAcl

## NOTES




## RELATED LINKS

[Online Version:](https://learn.microsoft.com/powershell/module/az.datalakeanalytics/set-azdatalakeanalyticscatalogitemaclentry)

[Get-AzDataLakeAnalyticsCatalogItemAclEntry]()

[Remove-AzDataLakeAnalyticsCatalogItemAclEntry]()

[U-SQL now offers database level access control](https://github.com/Azure/AzureDataLake/blob/master/docs/Release_Notes/2016/2016_08_01/USQL_Release_Notes_2016_08_01.md#u-sql-now-offers-database-level-access-control)

