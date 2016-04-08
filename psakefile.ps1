Framework 4.6.1

properties {
    $base_dir = resolve-path .
    $build_dir = join-path $base_dir "build"
    $artifacts_dir = join-path $base_dir "artifacts"
    $source_dir = join-path $base_dir "src"
    $nuget_packages_dir = join-path $env:UserProfile '.nuget' | join-path -ChildPath 'packages' # Nuget v3.x uses shared cache
    $sln_file = "Skat.sln"
    $configuration = "debug"
    $framework_dir = Get-FrameworkDirectory
    $tools_version = "14.0" # MSBuild 14 == vs2015 == C#6
}

task default -depends dev
task dev -depends compile, test -description "developer build (before commits)"
task full -depends dev, pack -description "full build (producing nupkg's)"

task resolveVersions -depends clearGitVersionCache {

    $logPath = [System.IO.Path]::GetTempFileName()
    $output = & gitversion /output json /l $logPath
    if ($LASTEXITCODE -ne 0) {
        Write-Host "GitVersion Log:"
        Get-Content $logPath | % { Write-Output $_ }
        Write-Error "$output"
        throw "GitVersion Exit Code: $LASTEXITCODE"
    }

    $versionInfoJson = $output -join "`n" # gitVersion /output json returns System.Object[] type
    $versionInfo = $versionInfoJson | ConvertFrom-Json

    $global:buildVersion = "$($versionInfo.FullSemVer).local.$(Get-BuildNumber)"
    # feature and pull request branches does not need padded PreReleaseNumber (0001)
    if ($versionInfo.PreReleaseTag.StartsWith('a.')) {
        # What happens with two identical feature branches (can myget packages be overriden without errors?)
        $global:pkgVersion = "$($versionInfo.MajorMinorPatch)-$(Format-PrereleaseTag $versionInfo.PreReleaseLabel)"
    }
    else {
        $global:pkgVersion = $versionInfo.NuGetVersion
    }
    $global:assemblyVersion = "$($versionInfo.Major).$($versionInfo.Minor)"
    $global:assemblyFileVersion = "$($versionInfo.MajorMinorPatch).$($versionInfo.CommitsSinceVersionSource)"
    $global:assemblyInformationalVersion = "$buildVersion+$($versionInfo.FullBuildMetaData)"

    # Update appveyor build details
    if ($env:APPVEYOR -ne $NULL) {
        # put the build number in the build metadata. i.e 1.0.0+146.build.{appveyor_build_number}
        # this way appveyor doesn't generate duplicate version numbers (-Version must be unique)
        $global:buildVersion = "$($versionInfo.FullSemVer).build.$($env:APPVEYOR_BUILD_NUMBER)"
        Update-AppveyorBuild -Version $buildVersion
    }
}

# See https://github.com/GitTools/GitVersion/issues/798
task clearGitVersionCache {
    $pathToGitVersionCache = join-path $base_dir '.git' | join-path -ChildPath 'gitversion_cache'
    if (Test-Path -PathType Container -Path $pathToGitVersionCache) {
        remove-item $pathToGitVersionCache -recurse -force
    }
}

task showVersion -depends clearGitVersionCache, resolveVersions {
    Show-Configuration
}

task verifyTools {
    # Visual Studio 2015 will exclusively use 2015 MSBuild and C# compilers (assembly version 14.0)
    # and the 2015 Toolset (ToolsVersion 14.0)
    #$version = &"$framework_dir\MSBuild.exe" /nologo /version
    $version = &{msbuild /nologo /version}
    $expectedVersion = "14.0.24723.2"
    Write-Host "Framework directory (GAC) is $framework_dir"
    Write-Host "MSBuild version is $version"
    Assert $version.StartsWith($tools_version) "MSBuild has version '$version'. It should be '$tools_version'."
    Assert ($version -eq $expectedVersion) "MSBuild has version '$version'. It should be '$expectedVersion'."
}

task clean -depends  clearGitVersionCache {
    delete_directory $artifacts_dir
}

task restore {
    exec {
        # always update nuget to latest release
        & $base_dir\.nuget\Nuget.exe update -self
        & $base_dir\.nuget\Nuget.exe restore $sln_file
    }
}

