---
external help file: dnslookup.dll-Help.xml
Module Name: DnsClient
ms.date: 05/20/2019
online version: https://learn.microsoft.com/powershell/module/dnsclient/resolve-dnsname?view=windowsserver2016-ps&wt.mc_id=ps-gethelp
schema: 2.0.0
title: Resolve-DnsName
---

# Resolve-DnsName

## SYNOPSIS
Performs a DNS name query resolution for the specified name.

## SYNTAX

```
Resolve-DnsName [-Name] <String> [[-Type] <RecordType>] [-Server <String[]>] [-DnsOnly] [-CacheOnly]
 [-DnssecOk] [-DnssecCd] [-NoHostsFile] [-LlmnrNetbiosOnly] [-LlmnrFallback] [-LlmnrOnly] [-NetbiosFallback]
 [-NoIdn] [-NoRecursion] [-QuickTimeout] [-TcpOnly] [<CommonParameters>]
```

## DESCRIPTION
The **Resolve-DnsName** cmdlet performs a DNS query for the specified name.
This cmdlet is functionally similar to the nslookup tool which allows users to query for names.

## EXAMPLES

### EXAMPLE 1
```
PS C:\> Resolve-DnsName -Name www.bing.com
```

This example resolves a name using the default options.

### EXAMPLE 2
```
PS C:\> Resolve-DnsName -Name www.bing.com -Server 10.0.0.1
```

This example resolves a name against the DNS server at 10.0.0.1.

### EXAMPLE 3
```
PS C:\> Resolve-DnsName -Name www.bing.com -Type A
```

This example queries for A type records for name www.bing.com.

### EXAMPLE 4
```
PS C:\> Resolve-DnsName -Name www.bing.com -DnsOnly
```

This example resolves a name using only DNS.
LLMNR and NetBIOS queries are not issued.

## PARAMETERS

### -CacheOnly
Resolves this query using only the local cache.

```yaml
Type: SwitchParameter
Parameter Sets: (All)
Aliases: 

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -DnsOnly
Resolves this query using only the DNS protocol.

```yaml
Type: SwitchParameter
Parameter Sets: (All)
Aliases: 

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -DnssecCd
Sets the DNSSEC checking-disabled bit for this query.

```yaml
Type: SwitchParameter
Parameter Sets: (All)
Aliases: 

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -DnssecOk
Sets the DNSSEC OK bit for this query.

```yaml
Type: SwitchParameter
Parameter Sets: (All)
Aliases: 

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -LlmnrFallback
Allows falling back to the LLMNR protocol when resolving this query with DNS fails.

```yaml
Type: SwitchParameter
Parameter Sets: (All)
Aliases: 

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -LlmnrNetbiosOnly
Resolves this query using only the LLMNR or NetBIOS protocols.

```yaml
Type: SwitchParameter
Parameter Sets: (All)
Aliases: 

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -LlmnrOnly
Resolves this query using only the LLMNR protocol.

```yaml
Type: SwitchParameter
Parameter Sets: (All)
Aliases: 

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -Name
Specifies the name to be resolved.

```yaml
Type: String
Parameter Sets: (All)
Aliases: 

Required: True
Position: 0
Default value: None
Accept pipeline input: True (ByPropertyName, ByValue)
Accept wildcard characters: False
```

### -NetbiosFallback
Allows fallback to the NetBIOS protocol when resolving this query with DNS fails.

```yaml
Type: SwitchParameter
Parameter Sets: (All)
Aliases: 

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -NoHostsFile
Skips the hosts file when resolving this query.

```yaml
Type: SwitchParameter
Parameter Sets: (All)
Aliases: 

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -NoIdn
Specifies not to use IDN encoding logic for the query.

```yaml
Type: SwitchParameter
Parameter Sets: (All)
Aliases: 

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -NoRecursion
Instructs the server not to use recursion when resolving this query.

```yaml
Type: SwitchParameter
Parameter Sets: (All)
Aliases: 

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -QuickTimeout
Uses shorter timeouts for this query.

```yaml
Type: SwitchParameter
Parameter Sets: (All)
Aliases: 

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -Server
Specifies the IP addresses or host names of the DNS servers to be queried.
By default the interface DNS servers are queried if this parameter is not supplied.

