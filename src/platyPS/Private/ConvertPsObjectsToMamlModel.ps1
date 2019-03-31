<#
    This function converts help and command object (possibly mocked) into a Maml Model
#>
Function ConvertPsObjectsToMamlModel 
{

    [CmdletBinding()]
    [OutputType([Markdown.MAML.Model.MAML.MamlCommand])]
    param(
        [Parameter(Mandatory = $true)]
        [object]$Command,
        [Parameter(Mandatory = $true)]
        [object]$Help,
        [switch]$UseHelpForParametersMetadata,
        [switch]$UsePlaceholderForSynopsis,
        [switch]$UseFullTypeName,
        [switch]$ExcludeDontShow
    )

    function isCommonParameterName
    {
        param([string]$parameterName, [switch]$Workflow)

        if (@(
                'Verbose',
                'Debug',
                'ErrorAction',
                'WarningAction',
                'InformationAction',
                'ErrorVariable',
                'WarningVariable',
                'InformationVariable',
                'OutVariable',
                'OutBuffer',
                'PipelineVariable'
            ) -contains $parameterName)
        {
            return $true
        }

        if ($Workflow)
        {
            return @(
                'PSParameterCollection',
                'PSComputerName',
                'PSCredential',
                'PSConnectionRetryCount',
                'PSConnectionRetryIntervalSec',
                'PSRunningTimeoutSec',
                'PSElapsedTimeoutSec',
                'PSPersist',
                'PSAuthentication',
                'PSAuthenticationLevel',
                'PSApplicationName',
                'PSPort',
                'PSUseSSL',
                'PSConfigurationName',
                'PSConnectionURI',
                'PSAllowRedirection',
                'PSSessionOption',
                'PSCertificateThumbprint',
                'PSPrivateMetadata',
                'AsJob',
                'JobName'
            ) -contains $parameterName
        }

        return $false
    }

    function getPipelineValue($Parameter)
    {
        if ($Parameter.ValueFromPipeline)
        {
            if ($Parameter.ValueFromPipelineByPropertyName)
            {
                return 'True (ByPropertyName, ByValue)'
            }
            else
            {
                return 'True (ByValue)'
            }
        }
        else
        {
            if ($Parameter.ValueFromPipelineByPropertyName)
            {
                return 'True (ByPropertyName)'
            }
            else
            {
                return 'False'
            }
        }
    }

    function normalizeFirstLatter
    {
        param(
            [Parameter(ValueFromPipeline = $true)]
            [string]$value
        )

        if ($value -and $value.Length -gt 0)
        {
            return $value.Substring(0, 1).ToUpperInvariant() + $value.substring(1)
        }

        return $value
    }

    #endregion

    $MamlCommandObject = New-Object -TypeName Markdown.MAML.Model.MAML.MamlCommand

    #region Command Object Values Processing

    $IsWorkflow = $Command.CommandType -eq 'Workflow'

    #Get Name
    $MamlCommandObject.Name = $Command.Name

    $MamlCommandObject.ModuleName = $Command.ModuleName

    #region Data not provided by the command object

    #Get Description
    #Not provided by the command object.
    $MamlCommandObject.Description = New-Object -TypeName Markdown.MAML.Model.Markdown.SectionBody ($LocalizedData.Description)

    #endregion

    #Get Syntax
    #region Get the Syntax Parameter Set objects

    function FillUpParameterFromHelp
    {
        param(
            [Parameter(Mandatory = $true)]
            [Markdown.MAML.Model.MAML.MamlParameter]$ParameterObject
        )

        $HelpEntry = $Help.parameters.parameter | Where-Object { $_.Name -eq $ParameterObject.Name }

        $ParameterObject.DefaultValue = $HelpEntry.defaultValue | normalizeFirstLatter
        $ParameterObject.VariableLength = $HelpEntry.variableLength -eq 'True'
        $ParameterObject.Globbing = $HelpEntry.globbing -eq 'True'
        $ParameterObject.Position = $HelpEntry.position | normalizeFirstLatter
        if ($HelpEntry.description)
        {
            if ($HelpEntry.description.text)
            {
                $ParameterObject.Description = $HelpEntry.description |
                DescriptionToPara |
                AddLineBreaksForParagraphs
            }
            else
            {
                # this case happens, when there is HelpMessage in 'Parameter' attribute,
                # but there is no maml or comment-based help.
                # then help engine put string outside of 'text' property
                # In this case there is no DescriptionToPara call needed
                $ParameterObject.Description = $HelpEntry.description | AddLineBreaksForParagraphs
            }
        }

        $syntaxParam = $Help.syntax.syntaxItem.parameter | Where-Object { $_.Name -eq $Parameter.Name } | Select-Object -First 1
        if ($syntaxParam)
        {
            # otherwise we could potentialy get it from Reflection but not doing it for now
            foreach ($parameterValue in $syntaxParam.parameterValueGroup.parameterValue)
            {
                $ParameterObject.parameterValueGroup.Add($parameterValue)
            }
        }
    }

    function FillUpSyntaxFromCommand
    {
        foreach ($ParameterSet in $Command.ParameterSets)
        {
            $SyntaxObject = New-Object -TypeName Markdown.MAML.Model.MAML.MamlSyntax

            $SyntaxObject.ParameterSetName = $ParameterSet.Name
            $SyntaxObject.IsDefault = $ParameterSet.IsDefault

            foreach ($Parameter in $ParameterSet.Parameters)
            {
                # ignore CommonParameters
                if (isCommonParameterName $Parameter.Name -Workflow:$IsWorkflow)
                {
                    # but don't ignore them, if they have explicit help entries
                    if ($Help.parameters.parameter | Where-Object { $_.Name -eq $Parameter.Name })
                    {
                    }
                    else
                    {
                        continue
                    }
                }

                if ($ExcludeDontShow)
                {
                    $hasDontShow = $false
                    foreach ($Attribute in $Parameter.Attributes)
                    {
                        if ($Attribute.TypeId.ToString() -eq 'System.Management.Automation.ParameterAttribute' -and $Attribute.DontShow)
                        {
                            $hasDontShow = $true
                        }
                    }

                    if ($hasDontShow)
                    {
                        continue
                    }
                }

                $ParameterObject = New-Object -TypeName Markdown.MAML.Model.MAML.MamlParameter
                $ParameterObject.Name = $Parameter.Name
                $ParameterObject.Required = $Parameter.IsMandatory
                $ParameterObject.PipelineInput = getPipelineValue $Parameter
                # the ParameterType could be just a string in case of remoting
                # or a TypeInfo object, in the regular case
                if ($Session)
                {
                    # in case of remoting we already pre-calcuated the Type string
                    $ParameterObject.Type = $Parameter.ParameterTypeName
                }
                else
                {
                    $ParameterObject.Type = GetTypeString -TypeObject $Parameter.ParameterType
                }
                # ToString() works in both cases
                $ParameterObject.FullType = $Parameter.ParameterType.ToString()

                $ParameterObject.ValueRequired = -not ($Parameter.Type -eq "SwitchParameter") # thisDefinition is a heuristic

                foreach ($Alias in $Parameter.Aliases)
                {
                    $ParameterObject.Aliases += $Alias
                }

                $ParameterObject.Description = if ([String]::IsNullOrEmpty($Parameter.HelpMessage))
                {
                    # additional new-lines are needed for Update-MarkdownHelp scenario.
                    switch ($Parameter.Name)
                    {
                        # we have well-known parameters and can generate a reasonable description for them
                        # https://github.com/PowerShell/platyPS/issues/211
                        'Confirm' { $LocalizedData.Confirm + "`r`n`r`n" }
                        'WhatIf' { $LocalizedData.WhatIf + "`r`n`r`n" }
                        'IncludeTotalCount' { $LocalizedData.IncludeTotalCount + "`r`n`r`n" }
                        'Skip' { $LocalizedData.Skip + "`r`n`r`n" }
                        'First' { $LocalizedData.First + "`r`n`r`n" }
                        default { ($LocalizedData.ParameterDescription -f $Parameter.Name) + "`r`n`r`n" }
                    }
                }
                else
                {
                    $Parameter.HelpMessage
                }

                FillUpParameterFromHelp $ParameterObject

                $SyntaxObject.Parameters.Add($ParameterObject)
            }

            $MamlCommandObject.Syntax.Add($SyntaxObject)
        }
    }

    function FillUpSyntaxFromHelp
    {
        function GuessTheType
        {
            param([string]$type)

            if (-not $type)
            {
                # weired, but that's how it works
                return 'SwitchParameter'
            }

            return $type
        }

        $ParamSetCount = 0
        foreach ($ParameterSet in $Help.syntax.syntaxItem)
        {
            $SyntaxObject = New-Object -TypeName Markdown.MAML.Model.MAML.MamlSyntax

            $ParamSetCount++
            $SyntaxObject.ParameterSetName = $script:SET_NAME_PLACEHOLDER + "_" + $ParamSetCount

            foreach ($Parameter in $ParameterSet.Parameter)
            {
                $ParameterObject = New-Object -TypeName Markdown.MAML.Model.MAML.MamlParameter

                $ParameterObject.Type = GuessTheType $Parameter.parameterValue

                $ParameterObject.Name = $Parameter.Name
                $ParameterObject.Required = $Parameter.required -eq 'true'
                $ParameterObject.PipelineInput = $Parameter.pipelineInput | normalizeFirstLatter

                $ParameterObject.ValueRequired = -not ($ParameterObject.Type -eq "SwitchParameter") # thisDefinition is a heuristic

                if ($parameter.Aliases -ne 'None')
                {
                    $ParameterObject.Aliases = $parameter.Aliases
                }

                FillUpParameterFromHelp $ParameterObject

                $SyntaxObject.Parameters.Add($ParameterObject)
            }

            $MamlCommandObject.Syntax.Add($SyntaxObject)
        }
    }

    if ($UseHelpForParametersMetadata)
    {
        FillUpSyntaxFromHelp
    }
    else
    {
        FillUpSyntaxFromCommand
    }

    #endregion
    ##########

    #####GET THE HELP-Object Content and add it to the MAML Object#####
    #region Help-Object processing

    #Get Synopsis
    if ($UsePlaceholderForSynopsis)
    {
        # Help object ALWAYS contains SYNOPSIS.
        # If it's not available, it's auto-generated.
        # We don't want to include auto-generated SYNOPSIS (see https://github.com/PowerShell/platyPS/issues/110)
        $MamlCommandObject.Synopsis = New-Object -TypeName Markdown.MAML.Model.Markdown.SectionBody ($LocalizedData.Synopsis)
    }
    else
    {
        $MamlCommandObject.Synopsis = New-Object -TypeName Markdown.MAML.Model.Markdown.SectionBody (
            # $Help.Synopsis only contains the first paragraph
            # https://github.com/PowerShell/platyPS/issues/328
            $Help.details.description |
            DescriptionToPara |
            AddLineBreaksForParagraphs
        )
    }

    #Get Description
    if ($Help.description -ne $null)
    {
        $MamlCommandObject.Description = New-Object -TypeName Markdown.MAML.Model.Markdown.SectionBody (
            $Help.description |
            DescriptionToPara |
            AddLineBreaksForParagraphs
        )
    }

    #Add to Notes
    #From the Help AlertSet data
    if ($help.alertSet)
    {
        $MamlCommandObject.Notes = New-Object -TypeName Markdown.MAML.Model.Markdown.SectionBody (
            $help.alertSet.alert |
            DescriptionToPara |
            AddLineBreaksForParagraphs
        )
    }

    # Not provided by the command object. Using the Command Type to create a note declaring it's type.
    # We can add this placeholder


    #Add to relatedLinks
    if ($help.relatedLinks)
    {
        foreach ($link in $Help.relatedLinks.navigationLink)
        {
            $mamlLink = New-Object -TypeName Markdown.MAML.Model.MAML.MamlLink
            $mamlLink.LinkName = $link.linkText
            $mamlLink.LinkUri = $link.uri
            $MamlCommandObject.Links.Add($mamlLink)
        }
    }

    #Add Examples
    foreach ($Example in $Help.examples.example)
    {
        $MamlExampleObject = New-Object -TypeName Markdown.MAML.Model.MAML.MamlExample

        $MamlExampleObject.Introduction = $Example.introduction
        $MamlExampleObject.Title = $Example.title
        $MamlExampleObject.Code = @(
            New-Object -TypeName Markdown.MAML.Model.MAML.MamlCodeBlock ($Example.code, '')
        )

        $RemarkText = $Example.remarks |
        DescriptionToPara |
        AddLineBreaksForParagraphs

        $MamlExampleObject.Remarks = $RemarkText
        $MamlCommandObject.Examples.Add($MamlExampleObject)
    }

    #Get Inputs
    #Reccomend adding a Parameter Name and Parameter Set Name to each input object.
    #region Inputs
    $Inputs = @()

    $Help.inputTypes.inputType | ForEach-Object {
        $InputDescription = $_.description
        $inputtypes = $_.type.name
        if ($_.description -eq $null -and $_.type.name -ne $null)
        {
            $inputtypes = $_.type.name.split("`n", [System.StringSplitOptions]::RemoveEmptyEntries)
        }

        $inputtypes | ForEach-Object {
            $InputObject = New-Object -TypeName Markdown.MAML.Model.MAML.MamlInputOutput
            $InputObject.TypeName = $_
            $InputObject.Description = $InputDescription |
            DescriptionToPara |
            AddLineBreaksForParagraphs
            $Inputs += $InputObject
        }
    }

    foreach ($Input in $Inputs) { $MamlCommandObject.Inputs.Add($Input) }

    #endregion

    #Get Outputs
    #No Output Type description is provided from the command object.
    #region Outputs
    $Outputs = @()

    $Help.returnValues.returnValue | ForEach-Object {
        $OuputDescription = $_.description
        $Outputtypes = $_.type.name
        if ($_.description -eq $null -and $_.type.name -ne $null)
        {
            $Outputtypes = $_.type.name.split("`n", [System.StringSplitOptions]::RemoveEmptyEntries)
        }

        $Outputtypes | ForEach-Object {
            $OutputObject = New-Object -TypeName Markdown.MAML.Model.MAML.MamlInputOutput
            $OutputObject.TypeName = $_
            $OutputObject.Description = $OuputDescription |
            DescriptionToPara |
            AddLineBreaksForParagraphs
            $Outputs += $OutputObject
        }
    }

    foreach ($Output in $Outputs) { $MamlCommandObject.Outputs.Add($Output) }
    #endregion
    ##########

    #####Adding Parameters Section from Syntax block#####
    #region Parameter Unique Selection from Parameter Sets
    #This will only work when the Parameters member has a public set as well as a get.

    function Get-ParameterByName
    {
        param(
            [string]$Name
        )

        $defaultSyntax = $MamlCommandObject.Syntax | Where-Object { $Command.DefaultParameterSet -eq $_.ParameterSetName }
        # default syntax should have a priority
        $syntaxes = @($defaultSyntax) + $MamlCommandObject.Syntax

        foreach ($s in $syntaxes)
        {
            $param = $s.Parameters | Where-Object { $_.Name -eq $Name }
            if ($param)
            {
                return $param
            }
        }
    }

    function Get-ParameterNamesOrder()
    {
        # we want to keep original order for existing help
        # if something changed:
        #   - remove it from it's position
        #   - add to the end

        $helpNames = $Help.parameters.parameter.Name
        if (-not $helpNames) { $helpNames = @() }

        # sort-object unique does case-insensiteve unification
        $realNames = $MamlCommandObject.Syntax.Parameters.Name | Sort-Object -Unique
        if (-not $realNames) { $realNames = @() }

        $realNamesList = New-Object 'System.Collections.Generic.List[string]'
        $realNamesList.AddRange( ( [string[]] $realNames) )

        foreach ($name in $helpNames)
        {
            if ($realNamesList.Remove($name))
            {
                # yeild
                $name
            }
            # Otherwise it didn't exist
        }

        foreach ($name in $realNamesList)
        {
            # yeild
            $name
        }

    }

    foreach ($ParameterName in (Get-ParameterNamesOrder))
    {
        $Parameter = Get-ParameterByName $ParameterName
        if ($Parameter)
        {
            if ($UseFullTypeName)
            {
                $Parameter = $Parameter.Clone()
                $Parameter.Type = $Parameter.FullType
            }
            $MamlCommandObject.Parameters.Add($Parameter)
        }
        else
        {
            Write-Warning -Message ($LocalizedData.ParameterNotFound -f '[Markdown generation]', $ParameterName, $Command.Name)
        }
    }

    # Handle CommonParameters, default for MamlCommand is SupportCommonParameters = $true
    if ($Command.CmdletBinding -eq $false)
    {
        # Remove CommonParameters by exception
        $MamlCommandObject.SupportCommonParameters = $false
    }

    # Handle CommonWorkflowParameters
    $MamlCommandObject.IsWorkflow = $IsWorkflow

    #endregion
    ##########

    return $MamlCommandObject

}
