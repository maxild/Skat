##########################################################################
# This is the Cake bootstrapper script for PowerShell.
# This file was downloaded from https://github.com/cake-build/resources
# Feel free to change this file to fit your needs.
##########################################################################

<#

.SYNOPSIS
This is a Powershell script to bootstrap a Cake build.

.DESCRIPTION
This Powershell script will download NuGet if missing, restore NuGet tools (including Cake)
and execute your Cake build script with the parameters you provide.

.PARAMETER Script
The build script to execute.
.PARAMETER Target
The build script target to run.
.PARAMETER Configuration
The build configuration to use.
.PARAMETER Verbosity
Specifies the amount of information to be displayed.
.PARAMETER Experimental
Tells Cake to use the latest Roslyn release.
.PARAMETER WhatIf
Performs a dry run of the build script.
No tasks will be executed.
.PARAMETER Mono
Tells Cake to use the Mono scripting engine.
.PARAMETER SkipToolPackageRestore
Skips restoring of packages.
.PARAMETER ScriptArgs
Remaining arguments are added here.

.LINK
http://cakebuild.net

#>

[CmdletBinding()]
Param(
    [string]$Script = "build.cake",
    [string]$Target = "Default",
    [string]$Configuration = "Release",
    [ValidateSet("Quiet", "Minimal", "Normal", "Verbose", "Diagnostic")]
    [string]$Verbosity = "Verbose",
    [switch]$Experimental,
    [Alias("DryRun","Noop")]
    [switch]$WhatIf,
    [switch]$Mono,
    [switch]$SkipToolPackageRestore,
    [Parameter(Position=0,Mandatory=$false,ValueFromRemainingArguments=$true)]
    [string[]]$ScriptArgs
)

Write-Host "Preparing to run build script..."

$PSScriptRoot = split-path -parent $MyInvocation.MyCommand.Definition;

# Tree
$BUILD_DIR       = Join-Path $PSScriptRoot "build" # build scripts, maybe rename
$TOOLS_DIR       = Join-Path $PSScriptRoot ".tools"
$NUGET_EXE       = Join-Path $TOOLS_DIR "nuget.exe"
$CAKE_EXE        = Join-Path $TOOLS_DIR "Cake/Cake.exe"
$PACKAGES_CONFIG = Join-Path $BUILD_DIR "packages.config" # containing Cake dependency

# Aliases for the entire powershell session
Set-Alias nuget $NUGET_EXE -scope global
Set-Alias cake  $CAKE_EXE -scope global

# Should we use mono?
$UseMono = "";
if($Mono.IsPresent) {
    Write-Verbose -Message "Using the Mono based scripting engine."
    $UseMono = "-mono"
}

# Should we use the new Roslyn?
$UseExperimental = "";
if($Experimental.IsPresent -and (-not ($Mono.IsPresent))) {
    Write-Verbose -Message "Using experimental version of Roslyn."
    $UseExperimental = "-experimental"
}

# Is this a dry run?
$UseDryRun = "";
if($WhatIf.IsPresent) {
    $UseDryRun = "-dryrun"
}

# Make sure .tools folder exists
if ((Test-Path $PSScriptRoot) -and (-not (Test-Path $TOOLS_DIR))) {
    Write-Verbose -Message "Creating tools directory..."
    New-Item -Path $TOOLS_DIR -Type directory | out-null
}

# Download NuGet if it does not exist.
if (-not (Test-Path $NUGET_EXE)) {
    Write-Verbose -Message "Downloading NuGet.exe..."
    try {
        Invoke-WebRequest "https://dist.nuget.org/win-x86-commandline/latest/nuget.exe" -OutFile $NUGET_EXE
    } catch {
        Throw "Could not download NuGet.exe."
    }
}

# Install tools (i.e. Cake) using NuGet
if(-not $SkipToolPackageRestore.IsPresent) {
    Push-Location
    Set-Location $TOOLS_DIR
    Write-Verbose -Message "Restoring tools from NuGet..."
    $NuGetOutput = &nuget install $PACKAGES_CONFIG -ExcludeVersion -OutputDirectory `"$TOOLS_DIR`"
    if ($LASTEXITCODE -ne 0) {
        Throw "An error occured while restoring NuGet tools."
    }
    Write-Verbose -Message ($NuGetOutput | out-string)
    Pop-Location
}

# Make sure that Cake has been installed.
if (-not (Test-Path $CAKE_EXE)) {
    Throw "Could not find Cake.exe at $CAKE_EXE"
}

# Start Cake
Write-Host "Running build script..."
# C# v6 features (e.g. string interpolation) are not supported without '-experimental' flag
#   See https://github.com/cake-build/cake/issues/293
#   See https://github.com/cake-build/cake/issues/326
&cake $Script -experimental -target="$Target" -configuration="$Configuration" -verbosity="$Verbosity" $UseMono $UseDryRun $UseExperimental $ScriptArgs
exit $LASTEXITCODE