```yaml
Type: String[]
Parameter Sets: (All)
Aliases: 

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -TcpOnly
Uses only TCP for this query.

```yaml
Type: SwitchParameter
Parameter Sets: (All)
Aliases: 

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -Type
Specifies the DNS query type that is to be issued.
By default the type is A_AAAA, the A and AAAA types will both be queried.
The acceptable values for this parameter are:

 -- UNKNOWN = 0, 
                         
 -- A_AAAA = 0, the DNS query type is A_AAAA. 
                         
 -- A = 1, the DNS query type is IPv4 server Address. 
                         
 -- AAAA = 28, the DNS query type is IPv6 server address. 
                         
 -- NS = 2, the DNS query type is name server. 
                         
 -- MX = 15, the DNS query type is mail routing information. 
                         
 -- MD = 3, the DNS query type is mail destination. 
                         
 -- MF = 4, the DNS query type is mail forwarder. 
                         
 -- CNAME = 5, the DNS query type is canonical name. 
                         
 -- SOA = 6, the DNS query type is start of authority zone. 
                         
 -- MB = 7, the DNS query type is mailbox domain name. 
                         
 -- MG = 8, the DNS query type is mail group member. 
                         
 -- MR = 9, the DNS query type is mail rename name. 
                         
 -- NULL = 10, the DNS query type is null resource record. 
                         
 -- WKS = 11, the DNS query type is well known service. 
                         
 -- PTR = 12, the DNS query type is domain name pointer. 
                         
 -- HINFO = 13, the DNS query type is host information. 
                         
 -- MINFO = 14, the DNS query type is mailbox information. 
                         
 -- TXT = 16, the DNS query type is text strings. 
                         
 -- RP = 17, the DNS query type is responsible person. 
                         
 -- AFSDB = 18, the DNS query type is AFS database servers. 
                         
 -- X25 = 19, the DNS query type is packet switched wide area network. 
                         
 -- ISDN = 20, the DNS query type is Integrated Services Digital Network. 
                         
 -- RT = 21, the DNS query type is DNS route through. 
                         
 -- SRV = 33, the DNS query type is server selection. 
                         
 -- DNAME = 39, the DNS query type is domain aliases. 
                         
 -- OPT = 41, the DNS query type is DNS option. 
                         
 -- DS = 43, the DNS query type is delegation signer. 
                         
 -- RRSIG = 46, the DNS query type is DNSSEC signature. 
                         
 -- NSEC = 47, the DNS query type is next-secure record. 
                         
 -- DNSKEY = 48, the DNS query type is DNS key record. 
                         
 -- DHCID = 49, the DNS query type is Dynamic Host Configuration Protocol information. 
                         
 -- NSEC3 = 50, the DNS query type is NSEC record version 3. 
                         
 -- NSEC3PARAM = 51, the DNS query type is NSEC3 parameters. 
                         
 -- ANY = 255, the DNS query type is wildcard match. 
                         
 -- ALL = 255, the DNS query type is wildcard match.

```yaml
Type: RecordType
Parameter Sets: (All)
Aliases: 
Accepted values: UNKNOWN, A_AAAA, A, NS, MD, MF, CNAME, SOA, MB, MG, MR, NULL, WKS, PTR, HINFO, MINFO, MX, TXT, RP, AFSDB, X25, ISDN, RT, AAAA, SRV, DNAME, OPT, DS, RRSIG, NSEC, DNSKEY, DHCID, NSEC3, NSEC3PARAM, ANY, ALL, WINS

Required: False
Position: 1
Default value: None
Accept pipeline input: True (ByPropertyName)
Accept wildcard characters: False
```

### CommonParameters
This cmdlet supports the common parameters: -Debug, -ErrorAction, -ErrorVariable, -InformationAction, -InformationVariable, -OutVariable, -OutBuffer, -PipelineVariable, -Verbose, -WarningAction, and -WarningVariable. For more information, see about_CommonParameters (http://go.microsoft.com/fwlink/?LinkID=113216).

## INPUTS

### None

## OUTPUTS

### Microsoft.DnsClient.Commands.DnsRecord
The DnsRecord object contains all of the records returned from the wire for the specified DNS query.

## NOTES

## RELATED LINKS

[Nslookup on TechNet](https://go.microsoft.com/fwlink/p/?LinkId=84907)

