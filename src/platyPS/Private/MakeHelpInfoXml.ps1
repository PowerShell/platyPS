Function MakeHelpInfoXml 
{

    Param(
        [Parameter(mandatory = $true)]
        [string]
        $ModuleName,
        [Parameter(mandatory = $true)]
        [string]
        $GUID,
        [Parameter(mandatory = $true)]
        [string]
        $HelpCulture,
        [Parameter(mandatory = $true)]
        [string]
        $HelpVersion,
        [Parameter(mandatory = $true)]
        [string]
        $URI,
        [Parameter(mandatory = $true)]
        [string]
        $OutputFolder


    )

    $HelpInfoFileNme = $ModuleName + "_" + $GUID + "_HelpInfo.xml"
    $OutputFullPath = Join-Path $OutputFolder $HelpInfoFileNme

    if (Test-Path $OutputFullPath -PathType Leaf)
    {
        [xml] $HelpInfoContent = Get-Content $OutputFullPath
    }

    #Create the base XML object for the Helpinfo.xml file.
    $xml = New-Object xml

    $ns = "http://schemas.microsoft.com/powershell/help/2010/05"
    $declaration = $xml.CreateXmlDeclaration("1.0", "utf-8", $null)

    $rootNode = $xml.CreateElement("HelpInfo", $ns)
    $xml.InsertBefore($declaration, $xml.DocumentElement)
    $xml.AppendChild($rootNode)

    $HelpContentUriNode = $xml.CreateElement("HelpContentURI", $ns)
    $HelpContentUriNode.InnerText = $URI
    $xml["HelpInfo"].AppendChild($HelpContentUriNode)

    $HelpSupportedCulturesNode = $xml.CreateElement("SupportedUICultures", $ns)
    $xml["HelpInfo"].AppendChild($HelpSupportedCulturesNode)


    #If no previous help file
    if (-not $HelpInfoContent)
    {
        $HelpUICultureNode = $xml.CreateElement("UICulture", $ns)
        $xml["HelpInfo"]["SupportedUICultures"].AppendChild($HelpUICultureNode)

        $HelpUICultureNameNode = $xml.CreateElement("UICultureName", $ns)
        $HelpUICultureNameNode.InnerText = $HelpCulture
        $xml["HelpInfo"]["SupportedUICultures"]["UICulture"].AppendChild($HelpUICultureNameNode)

        $HelpUICultureVersionNode = $xml.CreateElement("UICultureVersion", $ns)
        $HelpUICultureVersionNode.InnerText = $HelpVersion
        $xml["HelpInfo"]["SupportedUICultures"]["UICulture"].AppendChild($HelpUICultureVersionNode)

        [xml] $HelpInfoContent = $xml

    }
    else
    {
        #Get old culture info
        $ExistingCultures = @{ }
        foreach ($Culture in $HelpInfoContent.HelpInfo.SupportedUICultures.UICulture)
        {
            $ExistingCultures.Add($Culture.UICultureName, $Culture.UICultureVersion)
        }

        #If culture exists update version, if not, add culture and version
        if (-not ($HelpCulture -in $ExistingCultures.Keys))
        {
            $ExistingCultures.Add($HelpCulture, $HelpVersion)
        }
        else
        {
            $ExistingCultures[$HelpCulture] = $HelpVersion
        }

        $cultureNames = @()
        $cultureNames += $ExistingCultures.GetEnumerator()

        #write out cultures to XML
        for ($i = 0; $i -lt $ExistingCultures.Count; $i++)
        {
            $HelpUICultureNode = $xml.CreateElement("UICulture", $ns)


            $HelpUICultureNameNode = $xml.CreateElement("UICultureName", $ns)
            $HelpUICultureNameNode.InnerText = $cultureNames[$i].Name
            $HelpUICultureNode.AppendChild($HelpUICultureNameNode)

            $HelpUICultureVersionNode = $xml.CreateElement("UICultureVersion", $ns)
            $HelpUICultureVersionNode.InnerText = $cultureNames[$i].Value
            $HelpUICultureNode.AppendChild($HelpUICultureVersionNode)

            $xml["HelpInfo"]["SupportedUICultures"].AppendChild($HelpUICultureNode)
        }

        [xml] $HelpInfoContent = $xml
    }

    #Commit Help
    if (!(Test-Path $OutputFullPath))
    {
        New-Item -Path $OutputFolder -ItemType File -Name $HelpInfoFileNme

    }

    $HelpInfoContent.Save((Get-ChildItem $OutputFullPath).FullName)


}
