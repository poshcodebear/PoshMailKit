# Config
$sourceFolder = "$($PSScriptRoot)\.."
$conf = Get-Content -Path "$($PSScriptRoot)\BuildConfig.json" | ConvertFrom-Json

$modFolder = "$($sourceFolder)\$($conf.Name)"
$modManifest = "$($modFolder)\$($conf.Name).psd1"

# Module population
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

# Manifest templating
$prTag = $conf.PreReleaseTag ? "Prerelease = '$($conf.PreReleaseTag)'" : "# Prerelease = ''"

$manifest = Get-Content -Path $modManifest -Raw
$manifest = $manifest.Replace('<#$Version#>', $conf.Version).
                      Replace('<#$PreRelease#>', $prTag).
                      Replace('<#$Date#>', (Get-Date -Format 'yyyy-MM-dd')).
                      Replace('<#$Year#>', (Get-Date -Format 'yyyy')).
                      Replace('<#$FileList#>', "'$($conf.FileList -join "', '")'")

$manifest | Out-File -FilePath $modManifest -Force

# Module archive (zip) generation
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
