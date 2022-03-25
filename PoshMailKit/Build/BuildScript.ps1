$module = 'PoshMailKit'
$sourceFolder = "$($PSScriptRoot)\.."
$modFolder = "$($sourceFolder)\$($module)"

$conf = Get-Content -Path "$($PSScriptRoot)\BuildConfig.json" | ConvertFrom-Json

if (Test-Path -Path $modFolder)
{
    Remove-Item -Path $modFolder -Force -Recurse
}
New-Item -Path $modFolder -ItemType Directory

foreach ($file in $conf.FileList)
{
    Copy-Item -Path "$($sourceFolder)\$($file)" -Destination "$($modFolder)"
}

Copy-Item -Path "$($sourceFolder)\ModuleMembers\*" -Destination $modFolder -Recurse
