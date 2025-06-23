---
external help file: Microsoft.SqlServer.Management.PSSnapins.dll-Help.xml
Locale: en-US
Module Name: SqlServer
ms.date: 05/30/2025
online version: https://learn.microsoft.com/powershell/module/sqlserver/remove-sqlsensitivityclassification
schema: 2.0.0
title: Remove-SqlSensitivityClassification
---

# Remove-SqlSensitivityClassification

## SYNOPSIS
Remove the sensitivity label and/or information type of columns in the database.

## SYNTAX

### ByContext (Default)

```
Remove-SqlSensitivityClassification -ColumnName <String[]> [-SuppressProviderContextWarning]
 [<CommonParameters>]
```

### ByConnectionString

```
Remove-SqlSensitivityClassification -ColumnName <String[]> -ConnectionString <String>
  [<CommonParameters>]
```

### ByConnectionParameters

```
Remove-SqlSensitivityClassification -ColumnName <String[]> -ServerInstance <PSObject>
 -DatabaseName <String> [-Credential <PSCredential>] [<CommonParameters>]
```

### ByPath

```
Remove-SqlSensitivityClassification -ColumnName <String[]> -Path <String>
 [<CommonParameters>]
```

### ByDBObject

```
Remove-SqlSensitivityClassification -ColumnName <String[]> -InputObject <Database>
  [<CommonParameters>]
```

## DESCRIPTION

The Remove-SqlSensitivityClassification cmdlet removes the sensitivity label and information type of
columns in the database.

The sensitivity labels and information types of columns can be set using
[SQL Server Management Studio (SSMS)](/sql/ssms/sql-server-management-studio-ssms) release 17.5 and
above, or with the Set-SqlSensitivityClassification cmdlet.

The sensitivity labels and information types of columns can be viewed using
[SQL Server Management Studio (SSMS)](/sql/ssms/sql-server-management-studio-ssms) release 17.5 and
above, the
[Extended Properties catalog view](/sql/relational-databases/security/sql-data-discovery-and-classification?view=sql-server-2017#subheading-3),
or the **Get-SqlSensitivityClassification** cmdlet.

> `Module requirements: version 21+ on PowerShell 5.1; version 22+ on PowerShell 7.x.`

## EXAMPLES

### Example 1: Remove sensitivity label and information type from a column using Windows authentication

```powershell
PS C:\> Remove-SqlSensitivityClassification -ServerInstance "MyComputer\MainInstance" -Database "myDatabase" -ColumnName "Sales.Customers.email"
```

Remove the sensitivity label and information type of column `Sales.Customers.email` in `myDatabase`.

### Example 2: Remove sensitivity label and information type from a column by providing a database path

```powershell
PS C:\> Remove-SqlSensitivityClassification -Path "SQLSERVER:\SQL\MyComputer\MainInstance\Databases\MyDatabase" -ColumnName "Sales.Customers.email"
```

Remove the sensitivity label and information type of column `Sales.Customers.email` in `MyDatabase`.

### Example 3: Remove sensitivity labels and information types on multiple columns using current path context

```powershell
PS C:\> $columns = @("Sales.Customers.ip_address" , "Sales.Customers.email")
PS C:\> Set-Location "SQLSERVER:\SQL\MyComputer\MainInstance\Databases\MyDatabase"
PS SQLSERVER:\SQL\MyComputer\MainInstance> Remove-SqlSensitivityClassification -ColumnName $columns
 WARNING: Using provider context. Server = MyComputer, Database = MyDatabase.
```

Remove the sensitivity labels and information types of columns `Sales.Customers.ip_address` and
`Sales.Customers.email` in `MyDatabase`.

## PARAMETERS

### -ColumnName

Name(s) of columns for which information type and sensitivity label is fetched.

```yaml
Type: String[]
Parameter Sets: (All)
Aliases: Column

Required: True
Position: Named
Default value: None
Accept pipeline input: True (ByPropertyName)
Accept wildcard characters: False
```

### -ConnectionString

Specifies a connection string to connect to the database. If this parameter is present, other
connection parameters will be ignored

```yaml
Type: String
Parameter Sets: ByConnectionString
Aliases:

Required: True
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -Credential

Specifies a credential used to connect to the database.

```yaml
Type: PSCredential
Parameter Sets: ByConnectionParameters
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -DatabaseName

Specifies the name of a database. This cmdlet connects to this database in the instance that is
specified in the ServerInstance parameter.

If the *DatabaseName* parameter is not specified, the database that is used depends on whether the
current path specifies both the SQLSERVER:\SQL folder and a database name. If the path specifies
both the SQL folder and a database name, this cmdlet connects to the database that is specified in
the path.

```yaml
Type: String
Parameter Sets: ByConnectionParameters
Aliases:

Required: True
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -InputObject

Specifies a SQL Server Management Object (SMO) that represent the database that this cmdlet uses.

```yaml
Type: Database
Parameter Sets: ByDBObject
Aliases:

Required: True
Position: Named
Default value: None
Accept pipeline input: True (ByValue)
Accept wildcard characters: False
```

### -Path

Specifies the path to the instance of SQL Server on which this cmdlet runs the operation. If you do
not specify a value for this parameter, the cmdlet uses the current working location.

```yaml
Type: String
Parameter Sets: ByPath
Aliases:

Required: True
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -ServerInstance

Specifies either the name of the server instance (a string) or SQL Server Management Objects (SMO)
object that specifies the name of an instance of the Database Engine. For default instances, only
specify the computer name: MyComputer. For named instances, use the format
ComputerName\InstanceName.

```yaml
Type: PSObject
Parameter Sets: ByConnectionParameters
Aliases:

Required: True
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -SuppressProviderContextWarning

Indicates that this cmdlet suppresses the warning that this cmdlet has used in the database context
from the current SQLSERVER:\SQL path setting to establish the database context for the cmdlet.

```yaml
Type: SwitchParameter
Parameter Sets: ByContext
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### CommonParameters

This cmdlet supports the common parameters: -Debug, -ErrorAction, -ErrorVariable,
-InformationAction, -InformationVariable, -OutVariable, -OutBuffer, -PipelineVariable, -Verbose,
-WarningAction, and -WarningVariable. For more information, see
[about_CommonParameters](/powershell/module/microsoft.powershell.core/about/about_commonparameters).

## INPUTS

### System.String[]

### Microsoft.SqlServer.Management.Smo.Database

## OUTPUTS

### System.Object

## NOTES

## RELATED LINKS

[What's new in SSMS 17.5: Data Discovery and Classification](https://cloudblogs.microsoft.com/sqlserver/2018/02/20/whats-new-in-ssms-17-5-data-discovery-and-classification/)

[SQL Data Discovery and Classification](/sql/relational-databases/security/sql-data-discovery-and-classification)
