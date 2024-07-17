---
Module Name: Exchange PowerShell
Module Guid: XXXXXXXX-XXXX-XXXX-XXXX-XXXXXXXXXXXX
title: exchange
---

# Exchange PowerShell
## Description
Exchange PowerShell is built on Windows PowerShell technology and provides a powerful command-line interface that enables automation of administrative tasks. The following PowerShell environments are available in Exchange:

- [Exchange Server PowerShell (Exchange Management Shell)](https://learn.microsoft.com/powershell/exchange/exchange-management-shell)
- [Exchange Online PowerShell](https://learn.microsoft.com/powershell/exchange/exchange-online-powershell)
- [Security & Compliance PowerShell](https://learn.microsoft.com/powershell/exchange/scc-powershell)
- [Exchange Online Protection PowerShell](https://learn.microsoft.com/powershell/exchange/exchange-online-protection-powershell)

> [!NOTE]
> For Exchange Online, Security & Compliance, and Exchange Online Protection, the module from the PowerShell Gallery that you use to connect is [ExchangeOnlineManagement](https://www.powershellgallery.com/packages/ExchangeOnlineManagement/). For more information, see [About the Exchange Online PowerShell module](../../docs-conceptual/exchange-online-powershell-v2.md).
>
> For Exchange Server, there is no Microsoft-provided module in the PowerShell Gallery. Instead, to use PowerShell in Exchange, you have the following options:
>
> - Use the Exchange Management Shell on an Exchange server or that you've installed locally on your own computer using a **Management tools** only installation of Exchange server. For more information, see [Install the Exchange Server Management Tools](/Exchange/plan-and-deploy/post-installation-tasks/install-management-tools) and [Open the Exchange Management Shell](../../docs-conceptual/open-the-exchange-management-shell.md).
> - Use remote PowerShell from a Windows PowerShell session. For more information, see [Connect to Exchange servers using remote PowerShell](../../docs-conceptual/connect-to-exchange-servers-using-remote-powershell.md).

## active-directory Cmdlets
### [Add-ADPermission](Add-ADPermission.md)

### [Dump-ProvisioningCache](Dump-ProvisioningCache.md)

### [Get-ADPermission](Get-ADPermission.md)

### [Get-ADServerSettings](Get-ADServerSettings.md)

### [Get-ADSite](Get-ADSite.md)

### [Get-AdSiteLink](Get-AdSiteLink.md)

### [Get-DomainController](Get-DomainController.md)

### [Get-OrganizationalUnit](Get-OrganizationalUnit.md)

### [Get-Trust](Get-Trust.md)

### [Get-UserPrincipalNamesSuffix](Get-UserPrincipalNamesSuffix.md)

### [Remove-ADPermission](Remove-ADPermission.md)

### [Reset-ProvisioningCache](Reset-ProvisioningCache.md)

### [Set-ADServerSettings](Set-ADServerSettings.md)

### [Set-ADSite](Set-ADSite.md)

### [Set-AdSiteLink](Set-AdSiteLink.md)

## defender-for-office-365 Cmdlets
### [Disable-AntiPhishRule](Disable-AntiPhishRule.md)

### [Disable-ATPProtectionPolicyRule](Disable-ATPProtectionPolicyRule.md)

### [Disable-SafeAttachmentRule](Disable-SafeAttachmentRule.md)

### [Disable-SafeLinksRule](Disable-SafeLinksRule.md)

### [Enable-AntiPhishRule](Enable-AntiPhishRule.md)

### [Enable-ATPProtectionPolicyRule](Enable-ATPProtectionPolicyRule.md)

### [Enable-SafeAttachmentRule](Enable-SafeAttachmentRule.md)

### [Enable-SafeLinksRule](Enable-SafeLinksRule.md)

### [Get-AntiPhishPolicy](Get-AntiPhishPolicy.md)

### [Get-AntiPhishRule](Get-AntiPhishRule.md)

### [Get-ATPBuiltInProtectionRule](Get-ATPBuiltInProtectionRule.md)

### [Get-AtpPolicyForO365](Get-AtpPolicyForO365.md)

### [Get-ATPProtectionPolicyRule](Get-ATPProtectionPolicyRule.md)

### [Get-ATPTotalTrafficReport](Get-ATPTotalTrafficReport.md)

### [Get-ContentMalwareMdoAggregateReport](Get-ContentMalwareMdoAggregateReport.md)

### [Get-ContentMalwareMdoDetailReport](Get-ContentMalwareMdoDetailReport.md)

### [Get-EmailTenantSettings](Get-EmailTenantSettings.md)

### [Get-MailDetailATPReport](Get-MailDetailATPReport.md)

### [Get-MailTrafficATPReport](Get-MailTrafficATPReport.md)

### [Get-SafeAttachmentPolicy](Get-SafeAttachmentPolicy.md)

### [Get-SafeAttachmentRule](Get-SafeAttachmentRule.md)

### [Get-SafeLinksAggregateReport](Get-SafeLinksAggregateReport.md)

### [Get-SafeLinksDetailReport](Get-SafeLinksDetailReport.md)

### [Get-SafeLinksPolicy](Get-SafeLinksPolicy.md)

### [Get-SafeLinksRule](Get-SafeLinksRule.md)

### [Get-SpoofIntelligenceInsight](Get-SpoofIntelligenceInsight.md)

### [Get-SpoofMailReport](Get-SpoofMailReport.md)

### [New-AntiPhishPolicy](New-AntiPhishPolicy.md)

### [New-AntiPhishRule](New-AntiPhishRule.md)

### [New-ATPBuiltInProtectionRule](New-ATPBuiltInProtectionRule.md)

### [New-ATPProtectionPolicyRule](New-ATPProtectionPolicyRule.md)

### [New-SafeAttachmentPolicy](New-SafeAttachmentPolicy.md)

### [New-SafeAttachmentRule](New-SafeAttachmentRule.md)

### [New-SafeLinksPolicy](New-SafeLinksPolicy.md)

### [New-SafeLinksRule](New-SafeLinksRule.md)

### [Remove-AntiPhishPolicy](Remove-AntiPhishPolicy.md)

### [Remove-AntiPhishRule](Remove-AntiPhishRule.md)

### [Remove-ATPProtectionPolicyRule](Remove-ATPProtectionPolicyRule.md)

### [Remove-SafeAttachmentPolicy](Remove-SafeAttachmentPolicy.md)

### [Remove-SafeAttachmentRule](Remove-SafeAttachmentRule.md)

### [Remove-SafeLinksPolicy](Remove-SafeLinksPolicy.md)

### [Remove-SafeLinksRule](Remove-SafeLinksRule.md)

### [Set-AntiPhishPolicy](Set-AntiPhishPolicy.md)

### [Set-AntiPhishRule](Set-AntiPhishRule.md)

### [Set-ATPBuiltInProtectionRule](Set-ATPBuiltInProtectionRule.md)

### [Set-AtpPolicyForO365](Set-AtpPolicyForO365.md)

### [Set-ATPProtectionPolicyRule](Set-ATPProtectionPolicyRule.md)

### [Set-EmailTenantSettings](Set-EmailTenantSettings.md)

### [Set-SafeAttachmentPolicy](Set-SafeAttachmentPolicy.md)

### [Set-SafeAttachmentRule](Set-SafeAttachmentRule.md)

### [Set-SafeLinksPolicy](Set-SafeLinksPolicy.md)

### [Set-SafeLinksRule](Set-SafeLinksRule.md)

## antispam-antimalware Cmdlets
### [Add-AttachmentFilterEntry](Add-AttachmentFilterEntry.md)

### [Add-ContentFilterPhrase](Add-ContentFilterPhrase.md)

### [Add-IPAllowListEntry](Add-IPAllowListEntry.md)

### [Add-IPAllowListProvider](Add-IPAllowListProvider.md)

### [Add-IPBlockListEntry](Add-IPBlockListEntry.md)

### [Add-IPBlockListProvider](Add-IPBlockListProvider.md)

### [Delete-QuarantineMessage](Delete-QuarantineMessage.md)

### [Disable-EOPProtectionPolicyRule](Disable-EOPProtectionPolicyRule.md)

### [Disable-HostedContentFilterRule](Disable-HostedContentFilterRule.md)

### [Disable-HostedOutboundSpamFilterRule](Disable-HostedOutboundSpamFilterRule.md)

### [Disable-MalwareFilterRule](Disable-MalwareFilterRule.md)

### [Disable-ReportSubmissionRule](Disable-ReportSubmissionRule.md)

### [Enable-AntispamUpdates](Enable-AntispamUpdates.md)

### [Enable-EOPProtectionPolicyRule](Enable-EOPProtectionPolicyRule.md)

### [Enable-HostedContentFilterRule](Enable-HostedContentFilterRule.md)

### [Enable-HostedOutboundSpamFilterRule](Enable-HostedOutboundSpamFilterRule.md)

### [Enable-MalwareFilterRule](Enable-MalwareFilterRule.md)

### [Export-QuarantineMessage](Export-QuarantineMessage.md)

### [Enable-ReportSubmissionRule](Enable-ReportSubmissionRule.md)

### [Get-AgentLog](Get-AgentLog.md)

### [Get-AggregateZapReport](Get-AggregateZapReport.md)

### [Get-ArcConfig](Get-ArcConfig.md)

### [Get-AttachmentFilterEntry](Get-AttachmentFilterEntry.md)

### [Get-AttachmentFilterListConfig](Get-AttachmentFilterListConfig.md)

### [Get-BlockedConnector](Get-BlockedConnector.md)

### [Get-BlockedSenderAddress](Get-BlockedSenderAddress.md)

### [Get-ConfigAnalyzerPolicyRecommendation](Get-ConfigAnalyzerPolicyRecommendation.md)

### [Get-ContentFilterConfig](Get-ContentFilterConfig.md)

### [Get-ContentFilterPhrase](Get-ContentFilterPhrase.md)

### [Get-DetailZapReport](Get-DetailZapReport.md)

### [Get-DkimSigningConfig](Get-DkimSigningConfig.md)

### [Get-EOPProtectionPolicyRule](Get-EOPProtectionPolicyRule.md)

### [Get-ExoPhishSimOverrideRule](Get-ExoPhishSimOverrideRule.md)

### [Get-ExoSecOpsOverrideRule](Get-ExoSecOpsOverrideRule.md)

### [Get-HostedConnectionFilterPolicy](Get-HostedConnectionFilterPolicy.md)

### [Get-HostedContentFilterPolicy](Get-HostedContentFilterPolicy.md)

### [Get-HostedContentFilterRule](Get-HostedContentFilterRule.md)

### [Get-HostedOutboundSpamFilterPolicy](Get-HostedOutboundSpamFilterPolicy.md)

### [Get-HostedOutboundSpamFilterRule](Get-HostedOutboundSpamFilterRule.md)

### [Get-IPAllowListConfig](Get-IPAllowListConfig.md)

### [Get-IPAllowListEntry](Get-IPAllowListEntry.md)

### [Get-IPAllowListProvider](Get-IPAllowListProvider.md)

### [Get-IPAllowListProvidersConfig](Get-IPAllowListProvidersConfig.md)

### [Get-IPBlockListConfig](Get-IPBlockListConfig.md)

### [Get-IPBlockListEntry](Get-IPBlockListEntry.md)

### [Get-IPBlockListProvider](Get-IPBlockListProvider.md)

### [Get-IPBlockListProvidersConfig](Get-IPBlockListProvidersConfig.md)

### [Get-MailboxJunkEmailConfiguration](Get-MailboxJunkEmailConfiguration.md)

### [Get-MalwareFilteringServer](Get-MalwareFilteringServer.md)

### [Get-MalwareFilterPolicy](Get-MalwareFilterPolicy.md)

### [Get-MalwareFilterRule](Get-MalwareFilterRule.md)

### [Get-PhishSimOverridePolicy](Get-PhishSimOverridePolicy.md)

### [Get-PhishSimOverrideRule](Get-PhishSimOverrideRule.md)

### [Get-QuarantineMessage](Get-QuarantineMessage.md)

### [Get-QuarantineMessageHeader](Get-QuarantineMessageHeader.md)

### [Get-QuarantinePolicy](Get-QuarantinePolicy.md)

### [Get-RecipientFilterConfig](Get-RecipientFilterConfig.md)

### [Get-ReportSubmissionPolicy](Get-ReportSubmissionPolicy.md)

### [Get-ReportSubmissionRule](Get-ReportSubmissionRule.md)

### [Get-SecOpsOverridePolicy](Get-SecOpsOverridePolicy.md)

### [Get-SecOpsOverrideRule](Get-SecOpsOverrideRule.md)

### [Get-SenderFilterConfig](Get-SenderFilterConfig.md)

### [Get-SenderIdConfig](Get-SenderIdConfig.md)

### [Get-SenderReputationConfig](Get-SenderReputationConfig.md)

### [Get-TeamsProtectionPolicy](Get-TeamsProtectionPolicy.md)

### [Get-TeamsProtectionPolicyRule](Get-TeamsProtectionPolicyRule.md)

### [Get-TenantAllowBlockListItems](Get-TenantAllowBlockListItems.md)

### [Get-TenantAllowBlockListSpoofItems](Get-TenantAllowBlockListSpoofItems.md)

### [New-DkimSigningConfig](New-DkimSigningConfig.md)

### [New-EOPProtectionPolicyRule](New-EOPProtectionPolicyRule.md)

### [New-ExoPhishSimOverrideRule](New-ExoPhishSimOverrideRule.md)

### [New-ExoSecOpsOverrideRule](New-ExoSecOpsOverrideRule.md)

### [New-HostedContentFilterPolicy](New-HostedContentFilterPolicy.md)

### [New-HostedContentFilterRule](New-HostedContentFilterRule.md)

### [New-HostedOutboundSpamFilterPolicy](New-HostedOutboundSpamFilterPolicy.md)

### [New-HostedOutboundSpamFilterRule](New-HostedOutboundSpamFilterRule.md)

### [New-MalwareFilterPolicy](New-MalwareFilterPolicy.md)

### [New-MalwareFilterRule](New-MalwareFilterRule.md)

### [New-PhishSimOverridePolicy](New-PhishSimOverridePolicy.md)

### [New-QuarantinePermissions](New-QuarantinePermissions.md)

### [New-QuarantinePolicy](New-QuarantinePolicy.md)

### [New-ReportSubmissionPolicy](New-ReportSubmissionPolicy.md)

### [New-ReportSubmissionRule](New-ReportSubmissionRule.md)

### [New-SecOpsOverridePolicy](New-SecOpsOverridePolicy.md)

### [New-TeamsProtectionPolicy](New-TeamsProtectionPolicy.md)

### [New-TeamsProtectionPolicyRule](New-TeamsProtectionPolicyRule.md)

### [New-TenantAllowBlockListItems](New-TenantAllowBlockListItems.md)

### [New-TenantAllowBlockListSpoofItems](New-TenantAllowBlockListSpoofItems.md)

### [Preview-QuarantineMessage](Preview-QuarantineMessage.md)

### [Release-QuarantineMessage](Release-QuarantineMessage.md)

### [Remove-AttachmentFilterEntry](Remove-AttachmentFilterEntry.md)

### [Remove-BlockedConnector](Remove-BlockedConnector.md)

### [Remove-BlockedSenderAddress](Remove-BlockedSenderAddress.md)

### [Remove-ContentFilterPhrase](Remove-ContentFilterPhrase.md)

### [Remove-EOPProtectionPolicyRule](Remove-EOPProtectionPolicyRule.md)

### [Remove-ExoPhishSimOverrideRule](Remove-ExoPhishSimOverrideRule.md)

### [Remove-ExoSecOpsOverrideRule](Remove-ExoSecOpsOverrideRule.md)

### [Remove-HostedContentFilterPolicy](Remove-HostedContentFilterPolicy.md)

### [Remove-HostedContentFilterRule](Remove-HostedContentFilterRule.md)

### [Remove-HostedOutboundSpamFilterPolicy](Remove-HostedOutboundSpamFilterPolicy.md)

### [Remove-HostedOutboundSpamFilterRule](Remove-HostedOutboundSpamFilterRule.md)

### [Remove-IPAllowListEntry](Remove-IPAllowListEntry.md)

### [Remove-IPAllowListProvider](Remove-IPAllowListProvider.md)

### [Remove-IPBlockListEntry](Remove-IPBlockListEntry.md)

### [Remove-IPBlockListProvider](Remove-IPBlockListProvider.md)

### [Remove-MalwareFilterPolicy](Remove-MalwareFilterPolicy.md)

### [Remove-MalwareFilterRule](Remove-MalwareFilterRule.md)

### [Remove-PhishSimOverridePolicy](Remove-PhishSimOverridePolicy.md)

### [Remove-QuarantinePolicy](Remove-QuarantinePolicy.md)

### [Remove-ReportSubmissionPolicy](Remove-ReportSubmissionPolicy.md)

### [Remove-ReportSubmissionRule](Remove-ReportSubmissionRule.md)

### [Remove-SecOpsOverridePolicy](Remove-SecOpsOverridePolicy.md)

### [Remove-TenantAllowBlockListItems](Remove-TenantAllowBlockListItems.md)

### [Remove-TenantAllowBlockListSpoofItems](Remove-TenantAllowBlockListSpoofItems.md)

### [Rotate-DkimSigningConfig](Rotate-DkimSigningConfig.md)

### [Set-ArcConfig](Set-ArcConfig.md)

### [Set-AttachmentFilterListConfig](Set-AttachmentFilterListConfig.md)

### [Set-ContentFilterConfig](Set-ContentFilterConfig.md)

### [Set-DkimSigningConfig](Set-DkimSigningConfig.md)

### [Set-EOPProtectionPolicyRule](Set-EOPProtectionPolicyRule.md)

### [Set-ExoPhishSimOverrideRule](Set-ExoPhishSimOverrideRule.md)

### [Set-ExoSecOpsOverrideRule](Remove-ExoSecOpsOverrideRule.md)

### [Set-HostedConnectionFilterPolicy](Set-HostedConnectionFilterPolicy.md)

### [Set-HostedContentFilterPolicy](Set-HostedContentFilterPolicy.md)

### [Set-HostedContentFilterRule](Set-HostedContentFilterRule.md)

### [Set-HostedOutboundSpamFilterPolicy](Set-HostedOutboundSpamFilterPolicy.md)

### [Set-HostedOutboundSpamFilterRule](Set-HostedOutboundSpamFilterRule.md)

### [Set-IPAllowListConfig](Set-IPAllowListConfig.md)

### [Set-IPAllowListProvider](Set-IPAllowListProvider.md)

### [Set-IPAllowListProvidersConfig](Set-IPAllowListProvidersConfig.md)

### [Set-IPBlockListConfig](Set-IPBlockListConfig.md)

### [Set-IPBlockListProvider](Set-IPBlockListProvider.md)

### [Set-IPBlockListProvidersConfig](Set-IPBlockListProvidersConfig.md)

### [Set-MailboxJunkEmailConfiguration](Set-MailboxJunkEmailConfiguration.md)

### [Set-MalwareFilteringServer](Set-MalwareFilteringServer.md)

### [Set-MalwareFilterPolicy](Set-MalwareFilterPolicy.md)

### [Set-MalwareFilterRule](Set-MalwareFilterRule.md)

### [Set-PhishSimOverridePolicy](Set-PhishSimOverridePolicy.md)

### [Set-QuarantinePermissions](Set-QuarantinePermissions.md)

### [Set-QuarantinePolicy](Set-QuarantinePolicy.md)

### [Set-RecipientFilterConfig](Set-RecipientFilterConfig.md)

### [Set-ReportSubmissionPolicy](Set-ReportSubmissionPolicy.md)

### [Set-ReportSubmissionRule](Set-ReportSubmissionRule.md)

### [Set-SecOpsOverridePolicy](Set-SecOpsOverridePolicy.md)

### [Set-SenderFilterConfig](Set-SenderFilterConfig.md)

### [Set-SenderIdConfig](Set-SenderIdConfig.md)

### [Set-SenderReputationConfig](Set-SenderReputationConfig.md)

### [Set-TeamsProtectionPolicy](Set-TeamsProtectionPolicy.md)

### [Set-TeamsProtectionPolicyRule](Set-TeamsProtectionPolicyRule.md)

### [Set-TenantAllowBlockListItems](Set-TenantAllowBlockListItems.md)

### [Set-TenantAllowBlockListSpoofItems](Set-TenantAllowBlockListSpoofItems.md)

### [Test-IPAllowListProvider](Test-IPAllowListProvider.md)

### [Test-IPBlockListProvider](Test-IPBlockListProvider.md)

### [Test-SenderId](Test-SenderId.md)

### [Update-SafeList](Update-SafeList.md)

## client-access Cmdlets
### [Clear-TextMessagingAccount](Clear-TextMessagingAccount.md)

### [Compare-TextMessagingVerificationCode](Compare-TextMessagingVerificationCode.md)

### [Disable-PushNotificationProxy](Disable-PushNotificationProxy.md)

### [Enable-PushNotificationProxy](Enable-PushNotificationProxy.md)

### [Export-AutoDiscoverConfig](Export-AutoDiscoverConfig.md)

### [Get-CASMailbox](Get-CASMailbox.md)

### [Get-CASMailboxPlan](Get-CASMailboxPlan.md)

### [Get-ClientAccessRule](Get-ClientAccessRule.md)

### [Get-ImapSettings](Get-ImapSettings.md)

### [Get-MailboxCalendarConfiguration](Get-MailboxCalendarConfiguration.md)

### [Get-MailboxMessageConfiguration](Get-MailboxMessageConfiguration.md)

### [Get-MailboxRegionalConfiguration](Get-MailboxRegionalConfiguration.md)

### [Get-MailboxSpellingConfiguration](Get-MailboxSpellingConfiguration.md)

### [Get-OutlookProvider](Get-OutlookProvider.md)

### [Get-OwaMailboxPolicy](Get-OwaMailboxPolicy.md)

### [Get-PopSettings](Get-PopSettings.md)

### [Get-TextMessagingAccount](Get-TextMessagingAccount.md)

### [New-ClientAccessRule](New-ClientAccessRule.md)

### [New-OutlookProvider](New-OutlookProvider.md)

### [New-OwaMailboxPolicy](New-OwaMailboxPolicy.md)

### [Remove-ClientAccessRule](Remove-ClientAccessRule.md)

### [Remove-OutlookProvider](Remove-OutlookProvider.md)

### [Remove-OwaMailboxPolicy](Remove-OwaMailboxPolicy.md)

### [Send-TextMessagingVerificationCode](Send-TextMessagingVerificationCode.md)

### [Set-CASMailbox](Set-CASMailbox.md)

### [Set-CASMailboxPlan](Set-CASMailboxPlan.md)

### [Set-ClientAccessRule](Set-ClientAccessRule.md)

### [Set-ImapSettings](Set-ImapSettings.md)

### [Set-MailboxCalendarConfiguration](Set-MailboxCalendarConfiguration.md)

### [Set-MailboxMessageConfiguration](Set-MailboxMessageConfiguration.md)

### [Set-MailboxRegionalConfiguration](Set-MailboxRegionalConfiguration.md)

### [Set-MailboxSpellingConfiguration](Set-MailboxSpellingConfiguration.md)

### [Set-OutlookProvider](Set-OutlookProvider.md)

### [Set-OwaMailboxPolicy](Set-OwaMailboxPolicy.md)

### [Set-PopSettings](Set-PopSettings.md)

### [Set-TextMessagingAccount](Set-TextMessagingAccount.md)

### [Test-CalendarConnectivity](Test-CalendarConnectivity.md)

### [Test-ClientAccessRule](Test-ClientAccessRule.md)

### [Test-EcpConnectivity](Test-EcpConnectivity.md)

### [Test-ImapConnectivity](Test-ImapConnectivity.md)

### [Test-OutlookConnectivity](Test-OutlookConnectivity.md)

### [Test-OutlookWebServices](Test-OutlookWebServices.md)

### [Test-OwaConnectivity](Test-OwaConnectivity.md)

### [Test-PopConnectivity](Test-PopConnectivity.md)

### [Test-PowerShellConnectivity](Test-PowerShellConnectivity.md)

### [Test-WebServicesConnectivity](Test-WebServicesConnectivity.md)

## client-access-servers Cmdlets
### [Disable-OutlookAnywhere](Disable-OutlookAnywhere.md)

### [Enable-OutlookAnywhere](Enable-OutlookAnywhere.md)

### [Get-ActiveSyncVirtualDirectory](Get-ActiveSyncVirtualDirectory.md)

### [Get-AuthRedirect](Get-AuthRedirect.md)

### [Get-AutodiscoverVirtualDirectory](Get-AutodiscoverVirtualDirectory.md)

### [Get-ClientAccessArray](Get-ClientAccessArray.md)

### [Get-ClientAccessServer](Get-ClientAccessServer.md)

### [Get-ClientAccessService](Get-ClientAccessService.md)

### [Get-EcpVirtualDirectory](Get-EcpVirtualDirectory.md)

### [Get-MapiVirtualDirectory](Get-MapiVirtualDirectory.md)

### [Get-OutlookAnywhere](Get-OutlookAnywhere.md)

### [Get-OwaVirtualDirectory](Get-OwaVirtualDirectory.md)

### [Get-PowerShellVirtualDirectory](Get-PowerShellVirtualDirectory.md)

### [Get-RpcClientAccess](Get-RpcClientAccess.md)

### [Get-WebServicesVirtualDirectory](Get-WebServicesVirtualDirectory.md)

### [New-ActiveSyncVirtualDirectory](New-ActiveSyncVirtualDirectory.md)

### [New-AuthRedirect](New-AuthRedirect.md)

### [New-AutodiscoverVirtualDirectory](New-AutodiscoverVirtualDirectory.md)

### [New-ClientAccessArray](New-ClientAccessArray.md)

### [New-EcpVirtualDirectory](New-EcpVirtualDirectory.md)

### [New-MapiVirtualDirectory](New-MapiVirtualDirectory.md)

### [New-OwaVirtualDirectory](New-OwaVirtualDirectory.md)

### [New-PowerShellVirtualDirectory](New-PowerShellVirtualDirectory.md)

### [New-RpcClientAccess](New-RpcClientAccess.md)

### [New-WebServicesVirtualDirectory](New-WebServicesVirtualDirectory.md)

### [Remove-ActiveSyncVirtualDirectory](Remove-ActiveSyncVirtualDirectory.md)

### [Remove-AuthRedirect](Remove-AuthRedirect.md)

### [Remove-AutodiscoverVirtualDirectory](Remove-AutodiscoverVirtualDirectory.md)

### [Remove-ClientAccessArray](Remove-ClientAccessArray.md)

### [Remove-EcpVirtualDirectory](Remove-EcpVirtualDirectory.md)

### [Remove-MapiVirtualDirectory](Remove-MapiVirtualDirectory.md)

### [Remove-OwaVirtualDirectory](Remove-OwaVirtualDirectory.md)

### [Remove-PowerShellVirtualDirectory](Remove-PowerShellVirtualDirectory.md)

### [Remove-RpcClientAccess](Remove-RpcClientAccess.md)

### [Remove-WebServicesVirtualDirectory](Remove-WebServicesVirtualDirectory.md)

### [Set-ActiveSyncVirtualDirectory](Set-ActiveSyncVirtualDirectory.md)

### [Set-AuthRedirect](Set-AuthRedirect.md)

### [Set-AutodiscoverVirtualDirectory](Set-AutodiscoverVirtualDirectory.md)

### [Set-ClientAccessArray](Set-ClientAccessArray.md)

### [Set-ClientAccessServer](Set-ClientAccessServer.md)

### [Set-ClientAccessService](Set-ClientAccessService.md)

### [Set-EcpVirtualDirectory](Set-EcpVirtualDirectory.md)

### [Set-MapiVirtualDirectory](Set-MapiVirtualDirectory.md)

### [Set-OutlookAnywhere](Set-OutlookAnywhere.md)

### [Set-OwaVirtualDirectory](Set-OwaVirtualDirectory.md)

### [Set-PowerShellVirtualDirectory](Set-PowerShellVirtualDirectory.md)

### [Set-RpcClientAccess](Set-RpcClientAccess.md)

### [Set-WebServicesVirtualDirectory](Set-WebServicesVirtualDirectory.md)

## database-availability-groups Cmdlets
### [Add-DatabaseAvailabilityGroupServer](Add-DatabaseAvailabilityGroupServer.md)

### [Add-MailboxDatabaseCopy](Add-MailboxDatabaseCopy.md)

### [Get-DatabaseAvailabilityGroup](Get-DatabaseAvailabilityGroup.md)

### [Get-DatabaseAvailabilityGroupNetwork](Get-DatabaseAvailabilityGroupNetwork.md)

### [Get-MailboxDatabaseCopyStatus](Get-MailboxDatabaseCopyStatus.md)

### [Move-ActiveMailboxDatabase](Move-ActiveMailboxDatabase.md)

### [New-DatabaseAvailabilityGroup](New-DatabaseAvailabilityGroup.md)

### [New-DatabaseAvailabilityGroupNetwork](New-DatabaseAvailabilityGroupNetwork.md)

### [Remove-DatabaseAvailabilityGroup](Remove-DatabaseAvailabilityGroup.md)

### [Remove-DatabaseAvailabilityGroupNetwork](Remove-DatabaseAvailabilityGroupNetwork.md)

### [Remove-DatabaseAvailabilityGroupServer](Remove-DatabaseAvailabilityGroupServer.md)

### [Remove-MailboxDatabaseCopy](Remove-MailboxDatabaseCopy.md)

### [Restore-DatabaseAvailabilityGroup](Restore-DatabaseAvailabilityGroup.md)

### [Resume-MailboxDatabaseCopy](Resume-MailboxDatabaseCopy.md)

### [Set-DatabaseAvailabilityGroup](Set-DatabaseAvailabilityGroup.md)

### [Set-DatabaseAvailabilityGroupNetwork](Set-DatabaseAvailabilityGroupNetwork.md)

### [Set-MailboxDatabaseCopy](Set-MailboxDatabaseCopy.md)

### [Start-DatabaseAvailabilityGroup](Start-DatabaseAvailabilityGroup.md)

### [Stop-DatabaseAvailabilityGroup](Stop-DatabaseAvailabilityGroup.md)

### [Suspend-MailboxDatabaseCopy](Suspend-MailboxDatabaseCopy.md)

### [Test-ReplicationHealth](Test-ReplicationHealth.md)

### [Update-MailboxDatabaseCopy](Update-MailboxDatabaseCopy.md)

## devices Cmdlets
### [Clear-ActiveSyncDevice](Clear-ActiveSyncDevice.md)

### [Clear-MobileDevice](Clear-MobileDevice.md)

### [Export-ActiveSyncLog](Export-ActiveSyncLog.md)

### [Get-ActiveSyncDevice](Get-ActiveSyncDevice.md)

### [Get-ActiveSyncDeviceAccessRule](Get-ActiveSyncDeviceAccessRule.md)

### [Get-ActiveSyncDeviceAutoblockThreshold](Get-ActiveSyncDeviceAutoblockThreshold.md)

### [Get-ActiveSyncDeviceClass](Get-ActiveSyncDeviceClass.md)

### [Get-ActiveSyncDeviceStatistics](Get-ActiveSyncDeviceStatistics.md)

### [Get-ActiveSyncMailboxPolicy](Get-ActiveSyncMailboxPolicy.md)

### [Get-ActiveSyncOrganizationSettings](Get-ActiveSyncOrganizationSettings.md)

### [Get-DeviceConditionalAccessPolicy](Get-DeviceConditionalAccessPolicy.md)

### [Get-DeviceConditionalAccessRule](Get-DeviceConditionalAccessRule.md)

### [Get-DeviceConfigurationPolicy](Get-DeviceConfigurationPolicy.md)

### [Get-DeviceConfigurationRule](Get-DeviceConfigurationRule.md)

### [Get-DevicePolicy](Get-DevicePolicy.md)

### [Get-DeviceTenantPolicy](Get-DeviceTenantPolicy.md)

### [Get-DeviceTenantRule](Get-DeviceTenantRule.md)

### [Get-MobileDevice](Get-MobileDevice.md)

### [Get-MobileDeviceMailboxPolicy](Get-MobileDeviceMailboxPolicy.md)

### [Get-MobileDeviceStatistics](Get-MobileDeviceStatistics.md)

### [New-ActiveSyncDeviceAccessRule](New-ActiveSyncDeviceAccessRule.md)

### [New-ActiveSyncMailboxPolicy](New-ActiveSyncMailboxPolicy.md)

### [New-DeviceConditionalAccessPolicy](New-DeviceConditionalAccessPolicy.md)

### [New-DeviceConditionalAccessRule](New-DeviceConditionalAccessRule.md)

### [New-DeviceConfigurationPolicy](New-DeviceConfigurationPolicy.md)

### [New-DeviceConfigurationRule](New-DeviceConfigurationRule.md)

### [New-DeviceTenantPolicy](New-DeviceTenantPolicy.md)

### [New-DeviceTenantRule](New-DeviceTenantRule.md)

### [New-MobileDeviceMailboxPolicy](New-MobileDeviceMailboxPolicy.md)

### [Remove-ActiveSyncDevice](Remove-ActiveSyncDevice.md)

### [Remove-ActiveSyncDeviceAccessRule](Remove-ActiveSyncDeviceAccessRule.md)

### [Remove-ActiveSyncDeviceClass](Remove-ActiveSyncDeviceClass.md)

### [Remove-ActiveSyncMailboxPolicy](Remove-ActiveSyncMailboxPolicy.md)

### [Remove-DeviceConditionalAccessPolicy](Remove-DeviceConditionalAccessPolicy.md)

### [Remove-DeviceConditionalAccessRule](Remove-DeviceConditionalAccessRule.md)

### [Remove-DeviceConfigurationPolicy](Remove-DeviceConfigurationPolicy.md)

### [Remove-DeviceConfigurationRule](Remove-DeviceConfigurationRule.md)

### [Remove-DeviceTenantPolicy](Remove-DeviceTenantPolicy.md)

### [Remove-DeviceTenantRule](Remove-DeviceTenantRule.md)

### [Remove-MobileDevice](Remove-MobileDevice.md)

### [Remove-MobileDeviceMailboxPolicy](Remove-MobileDeviceMailboxPolicy.md)

### [Set-ActiveSyncDeviceAccessRule](Set-ActiveSyncDeviceAccessRule.md)

### [Set-ActiveSyncDeviceAutoblockThreshold](Set-ActiveSyncDeviceAutoblockThreshold.md)

### [Set-ActiveSyncMailboxPolicy](Set-ActiveSyncMailboxPolicy.md)

### [Set-ActiveSyncOrganizationSettings](Set-ActiveSyncOrganizationSettings.md)

### [Set-DeviceConditionalAccessPolicy](Set-DeviceConditionalAccessPolicy.md)

### [Set-DeviceConditionalAccessRule](Set-DeviceConditionalAccessRule.md)

### [Set-DeviceConfigurationPolicy](Set-DeviceConfigurationPolicy.md)

### [Set-DeviceConfigurationRule](Set-DeviceConfigurationRule.md)

### [Set-DeviceTenantPolicy](Set-DeviceTenantPolicy.md)

### [Set-DeviceTenantRule](Set-DeviceTenantRule.md)

### [Set-MobileDeviceMailboxPolicy](Set-MobileDeviceMailboxPolicy.md)

### [Test-ActiveSyncConnectivity](Test-ActiveSyncConnectivity.md)

## email-addresses-and-address-books Cmdlets
### [Disable-AddressListPaging](Disable-AddressListPaging.md)

### [Enable-AddressListPaging](Enable-AddressListPaging.md)

### [Get-AddressBookPolicy](Get-AddressBookPolicy.md)

### [Get-AddressList](Get-AddressList.md)

### [Get-DetailsTemplate](Get-DetailsTemplate.md)

### [Get-EmailAddressPolicy](Get-EmailAddressPolicy.md)

### [Get-GlobalAddressList](Get-GlobalAddressList.md)

### [Get-OabVirtualDirectory](Get-OabVirtualDirectory.md)

### [Get-OfflineAddressBook](Get-OfflineAddressBook.md)

### [Move-AddressList](Move-AddressList.md)

### [Move-OfflineAddressBook](Move-OfflineAddressBook.md)

### [New-AddressBookPolicy](New-AddressBookPolicy.md)

### [New-AddressList](New-AddressList.md)

### [New-EmailAddressPolicy](New-EmailAddressPolicy.md)

### [New-GlobalAddressList](New-GlobalAddressList.md)

### [New-OabVirtualDirectory](New-OabVirtualDirectory.md)

### [New-OfflineAddressBook](New-OfflineAddressBook.md)

### [Remove-AddressBookPolicy](Remove-AddressBookPolicy.md)

### [Remove-AddressList](Remove-AddressList.md)

### [Remove-EmailAddressPolicy](Remove-EmailAddressPolicy.md)

### [Remove-GlobalAddressList](Remove-GlobalAddressList.md)

### [Remove-OabVirtualDirectory](Remove-OabVirtualDirectory.md)

### [Remove-OfflineAddressBook](Remove-OfflineAddressBook.md)

### [Restore-DetailsTemplate](Restore-DetailsTemplate.md)

### [Set-AddressBookPolicy](Set-AddressBookPolicy.md)

### [Set-AddressList](Set-AddressList.md)

### [Set-DetailsTemplate](Set-DetailsTemplate.md)

### [Set-EmailAddressPolicy](Set-EmailAddressPolicy.md)

### [Set-GlobalAddressList](Set-GlobalAddressList.md)

### [Set-OabVirtualDirectory](Set-OabVirtualDirectory.md)

### [Set-OfflineAddressBook](Set-OfflineAddressBook.md)

### [Update-AddressList](Update-AddressList.md)

### [Update-EmailAddressPolicy](Update-EmailAddressPolicy.md)

### [Update-GlobalAddressList](Update-GlobalAddressList.md)

### [Update-OfflineAddressBook](Update-OfflineAddressBook.md)

## encryption-and-certificates Cmdlets
### [Enable-ExchangeCertificate](Enable-ExchangeCertificate.md)

### [Export-ExchangeCertificate](Export-ExchangeCertificate.md)

### [Get-DataEncryptionPolicy](Get-DataEncryptionPolicy.md)

### [Get-ExchangeCertificate](Get-ExchangeCertificate.md)

### [Get-IRMConfiguration](Get-IRMConfiguration.md)

### [Get-M365DataAtRestEncryptionPolicy](Get-M365DataAtRestEncryptionPolicy.md)

### [Get-M365DataAtRestEncryptionPolicyAssignment](Get-M365DataAtRestEncryptionPolicyAssignment.md)

### [Get-MailboxIRMAccess](Get-MailboxIRMAccess.md)

### [Get-OMEConfiguration](Get-OMEConfiguration.md)

### [Get-OMEMessageStatus](Get-OMEMessageStatus.md)

### [Get-RMSTemplate](Get-RMSTemplate.md)

### [Get-SmimeConfig](Get-SmimeConfig.md)

### [Import-ExchangeCertificate](Import-ExchangeCertificate.md)

### [New-DataEncryptionPolicy](New-DataEncryptionPolicy.md)

### [New-ExchangeCertificate](New-ExchangeCertificate.md)

### [New-M365DataAtRestEncryptionPolicy](New-M365DataAtRestEncryptionPolicy.md)

### [New-OMEConfiguration](New-OMEConfiguration.md)

### [Remove-ExchangeCertificate](Remove-ExchangeCertificate.md)

### [Remove-MailboxIRMAccess](Remove-MailboxIRMAccess.md)

### [Remove-OMEConfiguration](Remove-OMEConfiguration.md)

### [Set-DataEncryptionPolicy](Set-DataEncryptionPolicy.md)

### [Set-IRMConfiguration](Set-IRMConfiguration.md)

### [Set-M365DataAtRestEncryptionPolicy](Set-M365DataAtRestEncryptionPolicy.md)

### [Set-M365DataAtRestEncryptionPolicyAssignment](Set-M365DataAtRestEncryptionPolicyAssignment.md)

### [Set-MailboxIRMAccess](Set-MailboxIRMAccess.md)

### [Set-OMEConfiguration](Set-OMEConfiguration.md)

### [Set-OMEMessageRevocation](Set-OMEMessageRevocation.md)

### [Set-RMSTemplate](Set-RMSTemplate.md)

### [Set-SmimeConfig](Set-SmimeConfig.md)

### [Test-IRMConfiguration](Test-IRMConfiguration.md)

## federation-and-hybrid Cmdlets
### [Add-FederatedDomain](Add-FederatedDomain.md)

### [Disable-RemoteMailbox](Disable-RemoteMailbox.md)

### [Enable-RemoteMailbox](Enable-RemoteMailbox.md)

### [Get-FederatedDomainProof](Get-FederatedDomainProof.md)

### [Get-FederatedOrganizationIdentifier](Get-FederatedOrganizationIdentifier.md)

### [Get-FederationInformation](Get-FederationInformation.md)

### [Get-FederationTrust](Get-FederationTrust.md)

### [Get-HybridConfiguration](Get-HybridConfiguration.md)

### [Get-HybridMailflow](Get-HybridMailflow.md)

### [Get-HybridMailflowDatacenterIPs](Get-HybridMailflowDatacenterIPs.md)

### [Get-IntraOrganizationConfiguration](Get-IntraOrganizationConfiguration.md)

### [Get-IntraOrganizationConnector](Get-IntraOrganizationConnector.md)

### [Get-OnPremisesOrganization](Get-OnPremisesOrganization.md)

### [Get-PendingFederatedDomain](Get-PendingFederatedDomain.md)

### [Get-RemoteMailbox](Get-RemoteMailbox.md)

### [New-FederationTrust](New-FederationTrust.md)

### [New-HybridConfiguration](New-HybridConfiguration.md)

### [New-IntraOrganizationConnector](New-IntraOrganizationConnector.md)

### [New-OnPremisesOrganization](New-OnPremisesOrganization.md)

### [New-RemoteMailbox](New-RemoteMailbox.md)

### [Remove-FederatedDomain](Remove-FederatedDomain.md)

### [Remove-FederationTrust](Remove-FederationTrust.md)

### [Remove-HybridConfiguration](Remove-HybridConfiguration.md)

### [Remove-IntraOrganizationConnector](Remove-IntraOrganizationConnector.md)

### [Remove-OnPremisesOrganization](Remove-OnPremisesOrganization.md)

### [Remove-RemoteMailbox](Remove-RemoteMailbox.md)

### [Set-FederatedOrganizationIdentifier](Set-FederatedOrganizationIdentifier.md)

### [Set-FederationTrust](Set-FederationTrust.md)

### [Set-HybridConfiguration](Set-HybridConfiguration.md)

### [Set-HybridMailflow](Set-HybridMailflow.md)

### [Set-IntraOrganizationConnector](Set-IntraOrganizationConnector.md)

### [Set-OnPremisesOrganization](Set-OnPremisesOrganization.md)

### [Set-PendingFederatedDomain](Set-PendingFederatedDomain.md)

### [Set-RemoteMailbox](Set-RemoteMailbox.md)

### [Test-FederationTrust](Test-FederationTrust.md)

### [Test-FederationTrustCertificate](Test-FederationTrustCertificate.md)

### [Update-HybridConfiguration](Update-HybridConfiguration.md)

### [Update-Recipient](Update-Recipient.md)

## mailbox-databases-and-servers Cmdlets
### [Clean-MailboxDatabase](Clean-MailboxDatabase.md)

### [Disable-MailboxQuarantine](Disable-MailboxQuarantine.md)

### [Disable-MetaCacheDatabase](Disable-MetaCacheDatabase.md)

### [Dismount-Database](Dismount-Database.md)

### [Enable-MailboxQuarantine](Enable-MailboxQuarantine.md)

### [Enable-MetaCacheDatabase](Enable-MetaCacheDatabase.md)

### [Get-FailedContentIndexDocuments](Get-FailedContentIndexDocuments.md)

### [Get-MailboxDatabase](Get-MailboxDatabase.md)

### [Get-MailboxRepairRequest](Get-MailboxRepairRequest.md)

### [Get-MailboxServer](Get-MailboxServer.md)

### [Get-SearchDocumentFormat](Get-SearchDocumentFormat.md)

### [Get-StoreUsageStatistics](Get-StoreUsageStatistics.md)

### [Mount-Database](Mount-Database.md)

### [Move-DatabasePath](Move-DatabasePath.md)

### [New-MailboxDatabase](New-MailboxDatabase.md)

### [New-MailboxRepairRequest](New-MailboxRepairRequest.md)

### [New-SearchDocumentFormat](New-SearchDocumentFormat.md)

### [Remove-MailboxDatabase](Remove-MailboxDatabase.md)

### [Remove-MailboxRepairRequest](Remove-MailboxRepairRequest.md)

### [Remove-SearchDocumentFormat](Remove-SearchDocumentFormat.md)

### [Remove-StoreMailbox](Remove-StoreMailbox.md)

### [Set-MailboxDatabase](Set-MailboxDatabase.md)

### [Set-MailboxServer](Set-MailboxServer.md)

### [Set-SearchDocumentFormat](Set-SearchDocumentFormat.md)

### [Start-MailboxAssistant](Start-MailboxAssistant.md)

### [Test-AssistantHealth](Test-AssistantHealth.md)

### [Test-ExchangeSearch](Test-ExchangeSearch.md)

### [Test-MRSHealth](Test-MRSHealth.md)

### [Update-DatabaseSchema](Update-DatabaseSchema.md)

### [Update-FileDistributionService](Update-FileDistributionService.md)

### [Update-StoreMailboxState](Update-StoreMailboxState.md)

## mailboxes Cmdlets
### [Add-MailboxFolderPermission](Add-MailboxFolderPermission.md)

### [Add-MailboxPermission](Add-MailboxPermission.md)

### [Add-RecipientPermission](Add-RecipientPermission.md)

### [Connect-Mailbox](Connect-Mailbox.md)

### [Disable-App](Disable-App.md)

### [Disable-InboxRule](Disable-InboxRule.md)

### [Disable-Mailbox](Disable-Mailbox.md)

### [Disable-ServiceEmailChannel](Disable-ServiceEmailChannel.md)

### [Disable-SweepRule](Disable-SweepRule.md)

### [Enable-App](Enable-App.md)

### [Enable-InboxRule](Enable-InboxRule.md)

### [Enable-Mailbox](Enable-Mailbox.md)

### [Enable-ServiceEmailChannel](Enable-ServiceEmailChannel.md)

### [Enable-SweepRule](Enable-SweepRule.md)

### [Export-MailboxDiagnosticLogs](Export-MailboxDiagnosticLogs.md)

### [Export-RecipientDataProperty](Export-RecipientDataProperty.md)

### [Get-App](Get-App.md)

### [Get-CalendarDiagnosticAnalysis](Get-CalendarDiagnosticAnalysis.md)

### [Get-CalendarDiagnosticLog](Get-CalendarDiagnosticLog.md)

### [Get-CalendarDiagnosticObjects](Get-CalendarDiagnosticObjects.md)

### [Get-CalendarNotification](Get-CalendarNotification.md)

### [Get-CalendarProcessing](Get-CalendarProcessing.md)

### [Get-Clutter](Get-Clutter.md)

### [Get-EventsFromEmailConfiguration](Get-EventsFromEmailConfiguration.md)

### [Get-ExternalInOutlook](Get-ExternalInOutlook.md)

### [Get-FocusedInbox](Get-FocusedInbox.md)

### [Get-InboxRule](Get-InboxRule.md)

### [Get-Mailbox](Get-Mailbox.md)

### [Get-MailboxAutoReplyConfiguration](Get-MailboxAutoReplyConfiguration.md)

### [Get-MailboxCalendarFolder](Get-MailboxCalendarFolder.md)

### [Get-MailboxExportRequest](Get-MailboxExportRequest.md)

### [Get-MailboxExportRequestStatistics](Get-MailboxExportRequestStatistics.md)

### [Get-MailboxFolder](Get-MailboxFolder.md)

### [Get-MailboxFolderPermission](Get-MailboxFolderPermission.md)

### [Get-MailboxFolderStatistics](Get-MailboxFolderStatistics.md)

### [Get-MailboxImportRequest](Get-MailboxImportRequest.md)

### [Get-MailboxImportRequestStatistics](Get-MailboxImportRequestStatistics.md)

### [Get-MailboxLocation](Get-MailboxLocation.md)

### [Get-MailboxPermission](Get-MailboxPermission.md)

### [Get-MailboxPlan](Get-MailboxPlan.md)

### [Get-MailboxRestoreRequest](Get-MailboxRestoreRequest.md)

### [Get-MailboxRestoreRequestStatistics](Get-MailboxRestoreRequestStatistics.md)

### [Get-MailboxSentItemsConfiguration](Get-MailboxSentItemsConfiguration.md)

### [Get-MailboxStatistics](Get-MailboxStatistics.md)

### [Get-MailboxUserConfiguration](Get-MailboxUserConfiguration.md)

### [Get-MessageCategory](Get-MessageCategory.md)

### [Get-Place](Get-Place.md)

### [Get-RecipientPermission](Get-RecipientPermission.md)

### [Get-RecoverableItems](Get-RecoverableItems.md)

### [Get-ResourceConfig](Get-ResourceConfig.md)

### [Get-SweepRule](Get-SweepRule.md)

### [Get-UserPhoto](Get-UserPhoto.md)

### [Import-RecipientDataProperty](Import-RecipientDataProperty.md)

### [New-App](New-App.md)

### [New-InboxRule](New-InboxRule.md)

### [New-Mailbox](New-Mailbox.md)

### [New-MailboxExportRequest](New-MailboxExportRequest.md)

### [New-MailboxFolder](New-MailboxFolder.md)

### [New-MailboxImportRequest](New-MailboxImportRequest.md)

### [New-MailboxRestoreRequest](New-MailboxRestoreRequest.md)

### [New-MailMessage](New-MailMessage.md)

### [New-SiteMailbox](New-SiteMailbox.md)

### [New-SweepRule](New-SweepRule.md)

### [Remove-App](Remove-App.md)

### [Remove-CalendarEvents](Remove-CalendarEvents.md)

### [Remove-InboxRule](Remove-InboxRule.md)

### [Remove-Mailbox](Remove-Mailbox.md)

### [Remove-MailboxExportRequest](Remove-MailboxExportRequest.md)

### [Remove-MailboxFolderPermission](Remove-MailboxFolderPermission.md)

### [Remove-MailboxImportRequest](Remove-MailboxImportRequest.md)

### [Remove-MailboxPermission](Remove-MailboxPermission.md)

### [Remove-MailboxRestoreRequest](Remove-MailboxRestoreRequest.md)

### [Remove-MailboxUserConfiguration](Remove-MailboxUserConfiguration.md)

### [Remove-RecipientPermission](Remove-RecipientPermission.md)

### [Remove-SweepRule](Remove-SweepRule.md)

### [Remove-UserPhoto](Remove-UserPhoto.md)

### [Restore-Mailbox](Restore-Mailbox.md)

### [Restore-RecoverableItems](Restore-RecoverableItems.md)

### [Resume-MailboxExportRequest](Resume-MailboxExportRequest.md)

### [Resume-MailboxImportRequest](Resume-MailboxImportRequest.md)

### [Resume-MailboxRestoreRequest](Resume-MailboxRestoreRequest.md)

### [Search-Mailbox](Search-Mailbox.md)

### [Set-App](Set-App.md)

### [Set-CalendarNotification](Set-CalendarNotification.md)

### [Set-CalendarProcessing](Set-CalendarProcessing.md)

### [Set-Clutter](Set-Clutter.md)

### [Set-EventsFromEmailConfiguration](Set-EventsFromEmailConfiguration.md)

### [Set-ExternalInOutlook](Set-ExternalInOutlook.md)

### [Set-FocusedInbox](Set-FocusedInbox.md)

### [Set-InboxRule](Set-InboxRule.md)

### [Set-Mailbox](Set-Mailbox.md)

### [Set-MailboxAutoReplyConfiguration](Set-MailboxAutoReplyConfiguration.md)

### [Set-MailboxCalendarFolder](Set-MailboxCalendarFolder.md)

### [Set-MailboxExportRequest](Set-MailboxExportRequest.md)

### [Set-MailboxFolderPermission](Set-MailboxFolderPermission.md)

### [Set-MailboxImportRequest](Set-MailboxImportRequest.md)

### [Set-MailboxPlan](Set-MailboxPlan.md)

### [Set-MailboxRestoreRequest](Set-MailboxRestoreRequest.md)

### [Set-MailboxSentItemsConfiguration](Set-MailboxSentItemsConfiguration.md)

### [Set-Place](Set-Place.md)

### [Set-ResourceConfig](Set-ResourceConfig.md)

### [Set-SweepRule](Set-SweepRule.md)

### [Set-UserPhoto](Set-UserPhoto.md)

### [Suspend-MailboxExportRequest](Suspend-MailboxExportRequest.md)

### [Suspend-MailboxImportRequest](Suspend-MailboxImportRequest.md)

### [Suspend-MailboxRestoreRequest](Suspend-MailboxRestoreRequest.md)

### [Test-MAPIConnectivity](Test-MAPIConnectivity.md)

### [Undo-SoftDeletedMailbox](Undo-SoftDeletedMailbox.md)

## mail-flow Cmdlets
### [Add-ResubmitRequest](Add-ResubmitRequest.md)

### [Disable-TransportAgent](Disable-TransportAgent.md)

### [Enable-TransportAgent](Enable-TransportAgent.md)

### [Export-Message](Export-Message.md)

### [Get-AcceptedDomain](Get-AcceptedDomain.md)

### [Get-AddressRewriteEntry](Get-AddressRewriteEntry.md)

### [Get-DeliveryAgentConnector](Get-DeliveryAgentConnector.md)

### [Get-EdgeSubscription](Get-EdgeSubscription.md)

### [Get-EdgeSyncServiceConfig](Get-EdgeSyncServiceConfig.md)

### [Get-ForeignConnector](Get-ForeignConnector.md)

### [Get-FrontendTransportService](Get-FrontendTransportService.md)

### [Get-InboundConnector](Get-InboundConnector.md)

### [Get-MailboxTransportService](Get-MailboxTransportService.md)

### [Get-Message](Get-Message.md)

### [Get-MessageTrace](Get-MessageTrace.md)

### [Get-MessageTraceDetail](Get-MessageTraceDetail.md)

### [Get-MessageTrackingLog](Get-MessageTrackingLog.md)

### [Get-MessageTrackingReport](Get-MessageTrackingReport.md)

### [Get-NetworkConnectionInfo](Get-NetworkConnectionInfo.md)

### [Get-OutboundConnector](Get-OutboundConnector.md)

### [Get-Queue](Get-Queue.md)

### [Get-QueueDigest](Get-QueueDigest.md)

### [Get-ReceiveConnector](Get-ReceiveConnector.md)

### [Get-RemoteDomain](Get-RemoteDomain.md)

### [Get-ResubmitRequest](Get-ResubmitRequest.md)

### [Get-RoutingGroupConnector](Get-RoutingGroupConnector.md)

### [Get-SendConnector](Get-SendConnector.md)

### [Get-SystemMessage](Get-SystemMessage.md)

### [Get-TransportAgent](Get-TransportAgent.md)

### [Get-TransportConfig](Get-TransportConfig.md)

### [Get-TransportPipeline](Get-TransportPipeline.md)

### [Get-TransportServer](Get-TransportServer.md)

### [Get-TransportService](Get-TransportService.md)

### [Get-X400AuthoritativeDomain](Get-X400AuthoritativeDomain.md)

### [Install-TransportAgent](Install-TransportAgent.md)

### [New-AcceptedDomain](New-AcceptedDomain.md)

### [New-AddressRewriteEntry](New-AddressRewriteEntry.md)

### [New-DeliveryAgentConnector](New-DeliveryAgentConnector.md)

### [New-EdgeSubscription](New-EdgeSubscription.md)

### [New-EdgeSyncServiceConfig](New-EdgeSyncServiceConfig.md)

### [New-ForeignConnector](New-ForeignConnector.md)

### [New-InboundConnector](New-InboundConnector.md)

### [New-OutboundConnector](New-OutboundConnector.md)

### [New-ReceiveConnector](New-ReceiveConnector.md)

### [New-RemoteDomain](New-RemoteDomain.md)

### [New-RoutingGroupConnector](New-RoutingGroupConnector.md)

### [New-SendConnector](New-SendConnector.md)

### [New-SystemMessage](New-SystemMessage.md)

### [New-X400AuthoritativeDomain](New-X400AuthoritativeDomain.md)

### [Redirect-Message](Redirect-Message.md)

### [Remove-AcceptedDomain](Remove-AcceptedDomain.md)

### [Remove-AddressRewriteEntry](Remove-AddressRewriteEntry.md)

### [Remove-DeliveryAgentConnector](Remove-DeliveryAgentConnector.md)

### [Remove-EdgeSubscription](Remove-EdgeSubscription.md)

### [Remove-ForeignConnector](Remove-ForeignConnector.md)

### [Remove-InboundConnector](Remove-InboundConnector.md)

### [Remove-Message](Remove-Message.md)

### [Remove-OutboundConnector](Remove-OutboundConnector.md)

### [Remove-ReceiveConnector](Remove-ReceiveConnector.md)

### [Remove-RemoteDomain](Remove-RemoteDomain.md)

### [Remove-ResubmitRequest](Remove-ResubmitRequest.md)

### [Remove-RoutingGroupConnector](Remove-RoutingGroupConnector.md)

### [Remove-SendConnector](Remove-SendConnector.md)

### [Remove-SystemMessage](Remove-SystemMessage.md)

### [Remove-X400AuthoritativeDomain](Remove-X400AuthoritativeDomain.md)

### [Resume-Message](Resume-Message.md)

### [Resume-Queue](Resume-Queue.md)

### [Retry-Queue](Retry-Queue.md)

### [Search-MessageTrackingReport](Search-MessageTrackingReport.md)

### [Set-AcceptedDomain](Set-AcceptedDomain.md)

### [Set-AddressRewriteEntry](Set-AddressRewriteEntry.md)

### [Set-DeliveryAgentConnector](Set-DeliveryAgentConnector.md)

### [Set-EdgeSyncServiceConfig](Set-EdgeSyncServiceConfig.md)

### [Set-ForeignConnector](Set-ForeignConnector.md)

### [Set-FrontendTransportService](Set-FrontendTransportService.md)

### [Set-InboundConnector](Set-InboundConnector.md)

### [Set-MailboxTransportService](Set-MailboxTransportService.md)

### [Set-OutboundConnector](Set-OutboundConnector.md)

### [Set-ReceiveConnector](Set-ReceiveConnector.md)

### [Set-RemoteDomain](Set-RemoteDomain.md)

### [Set-ResubmitRequest](Set-ResubmitRequest.md)

### [Set-RoutingGroupConnector](Set-RoutingGroupConnector.md)

### [Set-SendConnector](Set-SendConnector.md)

### [Set-SystemMessage](Set-SystemMessage.md)

### [Set-TransportAgent](Set-TransportAgent.md)

### [Set-TransportConfig](Set-TransportConfig.md)

### [Set-TransportServer](Set-TransportServer.md)

### [Set-TransportService](Set-TransportService.md)

### [Set-X400AuthoritativeDomain](Set-X400AuthoritativeDomain.md)

### [Start-EdgeSynchronization](Start-EdgeSynchronization.md)

### [Start-HistoricalSearch](Start-HistoricalSearch.md)

### [Stop-HistoricalSearch](Stop-HistoricalSearch.md)

### [Suspend-Message](Suspend-Message.md)

### [Suspend-Queue](Suspend-Queue.md)

### [Test-EdgeSynchronization](Test-EdgeSynchronization.md)

### [Test-Mailflow](Test-Mailflow.md)

### [Test-SmtpConnectivity](Test-SmtpConnectivity.md)

### [Uninstall-TransportAgent](Uninstall-TransportAgent.md)

### [Validate-OutboundConnector](Validate-OutboundConnector.md)

## move-and-migration Cmdlets
### [Complete-MigrationBatch](Complete-MigrationBatch.md)

### [Export-MigrationReport](Export-MigrationReport.md)

### [Get-MigrationBatch](Get-MigrationBatch.md)

### [Get-MigrationConfig](Get-MigrationConfig.md)

### [Get-MigrationEndpoint](Get-MigrationEndpoint.md)

### [Get-MigrationStatistics](Get-MigrationStatistics.md)

### [Get-MigrationUser](Get-MigrationUser.md)

### [Get-MigrationUserStatistics](Get-MigrationUserStatistics.md)

### [Get-MoveRequest](Get-MoveRequest.md)

### [Get-MoveRequestStatistics](Get-MoveRequestStatistics.md)

### [Get-PublicFolderMailboxMigrationRequest](Get-PublicFolderMailboxMigrationRequest.md)

### [Get-PublicFolderMailboxMigrationRequestStatistics](Get-PublicFolderMailboxMigrationRequestStatistics.md)

### [Get-PublicFolderMigrationRequest](Get-PublicFolderMigrationRequest.md)

### [Get-PublicFolderMigrationRequestStatistics](Get-PublicFolderMigrationRequestStatistics.md)

### [Get-PublicFolderMoveRequest](Get-PublicFolderMoveRequest.md)

### [Get-PublicFolderMoveRequestStatistics](Get-PublicFolderMoveRequestStatistics.md)

### [New-MigrationBatch](New-MigrationBatch.md)

### [New-MigrationEndpoint](New-MigrationEndpoint.md)

### [New-MoveRequest](New-MoveRequest.md)

### [New-PublicFolderMigrationRequest](New-PublicFolderMigrationRequest.md)

### [New-PublicFolderMoveRequest](New-PublicFolderMoveRequest.md)

### [Remove-MigrationBatch](Remove-MigrationBatch.md)

### [Remove-MigrationEndpoint](Remove-MigrationEndpoint.md)

### [Remove-MigrationUser](Remove-MigrationUser.md)

### [Remove-MoveRequest](Remove-MoveRequest.md)

### [Remove-PublicFolderMailboxMigrationRequest](Remove-PublicFolderMailboxMigrationRequest.md)

### [Remove-PublicFolderMigrationRequest](Remove-PublicFolderMigrationRequest.md)

### [Remove-PublicFolderMoveRequest](Remove-PublicFolderMoveRequest.md)

### [Resume-MoveRequest](Resume-MoveRequest.md)

### [Resume-PublicFolderMigrationRequest](Resume-PublicFolderMigrationRequest.md)

### [Resume-PublicFolderMoveRequest](Resume-PublicFolderMoveRequest.md)

### [Set-MigrationBatch](Set-MigrationBatch.md)

### [Set-MigrationConfig](Set-MigrationConfig.md)

### [Set-MigrationEndpoint](Set-MigrationEndpoint.md)

### [Set-MigrationUser](Set-MigrationUser.md)

### [Set-MoveRequest](Set-MoveRequest.md)

### [Set-PublicFolderMigrationRequest](Set-PublicFolderMigrationRequest.md)

### [Set-PublicFolderMoveRequest](Set-PublicFolderMoveRequest.md)

### [Start-MigrationBatch](Start-MigrationBatch.md)

### [Start-MigrationUser](Start-MigrationUser.md)

### [Stop-MigrationBatch](Stop-MigrationBatch.md)

### [Stop-MigrationUser](Stop-MigrationUser.md)

### [Suspend-MoveRequest](Suspend-MoveRequest.md)

### [Suspend-PublicFolderMailboxMigrationRequest](Suspend-PublicFolderMailboxMigrationRequest.md)

### [Suspend-PublicFolderMigrationRequest](Suspend-PublicFolderMigrationRequest.md)

### [Suspend-PublicFolderMoveRequest](Suspend-PublicFolderMoveRequest.md)

### [Test-MigrationServerAvailability](Test-MigrationServerAvailability.md)

## organization Cmdlets
### [Disable-CmdletExtensionAgent](Disable-CmdletExtensionAgent.md)

### [Enable-CmdletExtensionAgent](Enable-CmdletExtensionAgent.md)

### [Enable-OrganizationCustomization](Enable-OrganizationCustomization.md)

### [Get-AccessToCustomerDataRequest](Get-AccessToCustomerDataRequest.md)

### [Get-ApplicationAccessPolicy](Get-ApplicationAccessPolicy.md)

### [Get-AuthConfig](Get-AuthConfig.md)

### [Get-AuthenticationPolicy](Get-AuthenticationPolicy.md)

### [Get-AuthServer](Get-AuthServer.md)

### [Get-CmdletExtensionAgent](Get-CmdletExtensionAgent.md)

### [Get-ExchangeAssistanceConfig](Get-ExchangeAssistanceConfig.md)

### [Get-ExchangeDiagnosticInfo](Get-ExchangeDiagnosticInfo.md)

### [Get-ExchangeServer](Get-ExchangeServer.md)

### [Get-ExchangeServerAccessLicense](Get-ExchangeServerAccessLicense.md)

### [Get-ExchangeServerAccessLicenseUser](Get-ExchangeServerAccessLicenseUser.md)

### [Get-ExchangeSettings](Get-ExchangeSettings.md)

### [Get-Notification](Get-Notification.md)

### [Get-OrganizationConfig](Get-OrganizationConfig.md)

### [Get-PartnerApplication](Get-PartnerApplication.md)

### [Get-PerimeterConfig](Get-PerimeterConfig.md)

### [Get-ServicePrincipal](Get-ServicePrincipal.md)

### [Get-SettingOverride](Get-SettingOverride.md)

### [New-ApplicationAccessPolicy](New-ApplicationAccessPolicy.md)

### [New-AuthenticationPolicy](New-AuthenticationPolicy.md)

### [New-AuthServer](New-AuthServer.md)

### [New-ExchangeSettings](New-ExchangeSettings.md)

### [New-PartnerApplication](New-PartnerApplication.md)

### [New-ServicePrincipal](New-ServicePrincipal.md)

### [New-SettingOverride](New-SettingOverride.md)

### [Remove-ApplicationAccessPolicy](Remove-ApplicationAccessPolicy.md)

### [Remove-AuthenticationPolicy](Remove-AuthenticationPolicy.md)

### [Remove-AuthServer](Remove-AuthServer.md)

### [Remove-PartnerApplication](Remove-PartnerApplication.md)

### [Remove-ServicePrincipal](Remove-ServicePrincipal.md)

### [Remove-SettingOverride](Remove-SettingOverride.md)

### [Set-AccessToCustomerDataRequest](Set-AccessToCustomerDataRequest.md)

### [Set-ApplicationAccessPolicy](Set-ApplicationAccessPolicy.md)

### [Set-AuthConfig](Set-AuthConfig.md)

### [Set-AuthenticationPolicy](Set-AuthenticationPolicy.md)

### [Set-AuthServer](Set-AuthServer.md)

### [Set-CmdletExtensionAgent](Set-CmdletExtensionAgent.md)

### [Set-ExchangeAssistanceConfig](Set-ExchangeAssistanceConfig.md)

### [Set-ExchangeServer](Set-ExchangeServer.md)

### [Set-ExchangeSettings](Set-ExchangeSettings.md)

### [Set-Notification](Set-Notification.md)

### [Set-OrganizationConfig](Set-OrganizationConfig.md)

### [Set-PartnerApplication](Set-PartnerApplication.md)

### [Set-PerimeterConfig](Set-PerimeterConfig.md)

### [Set-ServicePrincipal](Set-ServicePrincipal.md)

### [Set-SettingOverride](Set-SettingOverride.md)

### [Test-ApplicationAccessPolicy](Test-ApplicationAccessPolicy.md)

### [Test-OAuthConnectivity](Test-OAuthConnectivity.md)

### [Test-ServicePrincipalAuthorization](Test-ServicePrincipalAuthorization.md)

### [Test-SystemHealth](Test-SystemHealth.md)

### [Update-ExchangeHelp](Update-ExchangeHelp.md)

## policy-and-compliance Cmdlets
### [Disable-JournalArchiving](Disable-JournalArchiving.md)

### [Disable-JournalRule](Disable-JournalRule.md)

### [Disable-OutlookProtectionRule](Disable-OutlookProtectionRule.md)

### [Disable-TransportRule](Disable-TransportRule.md)

### [Enable-JournalRule](Enable-JournalRule.md)

### [Enable-OutlookProtectionRule](Enable-OutlookProtectionRule.md)

### [Enable-TransportRule](Enable-TransportRule.md)

### [Execute-AzureADLabelSync](Execute-AzureADLabelSync.md)

### [Export-JournalRuleCollection](Export-JournalRuleCollection.md)

### [Export-TransportRuleCollection](Export-TransportRuleCollection.md)

### [Get-ActivityAlert](Get-ActivityAlert.md)

### [Get-AdministrativeUnit](Get-AdministrativeUnit.md)

### [Get-AutoSensitivityLabelPolicy](Get-AutoSensitivityLabelPolicy.md)

### [Get-AutoSensitivityLabelRule](Get-AutoSensitivityLabelRule.md)

### [Get-EtrLimits](Get-EtrLimits.md)

### [Get-ExoInformationBarrierPolicy](Get-ExoInformationBarrierPolicy.md)

### [Get-ExoInformationBarrierSegment](Get-ExoInformationBarrierSegment.md)

### [Get-ExoInformationBarrierRelationship](Get-ExoInformationBarrierRelationship.md)

### [Get-InformationBarrierPoliciesApplicationStatus](Get-InformationBarrierPoliciesApplicationStatus.md)

### [Get-InformationBarrierPolicy](Get-InformationBarrierPolicy.md)

### [Get-InformationBarrierRecipientStatus](Get-InformationBarrierRecipientStatus.md)

### [Get-JournalRule](Get-JournalRule.md)

### [Get-Label](Get-Label.md)

### [Get-LabelPolicy](Get-LabelPolicy.md)

### [Get-MessageClassification](Get-MessageClassification.md)

### [Get-OrganizationSegment](Get-OrganizationSegment.md)

### [Get-OutlookProtectionRule](Get-OutlookProtectionRule.md)

### [Get-ProtectionAlert](Get-ProtectionAlert.md)

### [Get-ReviewItems](Get-ReviewItems.md)

### [Get-SupervisoryReviewPolicyV2](Get-SupervisoryReviewPolicyV2.md)

### [Get-SupervisoryReviewRule](Get-SupervisoryReviewRule.md)

### [Get-TransportRule](Get-TransportRule.md)

### [Get-TransportRuleAction](Get-TransportRuleAction.md)

### [Get-TransportRulePredicate](Get-TransportRulePredicate.md)

### [Import-JournalRuleCollection](Import-JournalRuleCollection.md)

### [Import-TransportRuleCollection](Import-TransportRuleCollection.md)

### [Install-UnifiedCompliancePrerequisite](Install-UnifiedCompliancePrerequisite.md)

### [New-ActivityAlert](New-ActivityAlert.md)

### [New-AutoSensitivityLabelPolicy](New-AutoSensitivityLabelPolicy.md)

### [New-AutoSensitivityLabelRule](New-AutoSensitivityLabelRule.md)

### [New-InformationBarrierPolicy](New-InformationBarrierPolicy.md)

### [New-JournalRule](New-JournalRule.md)

### [New-Label](New-Label.md)

### [New-LabelPolicy](New-LabelPolicy.md)

### [New-MessageClassification](New-MessageClassification.md)

### [New-OrganizationSegment](New-OrganizationSegment.md)

### [New-OutlookProtectionRule](New-OutlookProtectionRule.md)

### [New-ProtectionAlert](New-ProtectionAlert.md)

### [New-SupervisoryReviewPolicyV2](New-SupervisoryReviewPolicyV2.md)

### [New-SupervisoryReviewRule](New-SupervisoryReviewRule.md)

### [New-TransportRule](New-TransportRule.md)

### [Remove-ActivityAlert](Remove-ActivityAlert.md)

### [Remove-AutoSensitivityLabelPolicy](Remove-AutoSensitivityLabelPolicy.md)

### [Remove-AutoSensitivityLabelRule](Remove-AutoSensitivityLabelRule.md)

### [Remove-InformationBarrierPolicy](Remove-InformationBarrierPolicy.md)

### [Remove-JournalRule](Remove-JournalRule.md)

### [Remove-Label](Remove-Label.md)

### [Remove-LabelPolicy](Remove-LabelPolicy.md)

### [Remove-MessageClassification](Remove-MessageClassification.md)

### [Remove-OrganizationSegment](Remove-OrganizationSegment.md)

### [Remove-OutlookProtectionRule](Remove-OutlookProtectionRule.md)

### [Remove-ProtectionAlert](Remove-ProtectionAlert.md)

### [Remove-RecordLabel](Remove-RecordLabel.md)

### [Remove-SupervisoryReviewPolicyV2](Remove-SupervisoryReviewPolicyV2.md)

### [Remove-TransportRule](Remove-TransportRule.md)

### [Set-ActivityAlert](Set-ActivityAlert.md)

### [Set-AutoSensitivityLabelPolicy](Set-AutoSensitivityLabelPolicy.md)

### [Set-AutoSensitivityLabelRule](Set-AutoSensitivityLabelRule.md)

### [Set-InformationBarrierPolicy](Set-InformationBarrierPolicy.md)

### [Set-JournalRule](Set-JournalRule.md)

### [Set-Label](Set-Label.md)

### [Set-LabelPolicy](Set-LabelPolicy.md)

### [Set-MessageClassification](Set-MessageClassification.md)

### [Set-OrganizationSegment](Set-OrganizationSegment.md)

### [Set-OutlookProtectionRule](Set-OutlookProtectionRule.md)

### [Set-ProtectionAlert](Set-ProtectionAlert.md)

### [Set-SupervisoryReviewPolicyV2](Set-SupervisoryReviewPolicyV2.md)

### [Set-SupervisoryReviewRule](Set-SupervisoryReviewRule.md)

### [Set-TransportRule](Set-TransportRule.md)

### [Start-InformationBarrierPoliciesApplication](Start-InformationBarrierPoliciesApplication.md)

### [Stop-InformationBarrierPoliciesApplication](Stop-InformationBarrierPoliciesApplication.md)

### [Test-ArchiveConnectivity](Test-ArchiveConnectivity.md)

## policy-and-compliance-audit Cmdlets
### [Get-AdminAuditLogConfig](Get-AdminAuditLogConfig.md)

### [Get-AuditConfig](Get-AuditConfig.md)

### [Get-AuditConfigurationPolicy](Get-AuditConfigurationPolicy.md)

### [Get-AuditConfigurationRule](Get-AuditConfigurationRule.md)

### [Get-AuditLogSearch](Get-AuditLogSearch.md)

### [Get-MailboxAuditBypassAssociation](Get-MailboxAuditBypassAssociation.md)

### [Get-UnifiedAuditLogRetentionPolicy](Get-UnifiedAuditLogRetentionPolicy.md)

### [New-AdminAuditLogSearch](New-AdminAuditLogSearch.md)

### [New-AuditConfigurationPolicy](New-AuditConfigurationPolicy.md)

### [New-AuditConfigurationRule](New-AuditConfigurationRule.md)

### [New-MailboxAuditLogSearch](New-MailboxAuditLogSearch.md)

### [New-UnifiedAuditLogRetentionPolicy](New-UnifiedAuditLogRetentionPolicy.md)

### [Remove-AuditConfigurationPolicy](Remove-AuditConfigurationPolicy.md)

### [Remove-AuditConfigurationRule](Remove-AuditConfigurationRule.md)

### [Remove-UnifiedAuditLogRetentionPolicy](Remove-UnifiedAuditLogRetentionPolicy.md)

### [Search-AdminAuditLog](Search-AdminAuditLog.md)

### [Search-MailboxAuditLog](Search-MailboxAuditLog.md)

### [Search-UnifiedAuditLog](Search-UnifiedAuditLog.md)

### [Set-AdminAuditLogConfig](Set-AdminAuditLogConfig.md)

### [Set-AuditConfig](Set-AuditConfig.md)

### [Set-AuditConfigurationRule](Set-AuditConfigurationRule.md)

### [Set-MailboxAuditBypassAssociation](Set-MailboxAuditBypassAssociation.md)

### [Set-UnifiedAuditLogRetentionPolicy](Set-UnifiedAuditLogRetentionPolicy.md)

### [Write-AdminAuditLog](Write-AdminAuditLog.md)

## policy-and-compliance-content-search Cmdlets
### [Get-ComplianceSearch](Get-ComplianceSearch.md)

### [Get-ComplianceSearchAction](Get-ComplianceSearchAction.md)

### [Get-ComplianceSecurityFilter](Get-ComplianceSecurityFilter.md)

### [Get-MailboxSearch](Get-MailboxSearch.md)

### [Invoke-ComplianceSearchActionStep](Invoke-ComplianceSearchActionStep.md)

### [New-ComplianceSearch](New-ComplianceSearch.md)

### [New-ComplianceSearchAction](New-ComplianceSearchAction.md)

### [New-ComplianceSecurityFilter](New-ComplianceSecurityFilter.md)

### [New-MailboxSearch](New-MailboxSearch.md)

### [Remove-ComplianceSearch](Remove-ComplianceSearch.md)

### [Remove-ComplianceSearchAction](Remove-ComplianceSearchAction.md)

### [Remove-ComplianceSecurityFilter](Remove-ComplianceSecurityFilter.md)

### [Remove-MailboxSearch](Remove-MailboxSearch.md)

### [Set-ComplianceSearch](Set-ComplianceSearch.md)

### [Set-ComplianceSearchAction](Set-ComplianceSearchAction.md)

### [Set-ComplianceSecurityFilter](Set-ComplianceSecurityFilter.md)

### [Set-MailboxSearch](Set-MailboxSearch.md)

### [Start-ComplianceSearch](Start-ComplianceSearch.md)

### [Start-MailboxSearch](Start-MailboxSearch.md)

### [Stop-ComplianceSearch](Stop-ComplianceSearch.md)

### [Stop-MailboxSearch](Stop-MailboxSearch.md)

## policy-and-compliance-dlp Cmdlets
### [Export-ActivityExplorerData](Export-ActivityExplorerData.md)

### [Export-DlpPolicyCollection](Export-DlpPolicyCollection.md)

### [Get-ClassificationRuleCollection](Get-ClassificationRuleCollection.md)

### [Get-DataClassification](Get-DataClassification.md)

### [Get-DataClassificationConfig](Get-DataClassificationConfig.md)

### [Get-DlpCompliancePolicy](Get-DlpCompliancePolicy.md)

### [Get-DlpComplianceRule](Get-DlpComplianceRule.md)

### [Get-DlpDetailReport](Get-DlpDetailReport.md)

### [Get-DlpDetectionsReport](Get-DlpDetectionsReport.md)

### [Get-DlpEdmSchema](Get-DlpEdmSchema.md)

### [Get-DlpIncidentDetailReport](Get-DlpIncidentDetailReport.md)

### [Get-DlpKeywordDictionary](Get-DlpKeywordDictionary.md)

### [Get-DlpPolicy](Get-DlpPolicy.md)

### [Get-DlpPolicyTemplate](Get-DlpPolicyTemplate.md)

### [Get-DlpSensitiveInformationType](Get-DlpSensitiveInformationType.md)

### [Get-DlpSensitiveInformationTypeRulePackage](Get-DlpSensitiveInformationTypeRulePackage.md)

### [Get-DlpSiDetectionsReport](Get-DlpSiDetectionsReport.md)

### [Get-PolicyConfig](Get-PolicyConfig.md)

### [Get-PolicyTipConfig](Get-PolicyTipConfig.md)

### [Import-DlpPolicyCollection](Import-DlpPolicyCollection.md)

### [Import-DlpPolicyTemplate](Import-DlpPolicyTemplate.md)

### [New-ClassificationRuleCollection](New-ClassificationRuleCollection.md)

### [New-DataClassification](New-DataClassification.md)

### [New-DlpCompliancePolicy](New-DlpCompliancePolicy.md)

### [New-DlpComplianceRule](New-DlpComplianceRule.md)

### [New-DlpEdmSchema](New-DlpEdmSchema.md)

### [New-DlpFingerprint](New-DlpFingerprint.md)

### [New-DlpKeywordDictionary](New-DlpKeywordDictionary.md)

### [New-DlpPolicy](New-DlpPolicy.md)

### [New-DlpSensitiveInformationType](New-DlpSensitiveInformationType.md)

### [New-DlpSensitiveInformationTypeRulePackage](New-DlpSensitiveInformationTypeRulePackage.md)

### [New-Fingerprint](New-Fingerprint.md)

### [New-PolicyTipConfig](New-PolicyTipConfig.md)

### [Remove-ClassificationRuleCollection](Remove-ClassificationRuleCollection.md)

### [Remove-DataClassification](Remove-DataClassification.md)

### [Remove-DlpCompliancePolicy](Remove-DlpCompliancePolicy.md)

### [Remove-DlpComplianceRule](Remove-DlpComplianceRule.md)

### [Remove-DlpEdmSchema](Remove-DlpEdmSchema.md)

### [Remove-DlpKeywordDictionary](Remove-DlpKeywordDictionary.md)

### [Remove-DlpPolicy](Remove-DlpPolicy.md)

### [Remove-DlpPolicyTemplate](Remove-DlpPolicyTemplate.md)

### [Remove-DlpSensitiveInformationType](Remove-DlpSensitiveInformationType.md)

### [Remove-DlpSensitiveInformationTypeRulePackage](Remove-DlpSensitiveInformationTypeRulePackage.md)

### [Remove-PolicyTipConfig](Remove-PolicyTipConfig.md)

### [Set-ClassificationRuleCollection](Set-ClassificationRuleCollection.md)

### [Set-DataClassification](Set-DataClassification.md)

### [Set-DlpCompliancePolicy](Set-DlpCompliancePolicy.md)

### [Set-DlpComplianceRule](Set-DlpComplianceRule.md)

### [Set-DlpEdmSchema](Set-DlpEdmSchema.md)

### [Set-DlpKeywordDictionary](Set-DlpKeywordDictionary.md)

### [Set-DlpPolicy](Set-DlpPolicy.md)

### [Set-DlpSensitiveInformationType](Set-DlpSensitiveInformationType.md)

### [Set-DlpSensitiveInformationTypeRulePackage](Set-DlpSensitiveInformationTypeRulePackage.md)

### [Set-PolicyConfig](Set-PolicyConfig.md)

### [Set-PolicyTipConfig](Set-PolicyTipConfig.md)

### [Test-DataClassification](Test-DataClassification.md)

### [Test-TextExtraction](Test-TextExtraction.md)

## policy-and-compliance-ediscovery Cmdlets
### [Add-ComplianceCaseMember](Add-ComplianceCaseMember.md)

### [Add-eDiscoveryCaseAdmin](Add-eDiscoveryCaseAdmin.md)

### [Get-CaseHoldPolicy](Get-CaseHoldPolicy.md)

### [Get-CaseHoldRule](Get-CaseHoldRule.md)

### [Get-ComplianceCase](Get-ComplianceCase.md)

### [Get-ComplianceCaseMember](Get-ComplianceCaseMember.md)

### [Get-eDiscoveryCaseAdmin](Get-eDiscoveryCaseAdmin.md)

### [New-CaseHoldPolicy](New-CaseHoldPolicy.md)

### [New-CaseHoldRule](New-CaseHoldRule.md)

### [New-ComplianceCase](New-ComplianceCase.md)

### [Remove-CaseHoldPolicy](Remove-CaseHoldPolicy.md)

### [Remove-CaseHoldRule](Remove-CaseHoldRule.md)

### [Remove-ComplianceCase](Remove-ComplianceCase.md)

### [Remove-ComplianceCaseMember](Remove-ComplianceCaseMember.md)

### [Remove-eDiscoveryCaseAdmin](Remove-eDiscoveryCaseAdmin.md)

### [Set-CaseHoldPolicy](Set-CaseHoldPolicy.md)

### [Set-CaseHoldRule](Set-CaseHoldRule.md)

### [Set-ComplianceCase](Set-ComplianceCase.md)

### [Update-ComplianceCaseMember](Update-ComplianceCaseMember.md)

### [Update-eDiscoveryCaseAdmin](Update-eDiscoveryCaseAdmin.md)

## policy-and-compliance-retention Cmdlets
### [Enable-ComplianceTagStorage](Enable-ComplianceTagStorage.md)

### [Export-ContentExplorerData](Export-ContentExplorerData.md)

### [Export-FilePlanProperty](Export-FilePlanProperty.md)

### [Get-AdaptiveScope](Get-AdaptiveScope.md)

### [Get-AppRetentionCompliancePolicy](Get-AppRetentionCompliancePolicy.md)

### [Get-AppRetentionComplianceRule](Get-AppRetentionComplianceRule.md)

### [Get-ComplianceRetentionEvent](Get-ComplianceRetentionEvent.md)

### [Get-ComplianceRetentionEventType](Get-ComplianceRetentionEventType.md)

### [Get-ComplianceTag](Get-ComplianceTag.md)

### [Get-ComplianceTagStorage](Get-ComplianceTagStorage.md)

### [Get-DataRetentionReport](Get-DataRetentionReport.md)

### [Get-FilePlanPropertyAuthority](Get-FilePlanPropertyAuthority.md)

### [Get-FilePlanPropertyCategory](Get-FilePlanPropertyCategory.md)

### [Get-FilePlanPropertyCitation](Get-FilePlanPropertyCitation.md)

### [Get-FilePlanPropertyDepartment](Get-FilePlanPropertyDepartment.md)

### [Get-FilePlanPropertyReferenceId](Get-FilePlanPropertyReferenceId.md)

### [Get-FilePlanPropertyStructure](Get-FilePlanPropertyStructur.md)

### [Get-FilePlanPropertySubCategory](Get-FilePlanPropertySubCategory.md)

### [Get-HoldCompliancePolicy](Get-HoldCompliancePolicy.md)

### [Get-HoldComplianceRule](Get-HoldComplianceRule.md)

### [Get-ManagedContentSettings](Get-ManagedContentSettings.md)

### [Get-ManagedFolder](Get-ManagedFolder.md)

### [Get-ManagedFolderMailboxPolicy](Get-ManagedFolderMailboxPolicy.md)

### [Get-RecordReviewNotificationTemplateConfig](Get-RecordReviewNotificationTemplateConfig.md)

### [Get-RegulatoryComplianceUI](Get-RegulatoryComplianceUI.md)

### [Get-RetentionCompliancePolicy](Get-RetentionCompliancePolicy.md)

### [Get-RetentionComplianceRule](Get-RetentionComplianceRule.md)

### [Get-RetentionEvent](Get-RetentionEvent.md)

### [Get-RetentionPolicy](Get-RetentionPolicy.md)

### [Get-RetentionPolicyTag](Get-RetentionPolicyTag.md)

### [Import-FilePlanProperty](Import-FilePlanProperty.md)

### [Invoke-HoldRemovalAction](Invoke-HoldRemovalAction.md)

### [New-AdaptiveScope](New-AdaptiveScope.md)

### [New-AppRetentionCompliancePolicy](New-AppRetentionCompliancePolicy.md)

### [New-AppRetentionComplianceRule](New-AppRetentionComplianceRule.md)

### [New-ComplianceRetentionEvent](New-ComplianceRetentionEvent.md)

### [New-ComplianceRetentionEventType](New-ComplianceRetentionEventType.md)

### [New-ComplianceTag](New-ComplianceTag.md)

### [New-FilePlanPropertyAuthority](New-FilePlanPropertyAuthority.md)

### [New-FilePlanPropertyCategory](New-FilePlanPropertyCategory.md)

### [New-FilePlanPropertyCitation](New-FilePlanPropertyCitation.md)

### [New-FilePlanPropertyDepartment](New-FilePlanPropertyDepartment.md)

### [New-FilePlanPropertyReferenceId](New-FilePlanPropertyReferenceId.md)

### [New-FilePlanPropertySubCategory](New-FilePlanPropertySubCategory.md)

### [New-HoldCompliancePolicy](New-HoldCompliancePolicy.md)

### [New-HoldComplianceRule](New-HoldComplianceRule.md)

### [New-ManagedContentSettings](New-ManagedContentSettings.md)

### [New-ManagedFolder](New-ManagedFolder.md)

### [New-ManagedFolderMailboxPolicy](New-ManagedFolderMailboxPolicy.md)

### [New-RetentionCompliancePolicy](New-RetentionCompliancePolicy.md)

### [New-RetentionComplianceRule](New-RetentionComplianceRule.md)

### [New-RetentionPolicy](New-RetentionPolicy.md)

### [New-RetentionPolicyTag](New-RetentionPolicyTag.md)

### [Remove-AdaptiveScope](Remove-AdaptiveScope.md)

### [Remove-AppRetentionCompliancePolicy](Remove-AppRetentionCompliancePolicy.md)

### [Remove-AppRetentionComplianceRule](Remove-AppRetentionComplianceRule.md)

### [Remove-ComplianceRetentionEventType](Remove-ComplianceRetentionEventType.md)

### [Remove-ComplianceTag](Remove-ComplianceTag.md)

### [Remove-FilePlanPropertyAuthority](Remove-FilePlanPropertyAuthority.md)

### [Remove-FilePlanPropertyCategory](Remove-FilePlanPropertyCategory.md)

### [Remove-FilePlanPropertyCitation](Remove-FilePlanPropertyCitation.md)

### [Remove-FilePlanPropertyDepartment](Remove-FilePlanPropertyDepartment.md)

### [Remove-FilePlanPropertyReferenceId](Remove-FilePlanPropertyReferenceId.md)

### [Remove-FilePlanPropertySubCategory](Remove-FilePlanPropertySubCategor.md)

### [Remove-HoldCompliancePolicy](Remove-HoldCompliancePolicy.md)

### [Remove-HoldComplianceRule](Remove-HoldComplianceRule.md)

### [Remove-ManagedContentSettings](Remove-ManagedContentSettings.md)

### [Remove-ManagedFolder](Remove-ManagedFolder.md)

### [Remove-ManagedFolderMailboxPolicy](Remove-ManagedFolderMailboxPolicy.md)

### [Remove-RetentionCompliancePolicy](Remove-RetentionCompliancePolicy.md)

### [Remove-RetentionComplianceRule](Remove-RetentionComplianceRule.md)

### [Remove-RetentionPolicy](Remove-RetentionPolicy.md)

### [Remove-RetentionPolicyTag](Remove-RetentionPolicyTag.md)

### [Set-AdaptiveScope](Set-AdaptiveScope.md)

### [Set-AppRetentionCompliancePolicy](Set-AppRetentionCompliancePolicy.md)

### [Set-AppRetentionComplianceRule](Set-AppRetentionComplianceRule.md)

### [Set-ComplianceRetentionEventType](Set-ComplianceRetentionEventType.md)

### [Set-ComplianceTag](Set-ComplianceTag.md)

### [Set-FilePlanPropertyAuthority](Set-FilePlanPropertyAuthorit.md)

### [Set-FilePlanPropertyCategory](Set-FilePlanPropertyCategory.md)

### [Set-FilePlanPropertyCitation](Set-FilePlanPropertyCitation.md)

### [Set-FilePlanPropertyDepartment](Set-FilePlanPropertyDepartment.md)

### [Set-FilePlanPropertyReferenceId](Set-FilePlanPropertyReferenceId.md)

### [Set-FilePlanPropertySubCategory](Set-FilePlanPropertySubCategory.md)

### [Set-HoldCompliancePolicy](Set-HoldCompliancePolicy.md)

### [Set-HoldComplianceRule](Set-HoldComplianceRule.md)

### [Set-ManagedContentSettings](Set-ManagedContentSettings.md)

### [Set-ManagedFolder](Set-ManagedFolder.md)

### [Set-ManagedFolderMailboxPolicy](Set-ManagedFolderMailboxPolicy.md)

### [Set-RecordReviewNotificationTemplateConfig](Set-RecordReviewNotificationTemplateConfig.md)

### [Set-RegulatoryComplianceUI](Set-RegulatoryComplianceUI.md)

### [Set-RetentionCompliancePolicy](Set-RetentionCompliancePolicy.md)

### [Set-RetentionComplianceRule](Set-RetentionComplianceRule.md)

### [Set-RetentionPolicy](Set-RetentionPolicy.md)

### [Set-RetentionPolicyTag](Set-RetentionPolicyTag.md)

### [Start-ManagedFolderAssistant](Start-ManagedFolderAssistant.md)

### [Start-RetentionAutoTagLearning](Start-RetentionAutoTagLearning.md)

### [Stop-ManagedFolderAssistant](Stop-ManagedFolderAssistant.md)

### [Validate-RetentionRuleQuery](Validate-RetentionRuleQuery.md)

## powershell-v3-module Cmdlets
### [Add-VivaModuleFeaturePolicy](Add-VivaModuleFeaturePolicy.md)

### [Connect-ExchangeOnline](Connect-ExchangeOnline.md)

### [Connect-IPPSSession](Connect-IPPSSession.md)

### [Disconnect-ExchangeOnline](Disconnect-ExchangeOnline.md)

### [Get-ConnectionInformation](Get-ConnectionInformation.md)

### [Get-DefaultTenantBriefingConfig](Get-DefaultTenantBriefingConfig.md)

### [Get-DefaultTenantMyAnalyticsFeatureConfig](Get-DefaultTenantMyAnalyticsFeatureConfig.md)

### [Get-EXOCasMailbox](Get-EXOCasMailbox.md)

### [Get-EXOMailbox](Get-EXOMailbox.md)

### [Get-EXOMailboxFolderPermission](Get-EXOMailboxFolderPermission.md)

### [Get-EXOMailboxFolderStatistics](Get-EXOMailboxFolderStatistics.md)

### [Get-EXOMailboxPermission](Get-EXOMailboxPermission.md)

### [Get-EXOMailboxStatistics](Get-EXOMailboxStatistics.md)

### [Get-EXOMobileDeviceStatistics](Get-EXOMobileDeviceStatistics.md)

### [Get-EXORecipient](Get-EXORecipient.md)

### [Get-EXORecipientPermission](Get-EXORecipientPermission.md)

### [Get-MyAnalyticsFeatureConfig](Get-MyAnalyticsFeatureConfig.md)

### [Get-UserBriefingConfig](Get-UserBriefingConfig.md)

### [Get-VivaInsightsSettings](Get-VivaInsightsSettings.md)

### [Get-VivaModuleFeature](Get-VivaModuleFeature.md)

### [Get-VivaModuleFeatureEnablement](Get-VivaModuleFeatureEnablement.md)

### [Get-VivaModuleFeaturePolicy](Get-VivaModuleFeaturePolicy.md)

### [Remove-VivaModuleFeaturePolicy](Remove-VivaModuleFeaturePolicy.md)

### [Set-DefaultTenantBriefingConfig](Set-DefaultTenantBriefingConfig.md)

### [Set-DefaultTenantMyAnalyticsFeatureConfig](Set-DefaultTenantMyAnalyticsFeatureConfig.md)

### [Set-MyAnalyticsFeatureConfig](Set-MyAnalyticsFeatureConfig.md)

### [Set-UserBriefingConfig](Set-UserBriefingConfig.md)

### [Set-VivaInsightsSettings](Set-VivaInsightsSettings.md)

### [Update-VivaModuleFeaturePolicy](Update-VivaModuleFeaturePolicy.md)

## reporting Cmdlets
### [Get-CompromisedUserAggregateReport](Get-CompromisedUserAggregateReport.md)

### [Get-CompromisedUserDetailReport](Get-CompromisedUserDetailReport.md)

### [Get-ConnectionByClientTypeDetailReport](Get-ConnectionByClientTypeDetailReport.md)

### [Get-ConnectionByClientTypeReport](Get-ConnectionByClientTypeReport.md)

### [Get-CsActiveUserReport](Get-CsActiveUserReport.md)

### [Get-CsAVConferenceTimeReport](Get-CsAVConferenceTimeReport.md)

### [Get-CsClientDeviceDetailReport](Get-CsClientDeviceDetailReport.md)

### [Get-CsClientDeviceReport](Get-CsClientDeviceReport.md)

### [Get-CsConferenceReport](Get-CsConferenceReport.md)

### [Get-CsP2PAVTimeReport](Get-CsP2PAVTimeReport.md)

### [Get-CsP2PSessionReport](Get-CsP2PSessionReport.md)

### [Get-CsPSTNConferenceTimeReport](Get-CsPSTNConferenceTimeReport.md)

### [Get-CsPSTNUsageDetailReport](Get-CsPSTNUsageDetailReport.md)

### [Get-CsUserActivitiesReport](Get-CsUserActivitiesReport.md)

### [Get-CsUsersBlockedReport](Get-CsUsersBlockedReport.md)

### [Get-GroupActivityReport](Get-GroupActivityReport.md)

### [Get-HistoricalSearch](Get-HistoricalSearch.md)

### [Get-LicenseVsUsageSummaryReport](Get-LicenseVsUsageSummaryReport.md)

### [Get-LogonStatistics](Get-LogonStatistics.md)

### [Get-MailboxActivityReport](Get-MailboxActivityReport.md)

### [Get-MailboxUsageDetailReport](Get-MailboxUsageDetailReport.md)

### [Get-MailboxUsageReport](Get-MailboxUsageReport.md)

### [Get-MailDetailDlpPolicyReport](Get-MailDetailDlpPolicyReport.md)

### [Get-MailDetailEncryptionReport](Get-MailDetailEncryptionReport.md)

### [Get-MailDetailTransportRuleReport](Get-MailDetailTransportRuleReport.md)

### [Get-MailFilterListReport](Get-MailFilterListReport.md)

### [Get-MailflowStatusReport](Get-MailflowStatusReport.md)

### [Get-MailTrafficEncryptionReport](Get-MailTrafficEncryptionReport.md)

### [Get-MailTrafficPolicyReport](Get-MailTrafficPolicyReport.md)

### [Get-MailTrafficSummaryReport](Get-MailTrafficSummaryReport.md)

### [Get-MxRecordReport](Get-MxRecordReport.md)

### [Get-O365ClientBrowserDetailReport](Get-O365ClientBrowserDetailReport.md)

### [Get-O365ClientBrowserReport](Get-O365ClientBrowserReport.md)

### [Get-O365ClientOSDetailReport](Get-O365ClientOSDetailReport.md)

### [Get-O365ClientOSReport](Get-O365ClientOSReport.md)

### [Get-OutboundConnectorReport](Get-OutboundConnectorReport.md)

### [Get-RecipientStatisticsReport](Get-RecipientStatisticsReport.md)

### [Get-ReportExecutionInstance](Get-ReportExecutionInstance.md)

### [Get-SCInsights](Get-SCInsights.md)

### [Get-ServiceDeliveryReport](Get-ServiceDeliveryReport.md)

### [Get-SPOActiveUserReport](Get-SPOActiveUserReport.md)

### [Get-SPOSkyDriveProDeployedReport](Get-SPOSkyDriveProDeployedReport.md)

### [Get-SPOSkyDriveProStorageReport](Get-SPOSkyDriveProStorageReport.md)

### [Get-SPOTeamSiteDeployedReport](Get-SPOTeamSiteDeployedReport.md)

### [Get-SPOTeamSiteStorageReport](Get-SPOTeamSiteStorageReport.md)

### [Get-SPOTenantStorageMetricReport](Get-SPOTenantStorageMetricReport.md)

### [Get-StaleMailboxDetailReport](Get-StaleMailboxDetailReport.md)

### [Get-StaleMailboxReport](Get-StaleMailboxReport.md)

### [Get-SupervisoryReviewActivity](Get-SupervisoryReviewActivity.md)

### [Get-SupervisoryReviewOverallProgressReport](Get-SupervisoryReviewOverallProgressReport.md)

### [Get-SupervisoryReviewPolicyReport](Get-SupervisoryReviewPolicyReport.md)

### [Get-SupervisoryReviewReport](Get-SupervisoryReviewReport.md)

### [Get-SupervisoryReviewTopCasesReport](Get-SupervisoryReviewTopCasesReport.md)

### [Test-Message](Test-Message.md)

## role-based-access-control Cmdlets
### [Add-ManagementRoleEntry](Add-ManagementRoleEntry.md)

### [Add-RoleGroupMember](Add-RoleGroupMember.md)

### [Get-ManagementRole](Get-ManagementRole.md)

### [Get-ManagementRoleAssignment](Get-ManagementRoleAssignment.md)

### [Get-ManagementRoleEntry](Get-ManagementRoleEntry.md)

### [Get-ManagementScope](Get-ManagementScope.md)

### [Get-RoleAssignmentPolicy](Get-RoleAssignmentPolicy.md)

### [Get-RoleGroup](Get-RoleGroup.md)

### [Get-RoleGroupMember](Get-RoleGroupMember.md)

### [New-ManagementRole](New-ManagementRole.md)

### [New-ManagementRoleAssignment](New-ManagementRoleAssignment.md)

### [New-ManagementScope](New-ManagementScope.md)

### [New-RoleAssignmentPolicy](New-RoleAssignmentPolicy.md)

### [New-RoleGroup](New-RoleGroup.md)

### [Remove-ManagementRole](Remove-ManagementRole.md)

### [Remove-ManagementRoleAssignment](Remove-ManagementRoleAssignment.md)

### [Remove-ManagementRoleEntry](Remove-ManagementRoleEntry.md)

### [Remove-ManagementScope](Remove-ManagementScope.md)

### [Remove-RoleAssignmentPolicy](Remove-RoleAssignmentPolicy.md)

### [Remove-RoleGroup](Remove-RoleGroup.md)

### [Remove-RoleGroupMember](Remove-RoleGroupMember.md)

### [Set-ManagementRoleAssignment](Set-ManagementRoleAssignment.md)

### [Set-ManagementRoleEntry](Set-ManagementRoleEntry.md)

### [Set-ManagementScope](Set-ManagementScope.md)

### [Set-RoleAssignmentPolicy](Set-RoleAssignmentPolicy.md)

### [Set-RoleGroup](Set-RoleGroup.md)

### [Update-RoleGroupMember](Update-RoleGroupMember.md)

## server-health-and-performance Cmdlets
### [Add-GlobalMonitoringOverride](Add-GlobalMonitoringOverride.md)

### [Add-ServerMonitoringOverride](Add-ServerMonitoringOverride.md)

### [Get-AvailabilityReportOutage](Get-AvailabilityReportOutage.md)

### [Get-EventLogLevel](Get-EventLogLevel.md)

### [Get-GlobalMonitoringOverride](Get-GlobalMonitoringOverride.md)

### [Get-HealthReport](Get-HealthReport.md)

### [Get-MonitoringItemHelp](Get-MonitoringItemHelp.md)

### [Get-MonitoringItemIdentity](Get-MonitoringItemIdentity.md)

### [Get-ServerComponentState](Get-ServerComponentState.md)

### [Get-ServerHealth](Get-ServerHealth.md)

### [Get-ServerMonitoringOverride](Get-ServerMonitoringOverride.md)

### [Get-ThrottlingPolicy](Get-ThrottlingPolicy.md)

### [Get-ThrottlingPolicyAssociation](Get-ThrottlingPolicyAssociation.md)

### [Invoke-MonitoringProbe](Invoke-MonitoringProbe.md)

### [New-AvailabilityReportOutage](New-AvailabilityReportOutage.md)

### [New-ThrottlingPolicy](New-ThrottlingPolicy.md)

### [Remove-AvailabilityReportOutage](Remove-AvailabilityReportOutage.md)

### [Remove-GlobalMonitoringOverride](Remove-GlobalMonitoringOverride.md)

### [Remove-ServerMonitoringOverride](Remove-ServerMonitoringOverride.md)

### [Remove-ThrottlingPolicy](Remove-ThrottlingPolicy.md)

### [Set-AvailabilityReportOutage](Set-AvailabilityReportOutage.md)

### [Set-EventLogLevel](Set-EventLogLevel.md)

### [Set-ServerComponentState](Set-ServerComponentState.md)

### [Set-ServerMonitor](Set-ServerMonitor.md)

### [Set-ThrottlingPolicy](Set-ThrottlingPolicy.md)

### [Set-ThrottlingPolicyAssociation](Set-ThrottlingPolicyAssociation.md)

### [Test-ServiceHealth](Test-ServiceHealth.md)

## sharing-and-collaboration Cmdlets
### [Add-AvailabilityAddressSpace](Add-AvailabilityAddressSpace.md)

### [Add-PublicFolderAdministrativePermission](Add-PublicFolderAdministrativePermission.md)

### [Add-PublicFolderClientPermission](Add-PublicFolderClientPermission.md)

### [Disable-MailPublicFolder](Disable-MailPublicFolder.md)

### [Enable-MailPublicFolder](Enable-MailPublicFolder.md)

### [Get-AvailabilityAddressSpace](Get-AvailabilityAddressSpace.md)

### [Get-AvailabilityConfig](Get-AvailabilityConfig.md)

### [Get-MailPublicFolder](Get-MailPublicFolder.md)

### [Get-OrganizationRelationship](Get-OrganizationRelationship.md)

### [Get-PublicFolder](Get-PublicFolder.md)

### [Get-PublicFolderAdministrativePermission](Get-PublicFolderAdministrativePermission.md)

### [Get-PublicFolderClientPermission](Get-PublicFolderClientPermission.md)

### [Get-PublicFolderDatabase](Get-PublicFolderDatabase.md)

### [Get-PublicFolderItemStatistics](Get-PublicFolderItemStatistics.md)

### [Get-PublicFolderMailboxDiagnostics](Get-PublicFolderMailboxDiagnostics.md)

### [Get-PublicFolderStatistics](Get-PublicFolderStatistics.md)

### [Get-SharingPolicy](Get-SharingPolicy.md)

### [Get-SiteMailbox](Get-SiteMailbox.md)

### [Get-SiteMailboxDiagnostics](Get-SiteMailboxDiagnostics.md)

### [Get-SiteMailboxProvisioningPolicy](Get-SiteMailboxProvisioningPolicy.md)

### [New-AvailabilityConfig](New-AvailabilityConfig.md)

### [New-OrganizationRelationship](New-OrganizationRelationship.md)

### [New-PublicFolder](New-PublicFolder.md)

### [New-PublicFolderDatabase](New-PublicFolderDatabase.md)

### [New-PublicFolderDatabaseRepairRequest](New-PublicFolderDatabaseRepairRequest.md)

### [New-SharingPolicy](New-SharingPolicy.md)

### [New-SiteMailboxProvisioningPolicy](New-SiteMailboxProvisioningPolicy.md)

### [New-SyncMailPublicFolder](New-SyncMailPublicFolder.md)

### [Remove-AvailabilityAddressSpace](Remove-AvailabilityAddressSpace.md)

### [Remove-AvailabilityConfig](Remove-AvailabilityConfig.md)

### [Remove-OrganizationRelationship](Remove-OrganizationRelationship.md)

### [Remove-PublicFolder](Remove-PublicFolder.md)

### [Remove-PublicFolderAdministrativePermission](Remove-PublicFolderAdministrativePermission.md)

### [Remove-PublicFolderClientPermission](Remove-PublicFolderClientPermission.md)

### [Remove-PublicFolderDatabase](Remove-PublicFolderDatabase.md)

### [Remove-SharingPolicy](Remove-SharingPolicy.md)

### [Remove-SiteMailboxProvisioningPolicy](Remove-SiteMailboxProvisioningPolicy.md)

### [Remove-SyncMailPublicFolder](Remove-SyncMailPublicFolder.md)

### [Resume-PublicFolderReplication](Resume-PublicFolderReplication.md)

### [Set-AvailabilityConfig](Set-AvailabilityConfig.md)

### [Set-MailPublicFolder](Set-MailPublicFolder.md)

### [Set-OrganizationRelationship](Set-OrganizationRelationship.md)

### [Set-PublicFolder](Set-PublicFolder.md)

### [Set-PublicFolderDatabase](Set-PublicFolderDatabase.md)

### [Set-SharingPolicy](Set-SharingPolicy.md)

### [Set-SiteMailbox](Set-SiteMailbox.md)

### [Set-SiteMailboxProvisioningPolicy](Set-SiteMailboxProvisioningPolicy.md)

### [Suspend-PublicFolderReplication](Suspend-PublicFolderReplication.md)

### [Test-OrganizationRelationship](Test-OrganizationRelationship.md)

### [Test-SiteMailbox](Test-SiteMailbox.md)

### [Update-PublicFolder](Update-PublicFolder.md)

### [Update-PublicFolderHierarchy](Update-PublicFolderHierarchy.md)

### [Update-PublicFolderMailbox](Update-PublicFolderMailbox.md)

### [Update-SiteMailbox](Update-SiteMailbox.md)

## unified-messaging Cmdlets
### [Disable-UMAutoAttendant](Disable-UMAutoAttendant.md)

### [Disable-UMCallAnsweringRule](Disable-UMCallAnsweringRule.md)

### [Disable-UMIPGateway](Disable-UMIPGateway.md)

### [Disable-UMMailbox](Disable-UMMailbox.md)

### [Disable-UMServer](Disable-UMServer.md)

### [Disable-UMService](Disable-UMService.md)

### [Enable-UMAutoAttendant](Enable-UMAutoAttendant.md)

### [Enable-UMCallAnsweringRule](Enable-UMCallAnsweringRule.md)

### [Enable-UMIPGateway](Enable-UMIPGateway.md)

### [Enable-UMMailbox](Enable-UMMailbox.md)

### [Enable-UMServer](Enable-UMServer.md)

### [Enable-UMService](Enable-UMService.md)

### [Export-UMCallDataRecord](Export-UMCallDataRecord.md)

### [Export-UMPrompt](Export-UMPrompt.md)

### [Get-OnlineMeetingConfiguration](Get-OnlineMeetingConfiguration.md)

### [Get-UMActiveCalls](Get-UMActiveCalls.md)

### [Get-UMAutoAttendant](Get-UMAutoAttendant.md)

### [Get-UMCallAnsweringRule](Get-UMCallAnsweringRule.md)

### [Get-UMCallDataRecord](Get-UMCallDataRecord.md)

### [Get-UMCallRouterSettings](Get-UMCallRouterSettings.md)

### [Get-UMCallSummaryReport](Get-UMCallSummaryReport.md)

### [Get-UMDialPlan](Get-UMDialPlan.md)

### [Get-UMHuntGroup](Get-UMHuntGroup.md)

### [Get-UMIPGateway](Get-UMIPGateway.md)

### [Get-UMMailbox](Get-UMMailbox.md)

### [Get-UMMailboxPIN](Get-UMMailboxPIN.md)

### [Get-UMMailboxPolicy](Get-UMMailboxPolicy.md)

### [Get-UmServer](Get-UmServer.md)

### [Get-UMService](Get-UMService.md)

### [Import-UMPrompt](Import-UMPrompt.md)

### [New-UMAutoAttendant](New-UMAutoAttendant.md)

### [New-UMCallAnsweringRule](New-UMCallAnsweringRule.md)

### [New-UMDialPlan](New-UMDialPlan.md)

### [New-UMHuntGroup](New-UMHuntGroup.md)

### [New-UMIPGateway](New-UMIPGateway.md)

### [New-UMMailboxPolicy](New-UMMailboxPolicy.md)

### [Remove-UMAutoAttendant](Remove-UMAutoAttendant.md)

### [Remove-UMCallAnsweringRule](Remove-UMCallAnsweringRule.md)

### [Remove-UMDialPlan](Remove-UMDialPlan.md)

### [Remove-UMHuntGroup](Remove-UMHuntGroup.md)

### [Remove-UMIPGateway](Remove-UMIPGateway.md)

### [Remove-UMMailboxPolicy](Remove-UMMailboxPolicy.md)

### [Set-UMAutoAttendant](Set-UMAutoAttendant.md)

### [Set-UMCallAnsweringRule](Set-UMCallAnsweringRule.md)

### [Set-UMCallRouterSettings](Set-UMCallRouterSettings.md)

### [Set-UMDialPlan](Set-UMDialPlan.md)

### [Set-UMIPGateway](Set-UMIPGateway.md)

### [Set-UMMailbox](Set-UMMailbox.md)

### [Set-UMMailboxPIN](Set-UMMailboxPIN.md)

### [Set-UMMailboxPolicy](Set-UMMailboxPolicy.md)

### [Set-UmServer](Set-UmServer.md)

### [Set-UMService](Set-UMService.md)

### [Test-UMConnectivity](Test-UMConnectivity.md)

## users-and-groups Cmdlets
### [Add-DistributionGroupMember](Add-DistributionGroupMember.md)

### [Add-UnifiedGroupLinks](Add-UnifiedGroupLinks.md)

### [Disable-DistributionGroup](Disable-DistributionGroup.md)

### [Disable-MailContact](Disable-MailContact.md)

### [Disable-MailUser](Disable-MailUser.md)

### [Enable-DistributionGroup](Enable-DistributionGroup.md)

### [Enable-MailContact](Enable-MailContact.md)

### [Enable-MailUser](Enable-MailUser.md)

### [Get-Contact](Get-Contact.md)

### [Get-DistributionGroup](Get-DistributionGroup.md)

### [Get-DistributionGroupMember](Get-DistributionGroupMember.md)

### [Get-DynamicDistributionGroup](Get-DynamicDistributionGroup.md)

### [Get-DynamicDistributionGroupMember](Get-DynamicDistributionGroupMember.md)

### [Get-EligibleDistributionGroupForMigration](Get-EligibleDistributionGroupForMigration.md)

### [Get-Group](Get-Group.md)

### [Get-LinkedUser](Get-LinkedUser.md)

### [Get-MailContact](Get-MailContact.md)

### [Get-MailUser](Get-MailUser.md)

### [Get-Recipient](Get-Recipient.md)

### [Get-SecurityPrincipal](Get-SecurityPrincipal.md)

### [Get-UnifiedGroup](Get-UnifiedGroup.md)

### [Get-UnifiedGroupLinks](Get-UnifiedGroupLinks.md)

### [Get-User](Get-User.md)

### [New-DistributionGroup](New-DistributionGroup.md)

### [New-DynamicDistributionGroup](New-DynamicDistributionGroup.md)

### [New-MailContact](New-MailContact.md)

### [New-MailUser](New-MailUser.md)

### [New-UnifiedGroup](New-UnifiedGroup.md)

### [Remove-DistributionGroup](Remove-DistributionGroup.md)

### [Remove-DistributionGroupMember](Remove-DistributionGroupMember.md)

### [Remove-DynamicDistributionGroup](Remove-DynamicDistributionGroup.md)

### [Remove-MailContact](Remove-MailContact.md)

### [Remove-MailUser](Remove-MailUser.md)

### [Remove-UnifiedGroup](Remove-UnifiedGroup.md)

### [Remove-UnifiedGroupLinks](Remove-UnifiedGroupLinks.md)

### [Set-Contact](Set-Contact.md)

### [Set-DistributionGroup](Set-DistributionGroup.md)

### [Set-DynamicDistributionGroup](Set-DynamicDistributionGroup.md)

### [Set-Group](Set-Group.md)

### [Set-LinkedUser](Set-LinkedUser.md)

### [Set-MailContact](Set-MailContact.md)

### [Set-MailUser](Set-MailUser.md)

### [Set-UnifiedGroup](Set-UnifiedGroup.md)

### [Set-User](Set-User.md)

### [Undo-SoftDeletedUnifiedGroup](Undo-SoftDeletedUnifiedGroup.md)

### [Update-DistributionGroupMember](Update-DistributionGroupMember.md)

### [Upgrade-DistributionGroup](Upgrade-DistributionGroup.md)
