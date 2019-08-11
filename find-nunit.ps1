<#
.SYNOPSIS
Searches the packages directory for the NUnit console runner version specified
in PmlUnit.Tests\packages.config
#>

[xml]$config = Get-Content .\PmlUnit.Tests\packages.config

foreach ($package in $config.packages.package) {
    if ($package.id -ieq "NUnit.ConsoleRunner") {
        $directoryName = "packages\NUnit.ConsoleRunner." + $package.version
        $path = Join-Path -Path $directoryName -ChildPath tools\nunit3-console.exe -Resolve
        if (Test-Path $path) {
            return $path
        }
    }
}
