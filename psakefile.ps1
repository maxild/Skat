# Note that using the value 4.5.1 also works for projects that target 4.5.2
# (the value 4.5.2 is not recognised by psake)
# This will make msbuild point to one of:
#    [ProgramFilesX86]\MSBuild\12.0\bin\amd64 (x64 bitness of powershell)
#    [ProgramFilesX86]\MSBuild\12.0\bin (x32 bitness of powershell)
# See also http://blogs.msdn.com/b/visualstudio/archive/2013/07/24/msbuild-is-now-part-of-visual-studio.aspx
# See also https://github.com/damianh/psake/commit/0778be759e83014f0bfdd1a0c84caac008c8112f
Framework 4.5.1

properties {
    $base_dir = resolve-path .
    $build_dir = join-path $base_dir "build"
    $artifacts_dir = join-path $base_dir "artifacts"
    $source_dir = join-path $base_dir "src"
    $nuget_packages_dir = join-path $env:UserProfile '.nuget' | join-path -ChildPath 'packages' # Nuget v3.x uses shared cache
    $sln_file = "Skat.sln"
    $configuration = "debug"
    $global:version = "1.0.0-*"
    $framework_dir = Get-FrameworkDirectory
    $tools_version = "14.0" # MSBuild 14 == vs2015 == C#6
}

task default -depends dev
task dev -depends compile, test -description "developer build (before commits)"
#task full -depends dev, pack -description "full build (producing nupkg's)"

# For project.json as { "version": "1.0.0-*", ...}, together with label='alpha'
# and build=12345, the end result is something equivalent to (DNX_BUILD_VERSION=alpha-12345, DNX_ASSEMBLY_FILE_VERSION=12345)
#  [assembly: AssemblyVersion("1.0.0.0")]
#  [assembly: AssemblyFileVersion("1.0.0.12345")]
#  [assembly: AssemblyInformationalVersion("1.0.0-alpha-12345")]
#
# Before building (in build.ps1) do something like
#      $env:DNX_BUILD_VERSION="${PrereleaseTag}-${paddedBuildNumber}"
#      $env:DNX_ASSEMBLY_FILE_VERSION=$buildNumber
task resolveVersions {
    if ($global:version.EndsWith('-*')) {
        $global:assemblyVersion = $global:version.Substring(0, $global:version.Length - 2)
        if ($env:DNX_BUILD_VERSION -ne $NULL) {
            $global:pkgVersion = $global:version.Substring(0, $global:version.Length - 1) + $env:DNX_BUILD_VERSION
        }
        else {
            $paddedBuildNumber = Format-BuildNumber $(Get-BuildNumber)
            $global:pkgVersion = $global:assemblyVersion + "-local-$paddedBuildNumber"
        }
    }
    else {
        $global:assemblyVersion = $global:version
        $global:pkgVersion = $global:version
    }

    if ($env:DNX_ASSEMBLY_FILE_VERSION -ne $NULL) {
        [int]$fileVersion = $env:DNX_ASSEMBLY_FILE_VERSION
    }
    else {
        [int]$fileVersion = 0
    }
    $global:assemblyFileVersion = "${global:assemblyVersion}.$fileVersion"

    # Update appveyor build details (-Version must be unique)
    if ($env:APPVEYOR -ne $NULL) {
        Update-AppveyorBuild -Version $global:pkgVersion
    }
}

task VerifyTools {
    # Visual Studio 2015 will exclusively use 2015 MSBuild and C# compilers (assembly version 14.0)
    # and the 2015 Toolset (ToolsVersion 14.0)
    #$version = &"$framework_dir\MSBuild.exe" /nologo /version
    $version = &{msbuild /nologo /version}
    $expectedVersion = "14.0.23107.0"
    Write-Host "Framework directory (GAC) is $framework_dir"
    Write-Host "MSBuild version is $version"
    Assert $version.StartsWith($tools_version) "MSBuild has version '$version'. It should be '$tools_version'."
    Assert ($version -eq $expectedVersion) "MSBuild has version '$version'. It should be '$expectedVersion'."
}

task Clean {
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
    $commit = Get-Git-Commit-Full

    $outdir = $artifacts_dir
    if (-not ($outdir.EndsWith("\"))) {
      $outdir += '\' # MSBuild requires OutDir to end with a trailing slash
    }

    Write-Host "Compiling '$sln_file' with '$configuration' configuration" -ForegroundColor Yellow

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
    create-commonAssemblyInfo $(Get-Git-Commit-Full) "$source_dir\CommonAssemblyInfo.cs"
}

task pack -depends compile {

    # Find dependency versions
    $preludeCoreVersion = find-dependencyVersion (join-path (join-path $source_dir "Brf.Lofus.Core") "packages.config") 'Maxfire.Prelude.Core'
    $preludeTypeConverterVersion = find-dependencyVersion (join-path (join-path $source_dir "Brf.Lofus.Core") "packages.config") 'Maxfire.Prelude.ComponentModel.TypeConverter'

    # Could use the -Version option of the nuget.exe pack command to provide the actual version.
    # _but_ the package dependency version cannot be overriden at the commandline.
    $packages = Get-ChildItem $build_dir *.nuspec -recurse
    $packages | %{
        $nuspec = [xml](Get-Content $_.FullName)
        $nuspec.package.metadata.version = $global:pkgVersion
        $nuspec | Select-Xml '//dependency' | %{
            if ($_.Node.id.StartsWith('Brf.Lofus')) {
                $_.Node.version = $global:pkgVersion
            }
            if ($_.Node.id -eq 'Maxfire.Prelude.Core') {
                $_.Node.version = $preludeCoreVersion
            }
            if ($_.Node.id -eq 'Maxfire.Prelude.ComponentModel.TypeConverter') {
                $_.Node.version = $preludeTypeConverterVersion
            }
        }
        $nuspecFilename = join-path $artifacts_dir (Split-Path -Path $_.FullName -Leaf)
        $nuspec.Save($nuspecFilename)
        exec { & $base_dir\.nuget\Nuget.exe pack -OutputDirectory $artifacts_dir $nuspecFilename }
    }
}

function find-dependencyVersion($packagesConfigPath, $packageId) {

    $dependencies = [xml](Get-Content  $packagesConfigPath)
    $dependencies | Select-Xml '//package' | %{
        if ($_.Node.id -eq $packageId) {
            $packageVersion = $_.Node.version
        }
    }

    if (-not $packageVersion) {
        throw "Could not resolve the version of the $packageId dependency."
    }

    return $packageVersion
}

# -------------------------------------------------------------------------------------------------------------
# generalized functions
# --------------------------------------------------------------------------------------------------------------
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

function global:create-commonAssemblyInfo($commit, $filename)
{
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
[assembly: AssemblyCopyright(""Copyright maxfire 2015-" + $date.Year + ". All rights reserved."")]
[assembly: AssemblyTrademark("""")]
[assembly: ComVisible(false)]

[assembly: AssemblyVersion(""$assemblyVersion"")]
[assembly: AssemblyFileVersion(""$assemblyFileVersion"")]
[assembly: AssemblyInformationalVersion(""$pkgVersion / $commit / "")]
[assembly: AssemblyProduct(""Skat"")]

[assembly: CLSCompliant(true)]

#if DEBUG
[assembly: AssemblyConfiguration(""Debug"")]
#else
[assembly: AssemblyConfiguration(""Release"")]
#endif
" | out-file $filename -encoding "utf8"
}
