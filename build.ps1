[CmdletBinding()]
param (
    [ValidateSet("debug", "release")]
    [Alias('config')]
    [string]$Configuration = 'debug',
    [ValidateSet("Release", `
                 "alpha", `
                 "beta1", "beta2", "beta3", "beta4", "beta5", `
                 "rc1", "rc2", "rc3", "rc4", "rc5", `
                 "local")]
    [Alias('label')]
    [string]$PrereleaseTag = 'local',
    [ValidateRange(1,99999)]
    [Alias('build')]
    [int]$BuildNumber,
    [string]$CommitId = "0000000000000000000000000000000000000000",
    [switch]$SkipRestore,
    [switch]$CleanCache,
    [switch]$SkipTests
)

$RepoRoot = $PSScriptRoot
$NuGetExe = Join-Path $RepoRoot '.nuget\nuget.exe'

function Install-NuGet {
    [CmdletBinding()]
    param([string] $NugetVersion = "latest")
    if (-not (Test-Path $NuGetExe)) {
        Write-Host -ForegroundColor Cyan 'Downloading nuget.exe'
        $NuGetDir = Join-Path $RepoRoot '.nuget'
        if (-not (Test-Path $NuGetDir)) {
            New-Item -ItemType directory -Path $NuGetDir
        }
        Invoke-WebRequest "https://dist.nuget.org/win-x86-commandline/$NugetVersion/nuget.exe" -OutFile $NuGetExe
    }
}

function Install-PSake {
    if (-not (Test-Path (Join-Path $RepoRoot 'packages\psake'))) {
        & $NuGetExe install psake -version 4.5.0 -ExcludeVersion -o packages -nocache
    }
}

function Format-BuildNumber([int]$BuildNumber) {
    if ($BuildNumber -gt 99999) {
        Throw "Build number cannot be greater than 99999, because of Legacy SemVer limitations in Nuget."
    }
    '{0:D5}' -f $BuildNumber # Can handle 00001,...,99999 (this should be enough)
}

function Create-BuildEnvironment {
    if ($PrereleaseTag -ne 'Release') {
        $paddedBuildNumber = Format-BuildNumber $BuildNumber
        $env:DNX_BUILD_VERSION="${PrereleaseTag}-${paddedBuildNumber}"
    }
    $env:DNX_ASSEMBLY_FILE_VERSION=$BuildNumber
}

Install-NuGet
Install-PSake

Create-BuildEnvironment

# right now it is hardcoded to full task
.\packages\psake\tools\psake.ps1 .\psakefile.ps1 full -properties @{configuration=$Configuration}

# report success or failure
if ($psake.build_success -eq $false) { exit 1 } else { exit 0 }