task compile -depends clean, restore, commonAssemblyInfo {

    $outdir = $artifacts_dir
    if (-not ($outdir.EndsWith("\"))) {
      $outdir += '\' # MSBuild requires OutDir to end with a trailing slash
    }

    Show-Configuration
    exec { msbuild /t:Clean /t:Build /p:OutDir=$outdir /p:Configuration=$configuration /v:minimal /tv:${tools_version} /p:VisualStudioVersion=${tools_version} /maxcpucount "$sln_file" }

    # TODO: Use SourceLink.exe
    # if ($commit -ne "0000000000000000000000000000000000000000") {
    #     exec { &"$tools_dir\GitLink.Custom.exe" "$base_dir" /u https://github.com/maxild/Lofus /c $configuration /b master /s "$commit" /f "$sln_file_name" }
    # }
}

task test -depends compile {

    $test_prjs = @( `
        "$artifacts_dir\Maxfire.Skat.UnitTests.dll" `
        )

    $xunitConsoleRunner = join-path $nuget_packages_dir 'xunit.runner.console' | `
                          join-path -ChildPath '2.1.0' |
                          join-path -ChildPath 'tools' | `
                          join-path -ChildPath 'xunit.console.exe'

    $failures = @()
    $test_prjs | % {
        Write-Host "Executing tests from '$_'" -ForegroundColor Yellow
        & $xunitConsoleRunner $_
        if ($lastexitcode -ne 0) {
            $failures += (Split-Path -Leaf $_)
        }
    }
    if ($failures) {
        throw "Test failure!!! --- $failures"
    }
}

task commonAssemblyInfo -depends resolveVersions {
    $date = Get-Date
    "using System;
using System.Reflection;
using System.Runtime.InteropServices;

//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.4927
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

[assembly: AssemblyCompany(""Maxfire"")]
[assembly: AssemblyCopyright(""Copyright Maxfire 2012-" + $date.Year + ". All rights reserved."")]
[assembly: AssemblyTrademark("""")]
[assembly: ComVisible(false)]

[assembly: AssemblyVersion(""$assemblyVersion"")]
[assembly: AssemblyFileVersion(""$assemblyFileVersion"")]
[assembly: AssemblyInformationalVersion(""$assemblyInformationalVersion"")]
[assembly: AssemblyProduct(""Skat"")]

[assembly: CLSCompliant(true)]

#if DEBUG
[assembly: AssemblyConfiguration(""Debug"")]
#else
[assembly: AssemblyConfiguration(""Release"")]
#endif
" | out-file $(join-path $source_dir 'CommonAssemblyInfo.cs') -encoding "utf8"
}

task pack -depends compile {

    # Find dependency versions
    $maxfireCoreVersion = find-dependencyVersion (join-path (join-path $source_dir "Maxfire.Skat") "project.json") 'Maxfire.Core'

    # Could use the -Version option of the nuget.exe pack command to provide the actual version.
    # _but_ the package dependency version cannot be overriden at the commandline.
    $packages = Get-ChildItem $build_dir *.nuspec -recurse
    $packages | %{
        $nuspec = [xml](Get-Content $_.FullName)
        $nuspec.package.metadata.version = $global:pkgVersion
        $nuspec | Select-Xml '//dependency' | %{
            if ($_.Node.id -eq 'Maxfire.Core') {
                $_.Node.version = $maxfireCoreVersion
            }
        }
        $nuspecFilename = join-path $artifacts_dir (Split-Path -Path $_.FullName -Leaf)
        $nuspec.Save($nuspecFilename)
        exec { & $base_dir\.nuget\Nuget.exe pack -OutputDirectory $artifacts_dir $nuspecFilename }
    }
}

# Using project.json (i.e. not packages.config!!!!)
function find-dependencyVersion($projectJsonPath, $packageId) {

    $projectJson = Get-Content -Path $projectJsonPath -Raw | ConvertFrom-Json

    $dependencyVersion = $projectJson.dependencies | Select-Object -ExpandProperty $packageId

    # Note: We leave this more verbose get-dependency-by-packageId here to document psobject property
    # $projectJson.dependencies.psobject.properties | %{
    #     $pkgId = $_.name
    #     $pkgVersion = $_.value
    #     if ($pkgId -eq $packageId) {
    #         $dependencyVersion = $pkgVersion
    #     }
    # }

    if (-not $dependencyVersion) {
        throw "Could not resolve the version of the $packageId dependency."
    }

    return $dependencyVersion.ToString()
}

function Show-Configuration {
    Write-Host -NoNewline "Build version is '" -ForegroundColor Yellow
    Write-Host -NoNewline "$global:buildVersion" -ForegroundColor DarkGreen
    Write-Host -NoNewline "' with '" -ForegroundColor Yellow
    Write-Host -NoNewline "$configuration" -ForegroundColor DarkGreen
    Write-Host "' configuration has resulted in versions defined by" -ForegroundColor Yellow

    Write-Host -NoNewline "  Version: " -ForegroundColor Yellow
    Write-Host "$global:pkgVersion" -ForegroundColor DarkGreen

    Write-Host -NoNewline "  AssemblyVersion: " -ForegroundColor Yellow
    Write-Host "$global:assemblyVersion" -ForegroundColor DarkGreen

    Write-Host -NoNewline "  AssemblyFileVersion: " -ForegroundColor Yellow
    Write-Host "$global:assemblyFileVersion" -ForegroundColor DarkGreen

    Write-Host -NoNewline "  AssemblyInformationalVersion: " -ForegroundColor Yellow
    Write-Host "$global:assemblyInformationalVersion" -ForegroundColor DarkGreen
}

# -------------------------------------------------------------------------------------------------------------
# generalized functions
# --------------------------------------------------------------------------------------------------------------
function Format-PrereleaseTag([string]$PreReleaseTag) {
    # The string MUST be comprised of only alphanumerics plus dash [0-9A-Za-z-].
    $tag = $PreReleaseTag.Replace(".", "-");

    # The string cannot exceeed 20 chars
    if ($tag.Length -gt 20) {
        return $tag.Substring(0, 20);
    }

    return $tag;
}

function Format-BuildNumber([int]$BuildNumber) {
    if ($BuildNumber -gt 99999) {
        Throw "Build number cannot be greater than 99999, because of Legacy SemVer limitations in Nuget."
    }
    '{0:D5}' -f $BuildNumber # Can handle 00001,...,99999 (this should be enough)
}

# Local builds will generate a build number based on the 'duration' since semantic version date
Function Get-BuildNumber() {
    $SemanticVersionDate = '2015-11-30'
    [int](((Get-Date) - (Get-Date $SemanticVersionDate)).TotalMinutes / 5)
}

function Get-File-Exists-On-Path([string]$file)
{
    $results = ($env:Path).Split(";") | Get-ChildItem -filter $file -erroraction silentlycontinue
    $found = ($results -ne $null)
    return $found
}

# partial sha with 7 chars (a3497c9)
function Get-Git-Commit
{
    if ((Get-File-Exists-On-Path "git.exe")){
        $gitLog = git log --oneline -1
        return $gitLog.Split(' ')[0]
    }
    else {
        return "0000000"
    }
}

# full sha with 40 chars (a3497c9f044f45b5e295f7fb9d7494df3c209a31)
function Get-Git-Commit-Full
{
    if ((Get-File-Exists-On-Path "git.exe")){
        $gitLog = git log -1 --format=%H
        return $gitLog;
    }
    else {
        return "0000000000000000000000000000000000000000"
    }
}

# directory where MSBuild.exe is to be found
function Get-FrameworkDirectory()
{
    $frameworkPath = "$env:windir\Microsoft.NET\Framework\v4.0*"
    $frameworkPathDir = ls "$frameworkPath"
    if ( $frameworkPathDir -eq $null ) {
        throw "Building Brf.Lofus.Core requires .NET 4.0, which doesn't appear to be installed on this machine"
    }
    $net4Version = $frameworkPathDir.Name
    return "$env:windir\Microsoft.NET\Framework\$net4Version"
}

function global:delete_directory($directory_name)
{
  rd $directory_name -recurse -force  -ErrorAction SilentlyContinue | out-null
}

function global:delete_file($file)
{
    if($file) {
        remove-item $file  -force  -ErrorAction SilentlyContinue | out-null
    }
}

function global:create_directory($directory_name)
{
  mkdir $directory_name  -ErrorAction SilentlyContinue  | out-null
}

function global:copy_files($source, $destination, $exclude = @()) {
    create_directory $destination
    Get-ChildItem $source -Recurse -Exclude $exclude |
        Copy-Item -Destination {Join-Path $destination $_.FullName.Substring($source.length)}
}
