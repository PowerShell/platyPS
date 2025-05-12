---
external help file: MPIO_Cmdlets.xml
Module Name: MPIO
online version: https://learn.microsoft.com/powershell/module/mpio/get-mpiosetting?view=windowsserver2012-ps&wt.mc_id=ps-gethelp
schema: 2.0.0
title: Get-MPIOSetting
---
# Get-MPIOSetting

## SYNOPSIS
Gets MPIO settings.

## SYNTAX

```
Get-MPIOSetting
```

## DESCRIPTION
The **Get-MPIOSetting** cmdlet gets Microsoft Multipath I/O (MPIO) settings.
The settings are as follows:

- PathVerificationState
- PathVerificationPeriod
- PDORemovePeriod
- RetryCount
- RetryInterval
- UseCustomPathRecoveryTime
- CustomPathRecoveryTime
- DiskTimeoutValue

You can use the **Set-MPIOSetting** cmdlet to change these values.

## EXAMPLES

### Example 1: Get MPIO settings
```
PS C:\>Get-MPIOSetting

PathVerificationState     : Disabled
PathVerificationPeriod    : 30
PDORemovePeriod           : 20
RetryCount                : 3
RetryInterval             : 1
UseCustomPathRecoveryTime : Disabled
CustomPathRecoveryTime    : 40
DiskTimeoutValue          : 120
```

This command gets the MPIO settings.

## PARAMETERS

## INPUTS

## OUTPUTS

## NOTES

## RELATED LINKS

[Set-MPIOSetting](./Set-MPIOSetting.md)
