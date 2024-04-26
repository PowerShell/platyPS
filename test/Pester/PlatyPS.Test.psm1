# Copyright (c) Microsoft Corporation.
# Licensed under the MIT License.


$modRoot = (Get-Module Microsoft.PowerShell.PlatyPS).ModuleBase
$depRoot = Join-Path $modRoot "Dependencies"
$markDigAsm = Join-Path $depRoot "Markdig.Signed.dll"
$yamlDotNetAsm = Join-Path $depRoot "YamlDotNet.dll"
# debugging
#Get-ChildItem -Recurse $modRoot -File | Foreach-Object { $_ | Write-Verbose -Verbose }
#$null = import-Module $markDigAsm -ErrorAction SilentlyContinue
#$null = import-Module $yamlDotNetAsm -ErrorAction SilentlyContinue
Add-Type -Assembly $markDigAsm
Add-Type -Assembly $yamlDotNetAsm

class inputOutput {
    [string]$name
    [string]$description
}

class parameterSet {
    [string]$name
    [string]$position
    [bool]$isRequired
    [bool]$valueByPipeline
    [bool]$valueByPipelineByPropertyName
    [bool]$valueFromRemainingArguments
}

class parameter {
    [string]$name
    [string]$type
    [string]$description
    [string[]]$parameterValue
    [string]$defaultValue
    [bool]$variableLength
    [bool]$globbing
    [string]$aliases
    [bool]$dontShow
    [string[]]$acceptedValues
    [string]$helpMessage
    [parameterSet[]]$parameterSets
}

class example {
    [string]$title
    [string]$description
    [string]$summary
}

class link {
    [string]$href
    [string]$text

}
class ch {
    [System.Collections.Specialized.OrderedDictionary]$metadata
    [Globalization.CultureInfo]$Locale
    [Guid]$ModuleGuid
    [string]$ExternalHelpfile
    [string]$OnlineVersionUrl
    [string]$SchemaVersion
    [string]$ModuleName
    [string]$title
    [bool]$HasCmdletBinding
    [bool]$HasWorkflowCommonParameters
    [string]$synopsis
    [System.Collections.Generic.List[string]]$syntaxes
    [string]$aliases
    [string]$description
    [System.Collections.Generic.list[example]]$examples
    [System.Collections.Generic.list[parameter]]$parameters
    [System.Collections.Generic.list[inputOutput]]$inputs
    [System.Collections.Generic.list[inputOutput]]$outputs
    [string]$notes
    [System.Collections.Generic.list[link]]$links

    ch() {
        $this.metadata = [System.Collections.Specialized.OrderedDictionary]::new()
        $this.Syntaxes = [System.Collections.Generic.List[string]]::new()
        $this.examples = [System.Collections.Generic.List[example]]::new()
        $this.parameters = [System.Collections.Generic.List[parameter]]::new()
        $this.inputs = [System.Collections.Generic.List[inputOutput]]::new()
        $this.inputs = [System.Collections.Generic.List[inputOutput]]::new()
        $this.links = [System.Collections.Generic.List[link]]::new()
    }

    [string]ToString() {
        return $this.title
    }
}

# we need to call the generic deserialize method, so we need to build it.
$builderType = "YamlDotNet.Serialization.DeserializerBuilder" -as [type]
$script:yamldes = $builderType::new().WithNamingConvention([YamlDotnet.Serialization.NamingConventions.CamelCaseNamingConvention]::new()).Build()
[type[]]$tlist = @( [string] )

function Import-CommandYaml  {
    [CmdletBinding()]
    param (
        [System.Management.Automation.ParameterAttribute(ValueFromPipelineByPropertyName=$true,ValueFromPipeline=$true,Position=0,Mandatory=$true)]
        [string[]]$fullname,
        [switch]$PreserveOrder
    )

    BEGIN {
        if ($PreserveOrder) {
            $script:YamlDeserializeMethod = $yamldes.GetType().GetMethod("Deserialize", $tlist).MakeGenericMethod([System.Collections.Specialized.OrderedDictionary])
        }
        else {
            $script:YamlDeserializeMethod = $yamldes.GetType().GetMethod("Deserialize", $tlist).MakeGenericMethod([hashtable])
        }
    }

    PROCESS {
        foreach($yamlFile in $fullname) {
            try {
                $yaml = Get-Content -raw $yamlFile
                $result = $YamlDeserializeMethod.Invoke($yamldes, $yaml)
                # fix up some of the object elements
                $null = $result.parameters.where({$_.name -eq "CommonParameters"}).Foreach({$result.parameters.Remove($_); $result.HasCmdletBinding = $true})
                $result.Locale = $result['metadata']['Locale'] ?? "en-US"
                $result.ExternalHelpfile = $result['metadata']['external help file']
                $result
            }
            catch {
                Write-Warning "Failed to parse $yamlFile ($_)"
            }

        }
    }
}

function ConvertTo-CommandYaml {
    [CmdletBinding()]
    param (
        [Parameter(Mandatory=$true,ValueFromPipeline=$true,ValueFromPipelineByPropertyName=$true)]
        [ch]$ch
    )

    PROCESS {
        $cHelp = [Microsoft.PowerShell.PlatyPS.Model.CommandHelp]::new($ch.Metatdata['title'], $ch.Metadata['Module Name'], $ch.Metadata['Locale'])
        $cHelp.Metadata = $ch.Metadata
        $cHelp.Description = $ch.Description
        $cHelp.ExternalHelpFile = $ch.ExternalHelpfile
        $cHelp.HasCmdletBinding = $ch.HasCmdletBinding
        $cHelp.HasWorkflowCommonParameters = $ch.HasWorkflowCommonParameters
        $cHelp.Nodes = $ch.Notes
        $cHelp.OnlineVersionUrl = $ch.OnlineVersionUrl
        $cHelp.SchemaVersion = $ch.SchemaVersion
        $cHelp.Synopsis = $ch.Synopsis
        $cHelp.ModuleGuid = $ch.ModuleGuid

        foreach ($ex in $ch.Examples) {
            $example = [Microsoft.PowerShell.PlatyPS.Model.Example]::new($ex.title, $ex.description)
            $example.Summary = $ex.summary
            $cHelp.Examples.Add($example)
        }

        $cHelp.Aliases = $ch.Aliases?.split(",")
        $cHelp
    }
}
