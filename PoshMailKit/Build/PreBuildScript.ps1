# Config
$sourceFolder = "$($PSScriptRoot)\.."
$conf = Get-Content -Path "$($PSScriptRoot)\BuildConfig.json" | ConvertFrom-Json

$projectFile = "$($sourceFolder)\PoshMailKit.csproj"

# Project file version update
$prTag = $conf.PreReleaseTag ? "-$($conf.PreReleaseTag)" : ''

$projectContents = Get-Content -Path $projectFile -Raw

$projectContents = $projectContents -replace '<VersionPrefix>[^<]*</VersionPrefix>', "<VersionPrefix>$($conf.Version)</VersionPrefix>"
$projectContents = $projectContents -replace '<VersionSuffix>[^<]*</VersionSuffix>', "<VersionSuffix>$($prTag)</VersionSuffix>"

$projectContents = $projectContents.TrimEnd("`r`n")

$projectContents | Out-File -FilePath $projectFile -Force
