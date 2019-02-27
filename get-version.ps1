<#
.SYNOPSIS
Searches for an AssemblyInformationalVersion or an AssemblyFileVersion attribute
in the project's AssemblyInfo.cs file and returns its contents.
#>

Param(
    [parameter(Mandatory=$true, Position=0)]
    [String]
    $ProjectPath
)

$assemblyInfoPath = [System.IO.Path]::Combine($ProjectPath, "Properties", "AssemblyInfo.cs")

$fileVersion = "";
foreach ($line in Get-Content $assemblyInfoPath) {
    if ($line -match "AssemblyInformationalVersion\(""([^""]*)""\)") {
        return $matches[1]
    } elseif ($line -match "AssemblyFileVersion\(""([^""]*)""\)") {
        $fileVersion = $matches[1];
    }
}

if ($fileVersion) {
    return $fileVersion
}