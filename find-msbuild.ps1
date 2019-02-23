<#
.SYNOPSIS
Searches the system for the most recent MSBuild version and returns the path to
the MSBuild executable.
#>
$msBuildVersions = New-Object "System.Collections.Generic.SortedDictionary[[double], [string]]"

foreach ($path in Resolve-Path HKLM:\SOFTWARE\Microsoft\MSBuild\ToolsVersions\*) {
    $version = - [double](Split-Path $path -Leaf)
    $property = Get-ItemProperty -Path $path -Name "MSBuildToolsPath"
    $msBuildVersions[$version] = $property.MSBuildToolsPath
}

foreach ($msBuildToolsPath in $msBuildVersions.Values) {
    $fullPath = Join-Path -Path $msBuildToolsPath -ChildPath "msbuild.exe"
    if (Test-Path -Path $fullPath) {
        return $fullPath
    }
}
