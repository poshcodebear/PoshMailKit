$sourceFolder = "$($PSScriptRoot)\.."
$conf = Get-Content -Path "$($PSScriptRoot)\BuildConfig.json" | ConvertFrom-Json

$modFolder = "$($sourceFolder)\$($conf.Name)"

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

$archiveFilename = "$($conf.Name)-$($conf.Version)"
if ($conf.PreReleaseTag)
{
    $archiveFilename += "-$($conf.PreReleaseTag)"
}

$compressArchiveSplat = @{
    Path              = "$($modFolder)\*"
    DestinationPath   = "$($sourceFolder)\$($archiveFilename).zip"
    CompressionLevel  = 'Optimal'
    Force             = $true
}

Compress-Archive @compressArchiveSplat
